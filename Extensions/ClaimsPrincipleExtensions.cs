using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DatingApplicationBackEnd.Extensions
{
    public static class ClaimsPrincipleExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            //ClaimType what you see here is what we have defined in Token Service's Claims-List
            return user.FindFirst(ClaimTypes.Name)?.Value;
        }

        public static int GetUserId(this ClaimsPrincipal user)
        {
            //ClaimType what you see here is what we have defined in Token Service's Claims-List
            return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
    }
}
/*
 * Here ClaimTypes.Name represents UniqueName type in Clamis type in Token Service
 */