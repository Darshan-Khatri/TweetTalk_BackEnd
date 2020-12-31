using DatingApplicationBackEnd.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApplicationBackEnd.DTOs
{
    public class MemberDto
    {
        public int Id { get; set; }
        public string Username { get; set; }

        //it will have main photo Url.
        public string PhotoUrl { get; set; }

        //Our AppUser class has method GetAge(), So when automapper compares MemberDto and AppUser it will see GetAge method in AppUser and property Age in MemberDto so automapper magically calulates dob from AppUser method and give it to MemberDto's age property.
        public int Age { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; } 
        public DateTime LastActive { get; set; } 
        public string Gender { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public ICollection<PhotoDto> Photos { get; set; }
    }
}
