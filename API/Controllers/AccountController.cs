// Import necessary namespaces
using System.Security.Cryptography; // Provides cryptographic services like hashing and encryption
using System.Text; // For encoding strings (e.g., converting strings to byte arrays for hashing)
using API.Data; // Contains the application's database context
using API.DTOs; // Data Transfer Objects (DTOs) for transferring data between client and server
using API.Entities; // Defines database entity models (e.g., AppUser)
using API.Interfaces; // Interfaces for dependency injection, such as ITokenService
using API.Services; // Services implementation
using Microsoft.AspNetCore.Mvc; // Provides attributes and functionality for controllers
using Microsoft.EntityFrameworkCore; // Tools for interacting with Entity Framework Core (database management)

namespace API.Controllers; // Grouping the controller in the API namespace

// Define the AccountController class, inheriting from BaseApiController
// This controller is responsible for user account management (e.g., registration and login)
public class AccountController(DataContext context, ITokenService tokenService) : BaseApiController
{
  // Define the register endpoint (POST /account/register)
  [HttpPost("register")] // Decorator that maps this method to an HTTP POST request to the "register" route
  public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
  {
    // Check if a user with the given username already exists
    if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");

    // Create a new instance of HMACSHA512 for password hashing
    // HMACSHA512 generates a cryptographic hash and a key (salt) to make the hash unique
    using var hmac = new HMACSHA512();

    // Create a new user entity with the provided data
    var user = new AppUser
    {
      UserName = registerDto.Username.ToLower(), // Normalize the username to lowercase for consistency
      PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)), // Compute the hash of the password
      PasswordSalt = hmac.Key // Store the generated key (salt)
    };

    // Add the new user to the database context
    context.Users.Add(user);

    // Save changes to the database
    await context.SaveChangesAsync();

    // Return a DTO containing the username and a generated token
    return new UserDto
    {
      Username = user.UserName,
      Token = tokenService.CreateToken(user) // Generate a token using the token service
    };
  }

  // Define the login endpoint (POST /account/login)
  [HttpPost("login")] // Decorator that maps this method to an HTTP POST request to the "login" route
  public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
  {
    // Find a user by username in the database
    var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());

    // If the user is not found, return an Unauthorized response
    if (user == null) return Unauthorized("Invalid username");

    // Create a new instance of HMACSHA512 using the user's stored salt
    using var hmac = new HMACSHA512(user.PasswordSalt);

    // Hash the provided password with the same salt
    var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

    // Compare the computed hash with the stored password hash
    for (int i = 0; i < computedHash.Length; i++)
    {
      // If any byte in the hash doesn't match, return an Unauthorized response
      if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
    }

    // If the username and password are valid, return a DTO with the username and a generated token
    return new UserDto
    {
      Username = user.UserName,
      Token = tokenService.CreateToken(user) // Generate a token for the user
    };
  }

  // Private helper method to check if a user with a given username exists
  private async Task<bool> UserExists(string username)
  {
    // Query the database to see if any user matches the normalized (lowercase) username
    return await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
  }
}
