using DatingApplicationBackEnd.Extensions;
using DatingApplicationBackEnd.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace DatingApplicationBackEnd.Helper
{
    /*Action Filter 
    => It allows us to do something before request is excuting or after the request is executed
    */
    public class LogUserActivity : IAsyncActionFilter
    {                                     //Access to context before       Access to context after
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //Will wait until request is executed
            var resultContext = await next();

            //The below statement will check whether the user is authenticated or not
            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

            var userId = resultContext.HttpContext.User.GetUserId();

            //This is how you get acces of IUserRepository Service anywhere.
            //NOTE:- For this you have to import "Microsoft.Extensions.DependencyInjection"
            var repo = resultContext.HttpContext.RequestServices.GetService<IUserRepository>();

            var user = await repo.GetUserByIdAsync(userId);
            user.LastActive = DateTime.Now;

            await repo.SavaAllAsync();
        }
    }
}
