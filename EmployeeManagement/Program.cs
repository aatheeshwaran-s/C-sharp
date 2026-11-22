using EmployeeManagement.Models;
using EmployeeManagement.Services;

var builder = WebApplication.CreateBuilder(args);

// Read MongoDB configuration (supports both "MongoDbSettings" and "MongoDB" sections)
builder.Services.Configure<MongoDbSettings>(options =>
{
    var configSection = builder.Configuration.GetSection("MongoDbSettings");
    var legacySection = builder.Configuration.GetSection("MongoDB");

    options.ConnectionString = configSection["ConnectionString"] 
        ?? legacySection["ConnectionString"] 
        ?? "mongodb://localhost:27017/?directConnection=true";

    options.DatabaseName = configSection["DatabaseName"] 
        ?? legacySection["DatabaseName"] 
        ?? "EmployeeManagementDb";

    options.CollectionName = configSection["CollectionName"] ?? "Employees";
});

// Register Controllers
builder.Services.AddControllers();

// Register MongoEmployeeService in DI
builder.Services.AddSingleton<IEmployeeService, MongoEmployeeService>();

var app = builder.Build();

app.MapControllers();

app.Run();
