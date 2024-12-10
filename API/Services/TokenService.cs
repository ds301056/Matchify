using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService(IConfiguration config) : ITokenService
{
    public string CreateToken(AppUser user)
    {
        var tokenKey = config ["TokenKey"] ?? throw new Exception("Cannot access tokenKey from appsettings"); // get the token key from the appsettings.json file  ?? = is null?  

        if (tokenKey.Length < 64) throw new Exception("Token key must be at least 64 characters in length"); // check if the token key is at least 64 characters long

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)); // create a new symmetric security key using the token key

        var claims = new List<Claim> // create a list of claims
        {
            new (ClaimTypes.NameIdentifier, user.UserName) // add the username to the claims
        };

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature); // create a new signing credentials object

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims), // create a new claims identity
            Expires = DateTime.UtcNow.AddDays(7), // set the expiry date of the token to 7 days from now 
            SigningCredentials = creds // set the signing credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler(); // create a new jwt security token handler
        var token = tokenHandler.CreateToken(tokenDescriptor); // create a new token using the token descriptor

        return tokenHandler.WriteToken(token); // return the token
    }
}
