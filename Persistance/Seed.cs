using DatingApplicationBackEnd.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DatingApplicationBackEnd.Persistance
{
    //We will seedData program.cs file bcoz our applicaion starts from program.cs and when it starts at that time we will seed data. Therefore program.cs is the best place to seed data to database.
    public class Seed
    {
        //We have included role manager bcoz we are adding three types of role in AppRole Table
        public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            //It will check in Users table that whether we have any users or not. If table has user then it will return true else false.
            if (await userManager.Users.AnyAsync()) return;

            var userData = await System.IO.File.ReadAllTextAsync("Persistance/UserSeedData.json");

            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            if (users == null) return;

            var roles = new List<AppRole>
            {
                new AppRole { Name = "Member"},
                new AppRole { Name = "Admin"},
                new AppRole { Name = "Moderator"},
            };

            foreach (var item in roles)
            {
                await roleManager.CreateAsync(item);
            }

            foreach (var user in users)
            {
                user.UserName = user.UserName.ToLower();
                await userManager.CreateAsync(user, "Pa$$w0rd");
                await userManager.AddToRoleAsync(user, "Member");
            }

            //Here we are creating new user which has 2 roles Admin and moderator
            var admin = new AppUser
            {
                UserName = "admin"
            };

            await userManager.CreateAsync(admin,"Pa$$w0rd");

            await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });
        }
    }
}
