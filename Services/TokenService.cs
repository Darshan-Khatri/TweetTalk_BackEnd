using DatingApplicationBackEnd.Core.Models;
using DatingApplicationBackEnd.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DatingApplicationBackEnd.Services
{
    //This class is only responsible for generating token when user logIn or register.
    public class TokenService : ITokenService
    {
        //The secretKey that we have defined in appsetting.json is used for both data encryption and decryption.
        private readonly SymmetricSecurityKey _key;
        private readonly UserManager<AppUser> userManager;

        public TokenService(IConfiguration config, UserManager<AppUser> userManager)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
            this.userManager = userManager;
        }

        /*This method creates the JWT token as soon as user log-in.(Install nugetPackage "System.IdentityModel.Tokens.jwt")
        1- When user enters username, if username is valid then will get complete user object.
        2- Now we call create token method from TokenService class and pass that user as createToken method parameter.
        3- Once we are inside the createtoken method, We define the claims, in our case we are including username and id as the claim.
        4- Now we specify the SecretKey which we have in our server and Algorithm which gonna a create token with our secretKey.
        5- Now in tokenDescriptor object we have included our claimsList as Subject, Token expire time and SigningCredentials.
        6- JwtSecurityTokenHandler this class is responsible for JWT token creation, so will create object of this class.
        7- JwtSecurityTokenHandler class has a method named as CreateToken which takes tokenDescriptor as an argument to createToken.
        8- Now we have our token created for user who just loggedIn. But we need to convert it into compact serilize format,So we can easily pass into http header.
        9- For that will call another method from JwtSecurityTokenHandler class named as
           WriteToken and we will pass our token into it as a parameter to convert our token into compact serilize format.
        10-Finally we will return this token to that user to store it in local storage for further use.
         */
        public async Task<string> CreateToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
            };

            //we are doing this to add roles in JWT Token claims.
            //So now inside our roles variable we pass List of roles loggedIn user belongs to.
            /*For eg: lisa loggedIn in so role of lisa is "Member" so roles variable would contain "Member". if some other user LoggedIn and it has two roles then roles varible would contain "Admin" and "Moderator".
             */
            var roles = await userManager.GetRolesAsync(user);
            //We are adding new Claim to claims list from here.
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
