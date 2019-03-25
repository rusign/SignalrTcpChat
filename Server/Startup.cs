using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;


namespace ChatSpace
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddConnections();
            services.AddSignalR(options =>
            {
                options.KeepAliveInterval = TimeSpan.FromSeconds(5);
            });

            services.AddCors(o =>
            {
                o.AddPolicy("CorePolicy", p =>
                {
                    p.AllowAnyHeader()
                     .AllowAnyMethod()
                     .SetIsOriginAllowed((host) => true)
                     .AllowCredentials();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("CorePolicy");

            app.UseSignalR(routes =>
            {
                routes.MapHub<ChatHub>("/chatter");
            });
        }
    }
}
