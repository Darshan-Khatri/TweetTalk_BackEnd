using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApplicationBackEnd.Core.Models
{
    public class AppRole : IdentityRole<int>
    {
        //This is just to built relationship with AppUserRole table. Since "AppUserRole" table will have RoleId as the foreign key which is primary key in AppRole table.
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}
