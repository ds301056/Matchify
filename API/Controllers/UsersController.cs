using System;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;
// controller for users


public class UsersController(DataContext context) : BaseApiController
{
    [AllowAnonymous]
    [HttpGet] // method to return response to client 
  public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
  {
    // return list of users 
    var users = await context.Users.ToListAsync();

    return users;

  }
    [Authorize]
    [HttpGet("{id:int}")] // api/users/{id}
  public async Task<ActionResult<AppUser>> GetUser(int id)
  {
    // return list of users 
    var user = await context.Users.FindAsync(id); // get user by id 

    if (user == null) return NotFound(); // if user not found return 404

    return user;

  }

}
