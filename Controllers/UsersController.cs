using AutoMapper;
using DatingApplicationBackEnd.Core.Models;
using DatingApplicationBackEnd.DTOs;
using DatingApplicationBackEnd.Interfaces;
using DatingApplicationBackEnd.Persistance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DatingApplicationBackEnd.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;

        //To use all database tables you must instantiate your DataContext class in controllers constructor.  
        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetAllUser()
        {
            var query = await userRepository.GetMembersAsync();
            return Ok(query);
        }

        //If you don't specify anything as input parameter in HttpGet request and use paramter in action method then 
        //that parameter becomes the optional parameter/parameters.
        //But for now we are making it compulsary so we are explicitily defining parameter in curly brackets{Id}.
        /*This action method can't be accessiable directly by writing Default route URL, if you do then it would give you error "401"(Unauthorized user). System will look for any authorization scheme in startup.cs file.
         * So lets go to startup.cs file and see where we have defined our Autorization scheme.
         * Meet you in startup.cs file.
         */
        /*[HttpGet("GetUser/{Id}")]
        public async Task<ActionResult<AppUser>> GetUser(int Id)
        {
            var query = await userRepository.GetUserByIdAsync(Id);
            return Ok(query);
        }*/


        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await userRepository.GetMemberByUsernameAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            //This will gives us username from the token that api uses to authenticate user.
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await userRepository.GetUserByUsernameAsync(username);

            //Need to map from memberUpdateDto to AppUser
            mapper.Map(memberUpdateDto, user);

            userRepository.Update(user);
            if (await userRepository.SavaAllAsync()) return NoContent();
            return BadRequest("Fails to update user!!");
        }
    }
}
