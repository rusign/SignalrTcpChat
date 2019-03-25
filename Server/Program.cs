
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace ChatSpace
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();

            var host = new WebHostBuilder()
                .UseConfiguration(config)
                .UseSetting(WebHostDefaults.PreventHostingStartupKey, "true")
                .ConfigureLogging(factory =>
                {
                    factory.AddConsole();
                })
                .UseKestrel(options =>
                {
                    options.ListenLocalhost(5000);
                    options.Listen(IPAddress.Loopback, 9001, builder =>
                    {
                        builder.UseConnectionHandler<TCPConnectionHandler>();
                    });
                })
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
