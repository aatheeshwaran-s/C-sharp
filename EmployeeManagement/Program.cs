var builder = WebApplication.CreateBuilder(args);

// Register controllers with the dependency injection container
builder.Services.AddControllers();

var app = builder.Build();

// Map controller routes (e.g. /api/employees)
app.MapControllers();

// Start the web server and listen for incoming HTTP requests
app.Run();
