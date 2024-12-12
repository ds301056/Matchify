// Importing required namespaces
using API.Extensions; // Custom extension methods to add services or other custom features

// The `WebApplication` builder is used to set up a new application.
var builder = WebApplication.CreateBuilder(args); // This initializes a new instance of the application and sets up basic configurations.

// Add services to the dependency injection container.
// Dependency injection is a design pattern that provides services or objects when needed, improving modularity and testability.
builder.Services.AddApplicationServices(builder.Configuration);
// `AddApplicationServices` is likely a custom extension method in `API.Extensions`. It might configure database services, repositories, or other application-specific services.

builder.Services.AddIdentityServices(builder.Configuration);
// `AddIdentityServices` is another custom method, probably sets up authentication, such as managing user accounts, login, and JWT setup.

// Build the application
var app = builder.Build(); // Creates an application object with the specified configuration and services.

// Configure the HTTP request pipeline
// Middleware is added here to handle requests and responses in a specific sequence.

// Enable CORS (Cross-Origin Resource Sharing) to allow requests from specific origins.
app.UseCors(x => x
    .AllowAnyHeader() // Allows any HTTP header
    .AllowAnyMethod() // Allows any HTTP method (e.g., GET, POST, PUT, DELETE)
    .WithOrigins("http://localhost:4200", "https://localhost:4200"));
// Only requests from these origins will be allowed. This is typically used for enabling a frontend (like Angular/React) to interact with the backend.

// Add middleware for authentication and authorization.
// Middleware processes incoming requests and decides whether they should proceed further.
app.UseAuthentication(); // Enables authentication using the previously configured JWT bearer or other methods.
app.UseAuthorization(); // Checks if the user has the necessary permissions (roles or policies) to access specific endpoints.

// Map controllers to handle requests
app.MapControllers();
// Tells the application to use controllers to handle HTTP requests, such as API endpoints.

// Run the application
app.Run();
// Starts the application and listens for incoming HTTP requests on the configured port (default is 5000 for HTTP and 5001 for HTTPS).
