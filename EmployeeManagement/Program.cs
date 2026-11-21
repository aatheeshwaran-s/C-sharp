using EmployeeManagement.Services;

var builder = WebApplication.CreateBuilder(args);

// Register Controllers
builder.Services.AddControllers();

var mongoConnectionString = builder.Configuration["MongoDB:ConnectionString"];
var mongoDatabaseName = builder.Configuration["MongoDB:DatabaseName"];

if (string.IsNullOrWhiteSpace(mongoConnectionString) || string.IsNullOrWhiteSpace(mongoDatabaseName))
{
    throw new InvalidOperationException(
        "MongoDB configuration is missing. Set MongoDB:ConnectionString and MongoDB:DatabaseName.");
}

builder.Services.AddSingleton<IEmployeeService>(
    _ => new MongoEmployeeService(mongoConnectionString, mongoDatabaseName));

var app = builder.Build();

// Map routes to controllers
app.MapControllers();

app.Run();
