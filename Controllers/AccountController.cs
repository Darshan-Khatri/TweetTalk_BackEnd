using AutoMapper;
using DatingApplicationBackEnd.Core.Models;
using DatingApplicationBackEnd.DTOs;
using DatingApplicationBackEnd.Interfaces;
using DatingApplicationBackEnd.Persistance;
using Microsoft.AspNetCore.Identity;
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
       
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly ITokenService tokenService;
        private readonly IMapper mapper;

        public AccountController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager, 
            ITokenService tokenService, IMapper mapper)
        {
            
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.tokenService = tokenService;
            this.mapper = mapper;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Account controller");
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Username))
            {
                return BadRequest("Username is taken");
            }

            //As there are many fileds in AppUser and registerDto, we are using IMapper to map from registerDto to AppUser. As we are sending/stroing AppUser to database.
            var user = mapper.Map<AppUser>(registerDto);

            user.UserName = registerDto.Username.ToLower();

            //This statement creates the new user and save it in database automatically.
            var result = await userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            //Here we are assigning role of "Member" for any newly registerd user
            var roleResult = await userManager.AddToRoleAsync(user, "Member");
            if(!roleResult.Succeeded) return BadRequest(roleResult.Errors);
           
            //We are returing UserDto to client if registration in successful. Since below information about user in necessary throughtout application on client side.
            //As there are few fields in userDto therefore we are not using Mapper function to map from RegisterDto to UserDto.
            return Ok(
                new UserDto
                {
                    Username = registerDto.Username,
                    Token = await tokenService.CreateToken(user),
                    KnownAs = user.KnownAs,
                    Gender = user.Gender
                });
        }

        private async Task<bool> UserExists(string username)
        {
            return await userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            /*  This eager loading, It stores the query result in memory as we excute the query. 
             *  Eager loading is must where you need data immediately and below query excuation is depends on above     query result.
             *  If you write query with differed execuation/ Lazy loading in that case your query will not excute until you add ToList, Count etc. It is useful when you want to chain your queries and you don't want to store your query result in-memory.
             *  You should know when to user eager loading and Lazy loading otherwise it can create problem.
             *  Note:- If you're not sure what to use, then go for eager loading, but it is not efficient program since eager loading stores the query results in-memory.
             */

            //We still have access to Users table but after we added identity in our project we have access to that able via userManager.
            var user = await userManager.Users
                .Include(x => x.Photos)
                .SingleOrDefaultAsync(n => n.UserName == loginDto.Username.ToLower()); 

            if (user == null) return Unauthorized("Invalid username or password");

            //After user has enterd correct username we gonna use signInManager to verfiy password of user.
            var result = await signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            //If password in incorrect then we will say that user is Unauthorized.
            if (!result.Succeeded) return Unauthorized();

            return Ok(new UserDto
            {
                Username = user.UserName,
                Token = await tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x=> x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            });
        }
    }
}
