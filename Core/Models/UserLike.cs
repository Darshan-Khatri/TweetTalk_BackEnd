using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApplicationBackEnd.Core.Models
{
    public class UserLike
    {
        //User that likes other user
        public AppUser SourceUser { get; set; }

        public int SourceUserId { get; set; }


        //User that liked my profile
        public AppUser LikedUser { get; set; }

        public int LikedUserId { get; set; }
    }
}
