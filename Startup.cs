using DatingApplicationBackEnd.Extensions;
using DatingApplicationBackEnd.Middleware;
using DatingApplicationBackEnd.SignalR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApplicationBackEnd
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        //This configuration is defined in appsetting.json file
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddCors();

            //This are our custome Extension Methods
            services.AddApplicationServices(Configuration);
            services.AddIdentityService(Configuration);

            //Adding signalR service, Now we also need to tell are routing about Api/Hub end points.
            //So go to Configure method below and and end point routing for Hub.
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //This is our custom middleware for anytype of exception in any type of environment like Developement,production etc.
            //During application run time if some exception is occured then i will be called.
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseRouting();

            app.UseCors(x => x.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithOrigins("http://localhost:4200"));

            //When server sees any "authorize" property in ActionMethod then request stops here it will internally look for any authentication scheme in IServiceCollection. Here we have that scheme in our IdentityService extension method.
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                //This takes care of setting up our hubs => Now we also need to take care authorization bcoz our hub will get authenticated.
                //So will learn now how to authenticate user in SignaR => Go to PresenceHub.cs and apply authorize Tag their and then set up Authentication in "IdentityServiceExtension".
                endpoints.MapHub<PresenceHub>("hubs/presence");
                endpoints.MapHub<MessageHub>("hubs/message");
            });
        }
    }
}
