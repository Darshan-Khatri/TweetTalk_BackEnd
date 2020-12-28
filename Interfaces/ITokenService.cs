using DatingApplicationBackEnd.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApplicationBackEnd.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}
