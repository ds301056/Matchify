using API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add detailed error handling and logging
builder.Services.AddDbContext<DataContext>(opt => 
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"),
        options => options.MigrationsAssembly(typeof(DataContext).Assembly.FullName));
    opt.EnableSensitiveDataLogging();
    opt.EnableDetailedErrors();
});

var app = builder.Build();

// Add database initialization
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<DataContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during database migration");
    }
}

app.MapControllers();
app.Run();