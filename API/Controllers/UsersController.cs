using System;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
// controller for users
[ApiController]
[Route("api/[controller]")] // localhost:5000/api/users

public class UsersController(DataContext context) : ControllerBase
{
    [HttpGet] // method to return response to client 
  public ActionResult<IEnumerable<AppUser>> GetUsers()
  {
    // return list of users 
    var users = context.Users.ToList();

    return users;

  }

    [HttpGet("{id}")] // api/users/{id}
  public ActionResult<AppUser> GetUser(int id)
  {
    // return list of users 
    var user = context.Users.Find(id); // get user by id 

    if (user == null) return NotFound(); // if user not found return 404

    return user;

  }

}
