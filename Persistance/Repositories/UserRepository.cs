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
        }

        //*********Pagination*************************************
        /*When request reach here 
         * userParams = pageNumber=1,pageSize=10,currentUsername=lisa,gender=Male,minAge=18,maxAge=150,OrderBy=lastActive
         */
        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var query = context.Users.AsQueryable();

            //Filter queries
            query = query.Where(u => u.UserName != userParams.CurrentUsername);
            query = query.Where(u => u.Gender == userParams.Gender);

            var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1); // Oldest
            var maxDob = DateTime.Today.AddYears(-userParams.MinAge); // Youngest

            query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

            //Sorting queries
            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.Created),

                //We write Default statement of switch statement with "_"(dash).
                _ => query.OrderByDescending(u => u.LastActive)
            };

            return await PagedList<MemberDto>.CreateAsync
                (
                    query.ProjectTo<MemberDto>(mapper.ConfigurationProvider).AsNoTracking(),
                    userParams.PageNumber, userParams.PageSize
                );

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
