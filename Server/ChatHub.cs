using System;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace ChatSpace
{
    public class ChatHub: Hub
    {
        private readonly ILogger<ChatHub> _logger;
        private MyConnectionManager _cm;

        public ChatHub(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ChatHub>();
            _cm = MyConnectionManager.Instance;
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("New web connection id : " + Context.ConnectionId);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var name = _cm.GetName(Context.ConnectionId);
            _logger.LogInformation(name + " disconected connection id : " + Context.ConnectionId + " ,excep" + exception);
            _cm.RemoveHubConnection(Context.ConnectionId);
            await Clients.All.SendAsync("userDecoFromHub", name);
            await _cm.SendMessageCo(name, name + " has left the chat");
        }

        public async Task sendMessageToHub(string user, string message)
        {
            _logger.LogInformation("sendMessage " + message + ", from :" + user);
            await Clients.All.SendAsync("sendMessageToHub", user, message);
            await _cm.SendMessageCo(user, message);
        }

        public async Task JoinHub(string name)
        {
            var userList = _cm.UserList();
            _cm.AddHubConnection(name, Context.ConnectionId);
            var user = _cm.GetName(Context.ConnectionId);
            _logger.LogInformation("join " + user);
            await Clients.All.SendAsync("sendMessageToHub", user, user + " join the chat");
            await _cm.SendMessageCo(user, user + " join the chat");
            userList.ForEach(async x =>
            {
                await Clients.All.SendAsync("inHub", x);
            });
            
        }
    }
}
