// Importing required namespaces
using System; // Provides base class functionality like Exception handling
using System.Text; // Provides encoding utilities (used for token signing keys)
using Microsoft.AspNetCore.Authentication.JwtBearer; // Enables JWT authentication with ASP.NET Core
using Microsoft.IdentityModel.Tokens; // Provides classes for handling security tokens and validation

// Defining a namespace, grouping related classes and functionality together
namespace API.Extensions;

// A static class to extend the functionality of `IServiceCollection`
// This allows us to encapsulate and reuse service registration logic for cleaner code.
public static class IdentityServiceExtensions
{
  // Static method to add identity-related services to the application
  // `this IServiceCollection services` makes this an extension method for `IServiceCollection`
  public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
  {
    // Add authentication services to the application
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) // Sets the default authentication scheme to JWT
    .AddJwtBearer(options => // Configures JWT bearer authentication
    {
      // Retrieve the token key from configuration (e.g., appsettings.json or environment variables)
      // If the key is not found, throw an exception
      var tokenKey = config["TokenKey"] ?? throw new Exception("TokenKey not found");

      // Configure token validation parameters
      options.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuerSigningKey = true, // Ensures the token is signed with the expected security key
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
        // Creates a symmetric security key using the token key. 
        // Symmetric keys are used because both signing and validation use the same key.

        ValidateIssuer = false, // Disables validation of the token issuer (optional depending on your setup)
        ValidateAudience = false // Disables validation of the token audience (optional depending on your setup)
      };
    });

    // Return the service collection to allow chaining of additional service registrations
    return services;
  }
}
