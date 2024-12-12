// Importing required namespaces
using System; // Provides basic functionality like exception handling
using API.Data; // Custom namespace, likely contains `DataContext` for database interactions
using API.Interfaces; // Defines interfaces for services, such as `ITokenService`
using API.Services; // Contains the implementation of services, such as `TokenService`
using Microsoft.EntityFrameworkCore; // Provides tools for working with Entity Framework Core (database management)

namespace API.Extensions;

// A static class to extend the functionality of `IServiceCollection`
// Used to encapsulate application-specific service configurations for cleaner code.
public static class ApplicationServiceExtensions
{
  // Static method to add application-related services to the service container
  // `this IServiceCollection services` makes this an extension method for `IServiceCollection`
  public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
  {
    // Adds support for controllers, enabling the app to handle HTTP requests via controller classes.
    services.AddControllers();

    // Configures the application's database context.
    // `AddDbContext` registers `DataContext` (your database context class) with dependency injection.
    services.AddDbContext<DataContext>(opt =>
    {
      // Specifies SQLite as the database provider.
      // The connection string is retrieved from the configuration (e.g., appsettings.json or environment variables).
      opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
    });

    // Adds Cross-Origin Resource Sharing (CORS) service.
    // CORS is necessary to allow your frontend (e.g., React or Angular) to interact with the backend API.
    services.AddCors();

    // Adds `TokenService` to the DI container with a scoped lifetime.
    // Scoped means a new instance of the service is created for each HTTP request.
    // `ITokenService` is the interface, and `TokenService` is its implementation.
    services.AddScoped<ITokenService, TokenService>();

    // Return the modified service collection to allow further chaining.
    return services;
  }
}
