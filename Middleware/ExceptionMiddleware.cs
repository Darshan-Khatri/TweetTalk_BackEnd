using DatingApplicationBackEnd.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace DatingApplicationBackEnd.Middleware
{
    public class ExceptionMiddleware
    {
        //RequestDelegate is use to pass http request to next middleware.
        private readonly RequestDelegate next;
        //ILogger is use to show complete detailed error message in TERMINAL WINDOW.
        private readonly ILogger<ExceptionMiddleware> logger;
        //IHostEnvironment is use to check in which environment the has occured.
        private readonly IHostEnvironment env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            this.next = next;
            this.logger = logger;
            this.env = env;
        }

        //We are instantiate HttpContext to use all features of Http.
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                //We are passing http request to below or other middleware until some exception occurs, if exception occured in any of middleware then will move to catch block and see what kind of exception we get and what will do after we get an exception.
                await next(context);
            }
            catch (Exception ex)
            {
                //We are here because in our application some exception is occured. 
                logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = env.IsDevelopment()
                    ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                    : new ApiException(context.Response.StatusCode, "Internal Server Error.");

                var option = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = JsonSerializer.Serialize(response, option);

                await context.Response.WriteAsync(json);
            }
        }
    }
}
