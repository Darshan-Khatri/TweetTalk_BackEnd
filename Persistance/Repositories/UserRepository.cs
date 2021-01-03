using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApplicationBackEnd.Core.Models;
using DatingApplicationBackEnd.DTOs;
using DatingApplicationBackEnd.Helper;
using DatingApplicationBackEnd.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApplicationBackEnd.Persistance.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext context;
        private readonly IMapper mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<MemberDto> GetMemberByUsernameAsync(string username)
        {
            return await context.Users
                    .Where(s => s.UserName == username)
                    .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
                    .SingleOrDefaultAsync();
            throw new NotImplementedException();
        }

        //*********Pagination*************************************
        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var query = context.Users
                    .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
                    .AsNoTracking();

            return await PagedList<MemberDto>.CreateAsync(query, userParams.PageNumber, userParams.PageSize);
        }
        //************************************************************

        public async Task<AppUser> GetUserByIdAsync(int Id)
        {
            return await context.Users.FindAsync(Id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await context.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await context.Users
                .Include(p => p.Photos)
                .ToListAsync();
        }

        public async Task<bool> SavaAllAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            //What this does is it will apply flag to entity which hasbeen modified.
            context.Entry(user).State = EntityState.Modified;
        }
    }
}
