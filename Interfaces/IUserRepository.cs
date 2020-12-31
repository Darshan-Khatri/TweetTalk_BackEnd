using DatingApplicationBackEnd.Core.Models;
using DatingApplicationBackEnd.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApplicationBackEnd.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);
        Task<bool> SavaAllAsync();
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser> GetUserByIdAsync(int Id);
        Task<AppUser> GetUserByUsernameAsync(string username);


        Task<IEnumerable<MemberDto>> GetMembersAsync();
        Task<MemberDto> GetMemberByUsernameAsync(string username);


    }
}
