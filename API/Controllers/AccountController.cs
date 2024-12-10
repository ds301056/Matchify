using System.Security.Cryptography; // import security namespace
using System.Text; // import text namespace
using API.Data; // import data namespace
using API.DTOs;
using API.Entities; // import entities namespace
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // import mvc namespace

namespace API.Controllers;

public class AccountController(DataContext context, ITokenService tokenService) : BaseApiController // controller for account
{
  [HttpPost("register")] // account/register

  // method to register user
  public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto) 
  {
    if (await UserExists(registerDto.Username)) return BadRequest("Username is taken"); // check if user exists

    using var hmac = new HMACSHA512(); // create new instance of HMACSHA512

    var user = new AppUser // create new user
    {
      UserName = registerDto.Username.ToLower(), // set username _ lowercase
      PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)), // hash password
      PasswordSalt = hmac.Key // set password salt
    };

    context.Users.Add(user); // add user to database
    await context.SaveChangesAsync(); // save changes

    return new UserDto{
      Username = user.UserName,
      Token = tokenService.CreateToken(user) // create token
    }; // return user
  }

  // login endpoint 
  [HttpPost("login")] 
  public async Task<ActionResult<UserDto>> Login(LoginDto loginDto) // login method
  {
    var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower()); // get user

    if (user == null) return Unauthorized("Invalid username"); // return if user is null
  
    using var hmac = new HMACSHA512(user.PasswordSalt); // create new instance of HMACSHA512 using password salt key 

    var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password)); // hash password

    for (int i = 0; i < computedHash.Length; i++) // loop through hash
    {
      if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password"); // return if password is invalid
    }
    return new UserDto
    {
      Username = user.UserName,
      Token = tokenService.CreateToken(user) // create token
    }; // return user
  }


  private async Task<bool> UserExists(string username) // check if user exists
  {
    // remember  Bob !- bob 
    return await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower()); // return if user exists
  }
}
