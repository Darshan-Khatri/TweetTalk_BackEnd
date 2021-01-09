using DatingApplicationBackEnd.Extensions;
using DatingApplicationBackEnd.Middleware;
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //This is our custom middleware for anytype of exception in any type of environment like Developement,production etc.
            //During application run time if some exception is occured then i will be called.
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseRouting();

            app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200"));

            //When server sees any "authorize" property in ActionMethod then request stops here it will internally look for any authentication scheme in IServiceCollection. Here we have that scheme in our IdentityService extension method.
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
