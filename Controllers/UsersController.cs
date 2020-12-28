using DatingApplicationBackEnd.Core.Models;
using DatingApplicationBackEnd.Persistance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApplicationBackEnd.Controllers
{
    public class UsersController : BaseApiController
    {
        private readonly DataContext context;

        public UsersController(DataContext context)
        {
            this.context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetAllUser()
        {
            var query = await context.Users.ToListAsync();
            return Ok(query);
        }

        //If you don't specify anything as input parameter in HttpGet request and use paramter in action method then 
        //that parameter becomes the optional parameter/parameters.
        //Bu t for now we are making it compulsary so we are explicitily defining parameter in curly brackets{Id}.
        [Authorize]
        [HttpGet("GetUser/{Id}")]
        public async Task<ActionResult<AppUser>> GetUser(int Id)
        {
            var query = await context.Users.FindAsync(Id);
            return Ok(query);
        }
    }
}
