using DatingApplicationBackEnd.Core.Models;
using DatingApplicationBackEnd.DTOs;
using DatingApplicationBackEnd.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApplicationBackEnd.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);
        
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser> GetUserByIdAsync(int Id);
        Task<AppUser> GetUserByUsernameAsync(string username);


        Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);


        Task<MemberDto> GetMemberByUsernameAsync(string username);

        Task<string> GetUserGender(string username);
    }
}
