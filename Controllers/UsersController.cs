using AutoMapper;
using DatingApplicationBackEnd.Core.Models;
using DatingApplicationBackEnd.DTOs;
using DatingApplicationBackEnd.Extensions;
using DatingApplicationBackEnd.Helper;
using DatingApplicationBackEnd.Interfaces;
using DatingApplicationBackEnd.Persistance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        private readonly IPhotoService photoService;

        //To use all database tables you must instantiate your DataContext class in controllers constructor.  
        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
            this.photoService = photoService;
        }

        /*This action methods can't be accessiable directly by writing Default route URL, if you do then it would give you error "401"(Unauthorized user). System will look for any authorization scheme in startup.cs file.
        * So lets go to startup.cs file and see where we have defined our Autorization scheme.
        * Meet you in startup.cs file.
        */

        //{{url}}/users?pageNumber=1&pageSize=10&gender=Male&minAge=18&maxAge=150
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetAllUser([FromQuery]UserParams userParams)
        {
            var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
            userParams.CurrentUsername = user.UserName;

            /*  If you don't specify anything in request header for Gender type i.e if userParams.Gender is empty or        null then we will select Gender opposite of current loggedIn user gender.
                For ex:- LoggedIn user is Male then will set Gender as Female.
                - When request comes here it has following paramters in request header
                {{url}}/users?pageNumber=1&pageSize=10&currentUsername=lisa&minAge=18&maxAge=150&OrderBy=lastActive
             */

            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = user.Gender == "male" ? "female" : "male";
            }

            //Request header now => 
            //{{url}}/users?pageNumber=1&pageSize=10&currentUsername=lisa&gender=Male&minAge=18&maxAge=150&OrderBy=lastActive
            var query = await userRepository.GetMembersAsync(userParams);

            //This will go to response header
            Response.AddPaginationHeader(query.CurrentPage, query.PageSize, query.TotalCount, query.TotalPages);

            return Ok(query);
        }

        //If you don't specify anything as input parameter in HttpGet request and use paramter in action method then 
        //that parameter becomes the optional parameter/parameters.
        //But for now we are making it compulsary so we are explicitily defining parameter in curly brackets{Id}.

        /*[HttpGet("GetUser/{Id}")]
        public async Task<ActionResult<AppUser>> GetUser(int Id)
        {
            var query = await userRepository.GetUserByIdAsync(Id);
            return Ok(query);
        }*/

        //In response header of add-photo action method we will get this http.
        //So the client comes to know for which user we have created this photo.
        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await userRepository.GetMemberByUsernameAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            //User.GetUsername() => This will gives us username from the token that api uses to authenticate user.
            /* As you know our Claims(username, email, id etc) are encrypted in our JWT tokens. To get/extract claims from encrypted token we are using ClaimsPrinciple property "User" for it and we get our username back from it.
             */
            var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());

            //Need to map from memberUpdateDto to AppUser
            mapper.Map(memberUpdateDto, user);

            userRepository.Update(user);
            if (await userRepository.SavaAllAsync()) return NoContent();
            return BadRequest("Fails to update user!!");
        }

        /* 1- Client send as a body in this http post request.
         * 2- Now our server will send this photo file to Cloudinary service.
         * 3- After successful photo upload in cloudinary server, it sends back the response and we receive this response     and sends back this response to client.
         */
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());

            var result = await photoService.AddPhotoAsync(file);

            if (result.Error != null)
            {
                return BadRequest(result.Error.Message);
            }

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if (user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }

            /* Our Photos property inside AppUser/AppUserDto is ICollection, we are adding new photo to ICollection  
             * using its ADD method and when we add Photos to AppUser internally it is added to our photos table
             * due to relationship between two tables. 
             * It is giving abstraction like we are adding photo to our User table but it is not true we are ultimately 
             * adding that photo to photos tables with AppUserId as logged in user.
             */
            user.Photos.Add(photo);

            if (await userRepository.SavaAllAsync())
            {
                return CreatedAtRoute("GetUser", new { username = user.UserName }, mapper.Map<PhotoDto>(photo));
            }
            return BadRequest("Problem in adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            //We need to use query with eager load otherwise it will lead to error. 
            var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo.IsMain) return BadRequest("This is already your main photo");

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;

            if (await userRepository.SavaAllAsync()) return NoContent();

            return BadRequest("Failed to set main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null) return NotFound();
            if (photo.IsMain) return BadRequest("You can not delete main photo");

            if (photo.PublicId != null)
            {
                var result = await photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);

            if (await userRepository.SavaAllAsync()) return Ok();

            return BadRequest("Failed to delete photo !!");
        }
    }
}
