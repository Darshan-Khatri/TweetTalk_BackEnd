using DatingApplicationBackEnd.Core.Models;
using DatingApplicationBackEnd.DTOs;
using DatingApplicationBackEnd.Persistance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Username))
            {
                return BadRequest("Username is taken");
            }
            
            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };
            //Don not add anything
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            return Ok(user);
        }

        private async Task<bool> UserExists(string username)
        {
            return await context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }

        [HttpPost("login")]
        public async Task<ActionResult<AppUser>> Login(LoginDto loginDto)
        {
            var user = await context.Users.SingleOrDefaultAsync(n => n.UserName == loginDto.Username); 
            if (user == null) return Unauthorized("Invalid username or password");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0 ; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                {
                    return Unauthorized("Invalid password");
                }
            }

            return Ok(user);
        }
    }
}
