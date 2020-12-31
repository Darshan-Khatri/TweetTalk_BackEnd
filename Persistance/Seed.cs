using DatingApplicationBackEnd.Core.Models;
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
        public static async Task SeedUsers(DataContext context)
        {
            //It will check in User table that whether we have any users or not. If table has user then it will return true else false.
            if (await context.Users.AnyAsync()) return;

            var userData = await System.IO.File.ReadAllTextAsync("Persistance/UserSeedData.json");

            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            foreach (var user in users)
            {
                using var hmac = new HMACSHA512();
                user.UserName = user.UserName.ToLower();
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
                user.PasswordSalt = hmac.Key;

                context.Users.Add(user);
            }

            await context.SaveChangesAsync();
        }
    }
}
