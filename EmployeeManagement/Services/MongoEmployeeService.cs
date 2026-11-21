using MongoDB.Driver;
using EmployeeManagement.Models;

namespace EmployeeManagement.Services;

public class MongoEmployeeService : IEmployeeService
{
    private readonly IMongoCollection<Employee> _employees;

    public MongoEmployeeService(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _employees = database.GetCollection<Employee>("employees");

        SeedInitialEmployees();
    }

    public IEnumerable<Employee> GetAll()
    {
        return _employees.Find(FilterDefinition<Employee>.Empty).ToList();
    }

    public Employee? GetById(int id)
    {
        return _employees.Find(employee => employee.Id == id).FirstOrDefault();
    }

    public Employee Create(Employee employee)
    {
        Validate(employee);

        if (Exists(employee.Id))
        {
            throw new InvalidOperationException($"Employee with ID {employee.Id} already exists.");
        }

        try
        {
            _employees.InsertOne(employee);
        }
        catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            throw new InvalidOperationException($"Employee with ID {employee.Id} already exists.", ex);
        }

        return employee;
    }

    public bool Update(int id, Employee updatedEmployee)
    {
        Validate(updatedEmployee);

        var update = Builders<Employee>.Update
            .Set(employee => employee.Name, updatedEmployee.Name.Trim())
            .Set(employee => employee.Department, updatedEmployee.Department.Trim())
            .Set(employee => employee.Salary, updatedEmployee.Salary);

        var result = _employees.UpdateOne(employee => employee.Id == id, update);
        return result.ModifiedCount > 0 || result.MatchedCount > 0;
    }

    public bool Delete(int id)
    {
        var result = _employees.DeleteOne(employee => employee.Id == id);
        return result.DeletedCount > 0;
    }

    public bool Exists(int id)
    {
        return _employees.Find(employee => employee.Id == id).Limit(1).Any();
    }

    private void SeedInitialEmployees()
    {
        if (_employees.EstimatedDocumentCount() > 0)
        {
            return;
        }

        _employees.InsertMany(
            new[]
            {
                new Employee { Id = 101, Name = "Alice Johnson", Department = "IT", Salary = 75000 },
                new Employee { Id = 102, Name = "Bob Smith", Department = "HR", Salary = 60000 }
            });
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
