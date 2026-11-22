using EmployeeManagement.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EmployeeManagement.Services;

public class MongoEmployeeService : IEmployeeService
{
    private readonly IMongoCollection<Employee> _employeesCollection;
    private readonly ILogger<MongoEmployeeService> _logger;

    public MongoEmployeeService(IOptions<MongoDbSettings> settings, ILogger<MongoEmployeeService> logger)
    {
        _logger = logger;

        var clientSettings = MongoClientSettings.FromConnectionString(settings.Value.ConnectionString);
        // Fail fast if MongoDB server is offline instead of waiting 30 seconds
        clientSettings.ServerSelectionTimeout = TimeSpan.FromSeconds(3);

        var client = new MongoClient(clientSettings);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _employeesCollection = database.GetCollection<Employee>(settings.Value.CollectionName);

        // Run seed in background so web server starts listening immediately
        _ = Task.Run(SeedInitialDataAsync);
    }

    private async Task SeedInitialDataAsync()
    {
        try
        {
            var count = await _employeesCollection.CountDocumentsAsync(_ => true);
            if (count == 0)
            {
                var initialEmployees = new List<Employee>
                {
                    new() { Id = 101, Name = "Alice Johnson", Department = "IT", Salary = 75000 },
                    new() { Id = 102, Name = "Bob Smith", Department = "HR", Salary = 60000 }
                };
                await _employeesCollection.InsertManyAsync(initialEmployees);
                _logger.LogInformation("Seeded initial employee records into MongoDB.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not seed initial MongoDB data. Ensure MongoDB is running on mongodb://localhost:27017.");
        }
    }

    public async Task<IEnumerable<Employee>> GetAllAsync()
    {
        return await _employeesCollection.Find(_ => true).ToListAsync();
    }

    public async Task<Employee?> GetByIdAsync(int id)
    {
        return await _employeesCollection.Find(e => e.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Employee> CreateAsync(Employee employee)
    {
        Validate(employee);

        if (await ExistsAsync(employee.Id))
        {
            throw new InvalidOperationException($"Employee with ID {employee.Id} already exists.");
        }

        try
        {
            await _employeesCollection.InsertOneAsync(employee);
        }
        catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            throw new InvalidOperationException($"Employee with ID {employee.Id} already exists.", ex);
        }

        return employee;
    }

    public async Task<bool> UpdateAsync(int id, Employee updatedEmployee)
    {
        Validate(updatedEmployee);

        updatedEmployee.Id = id;
        var result = await _employeesCollection.ReplaceOneAsync(e => e.Id == id, updatedEmployee);
        return result.IsAcknowledged && result.MatchedCount > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await _employeesCollection.DeleteOneAsync(e => e.Id == id);
        return result.IsAcknowledged && result.DeletedCount > 0;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        var count = await _employeesCollection.CountDocumentsAsync(e => e.Id == id);
        return count > 0;
    }

    private static void Validate(Employee employee)
    {
        if (employee.Salary < 0)
        {
            throw new ArgumentException("Salary cannot be negative.");
        }

        if (string.IsNullOrWhiteSpace(employee.Name))
        {
            throw new ArgumentException("Employee name is required.");
        }
    }
}
