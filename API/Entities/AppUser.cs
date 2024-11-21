using System;

namespace API.Entities;

public class AppUser
{
  
  // entity framework must be public to access the properties
  public int Id { get; set; } // Id is an integer _ default value is 0

  public required string UserName { get; set; } // username is a string & required

}
