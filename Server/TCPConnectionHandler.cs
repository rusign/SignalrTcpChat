using System;
using System.Buffers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ChatSpace
{
    /// Handles incoming connections and implements the SignalR Hub Protocol.
    public class TCPConnectionHandler : HubConnectionHandler<ChatHub>
    {
        private readonly ILogger<TCPConnectionHandler> _logger;
        private readonly IHubContext<ChatHub> _hubContext;
        public TCPConnectionHandler(HubLifetimeManager<ChatHub> lifetimeManager, IHubProtocolResolver protocolResolver, IOptions<HubOptions> globalHubOptions, IOptions<HubOptions<ChatHub>> hubOptions, ILoggerFactory loggerFactory, IUserIdProvider userIdProvider, HubDispatcher<ChatHub> dispatcher, IHubContext<ChatHub> hubContext) : base(lifetimeManager, protocolResolver, globalHubOptions, hubOptions, loggerFactory, userIdProvider, dispatcher)
        {
            _logger = loggerFactory.CreateLogger<TCPConnectionHandler>();
            _hubContext = hubContext;
        }

        string GetAsciiString(ReadOnlySequence<byte> buffer)
        {
            if (buffer.IsSingleSegment)
            {
                return Encoding.ASCII.GetString(buffer.First.Span);
            }

            return string.Create((int)buffer.Length, buffer, (span, sequence) =>
            {
                foreach (var segment in sequence)
                {
                    Encoding.ASCII.GetChars(segment.Span, span);

                    span = span.Slice(segment.Length);
                }
            });
        }

        public override async Task OnConnectedAsync(ConnectionContext connection)
        {
            await connection.Transport.Output.WriteAsync(Encoding.ASCII.GetBytes("Welcome to the chat \r\nPlease enter your username :"));
            var res = await connection.Transport.Input.ReadAsync();
            var buf = res.Buffer;
            var name = GetAsciiString(buf);
            connection.Transport.Input.AdvanceTo(buf.End);
            res = await connection.Transport.Input.ReadAsync();
            buf = res.Buffer;
            name = Regex.Replace(GetAsciiString(buf), @"\t|\n|\r", "");
            connection.Transport.Input.AdvanceTo(buf.End);
            _logger.LogInformation("New tcp connection id : " + connection.ConnectionId + ", name :" + name);
            MyConnectionManager.Instance.AddTcpConnection(name, connection);
            await MyConnectionManager.Instance.SendMessageCo(MyConnectionManager.Instance.GetName(connection), name + " join the chat");
            await _hubContext.Clients.All.SendAsync("sendMessageToHub", MyConnectionManager.Instance.GetName(connection), name + " join the chat");
            try
            {
                while (true)
                {
                    var result = await connection.Transport.Input.ReadAsync();
                    var buffer = result.Buffer;
                        if (!buffer.IsEmpty)
                        {
                            var stringRes = GetAsciiString(buffer);
                            await MyConnectionManager.Instance.SendMessageCo(MyConnectionManager.Instance.GetName(connection), stringRes);
                            await _hubContext.Clients.All.SendAsync("sendMessageToHub", MyConnectionManager.Instance.GetName(connection), stringRes);
                            connection.Transport.Input.AdvanceTo(buffer.End);
                        }
                        else if (result.IsCompleted)
                        {
                            break;
                        }
                }
            }
            finally
            {
                var deconame = MyConnectionManager.Instance.GetName(connection);

                MyConnectionManager.Instance.RemoveTcpConnection(connection);
                _logger.LogInformation(connection.ConnectionId + " deconected");
                await MyConnectionManager.Instance.SendMessageCo(deconame, deconame + "has left the chat");
                // await _hubContext.Clients.All.SendAsync("sendMessageToHub", deconame, stringRes);
                await _hubContext.Clients.All.SendAsync("userDecoFromHub", deconame);
            }
        }
    }
}
