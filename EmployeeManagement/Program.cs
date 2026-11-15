using EmployeeManagement.Services;

var builder = WebApplication.CreateBuilder(args);

// Register Controllers
builder.Services.AddControllers();

// Day 07: Dependency Injection Registration
// We register EmployeeService as a Singleton so in-memory state persists across requests.
// (In Day 09 with EF Core / database, we will switch database services to Scoped).
builder.Services.AddSingleton<IEmployeeService, EmployeeService>();

var app = builder.Build();

// Map routes to controllers
app.MapControllers();

app.Run();
