using DatingApplicationBackEnd.Core.Models;
using DatingApplicationBackEnd.Persistance;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DatingApplicationBackEnd.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext context;

        public AccountController(DataContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult index()
        {
            return Ok("Account controller");
        }

        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register(string username, string password)
        {
            if (username == "" || username == null || password == "" || password == null)
            {
                return BadRequest("Invalid username or password");
            }
            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
                UserName = username,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)),
                PasswordSalt = hmac.Key
            };
            //Don not add anything
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            return Ok(user);
        }
    }
}
