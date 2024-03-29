﻿using DatingApplicationBackEnd.Core.Models;
using DatingApplicationBackEnd.Persistance;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApplicationBackEnd.Extensions
{
    //Here we are telling to our .NetCore that we are using JWT token for authorization and authentication of user.
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityService(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddIdentityCore<AppUser>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = false;
            })
                .AddRoles<AppRole>()
                .AddRoleManager<RoleManager<AppRole>>()
                .AddSignInManager<SignInManager<AppUser>>()
                .AddRoleValidator<RoleValidator<AppRole>>()
                .AddEntityFrameworkStores<DataContext>();

            /* 1- When user login into application, server generates the token for that user and that token is stored  in client browser.
             * 2- In our UserController, we have made all method "authorize".
             * 3- So when user tries to use that actionMethod, our server will validate userToken which is stored in user's browser local storage. These authentication process is done here.
             * 4- Here we are defining "JwtBearerDefaults" as a default scheme for authenticate/authorize user.
             * 5- Now we are defining paramters used to authenticate user ("TokenValidationParameters").
             * 6- For simplicity currenlty we are using IssuerSigningKey as the only authentication paramter.
             * 7- You must add "app.UseAuthentication()" middleware before "app.UseAuthorization()" in startup.cs file.
             */
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(Options =>
                {
                    //JWT token is added to is added to authorization header by default
                    Options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["TokenKey"])),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                    };

                    //SignalR passes authorization in query params not in Header. 
                    //This allows our client to send token as query string
                    Options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            //When client request some messaging feature then it passes signalR access_token in http request query params to our server, so that access_token is received here.
                            var accessToken = context.Request.Query["access_token"];

                            var path = context.HttpContext.Request.Path;    //Below string must match with what you have specified in startup.cs file.
                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
                    };

                });

            //we are using authorize service for user authorization role based on user role.
            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                opt.AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin","Moderator"));
            });

            return services;
        }
    }
}
