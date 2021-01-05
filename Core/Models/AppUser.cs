using DatingApplicationBackEnd.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApplicationBackEnd.Core.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; } = DateTime.Now;
        public string Gender { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public ICollection<Photo> Photos { get; set; }

        //Our AppUser class has method GetAge(), So when automapper compares MemberDto and AppUser it will see GetAge method in AppUser and property Age in MemberDto so automapper magically calulates dob from AppUser method and give it to MemberDto's age property.
        /*public int GetAge() 
        {
            return DateOfBirth.CalculateAge(); 
        }*/

        //Who has liked currently loggedIn user.
        public ICollection<UserLike> LikedByUsers { get; set; }

        //list of user which is liked by loggedIn user.
        public ICollection<UserLike> LikedUsers { get; set; }
    }
}
