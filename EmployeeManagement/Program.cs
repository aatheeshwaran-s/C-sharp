using EmployeeManagement.Models;
using EmployeeManagement.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure MongoDB options from appsettings.json
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

// Register Controllers
builder.Services.AddControllers();

// Register MongoEmployeeService as a Singleton (recommended for MongoClient and Mongo collections)
builder.Services.AddSingleton<IEmployeeService, MongoEmployeeService>();

var app = builder.Build();

app.MapControllers();

app.Run();
