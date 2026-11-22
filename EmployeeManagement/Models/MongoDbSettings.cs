namespace EmployeeManagement.Models;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = "mongodb://localhost:27017";
    public string DatabaseName { get; set; } = "EmployeeManagementDb";
    public string CollectionName { get; set; } = "Employees";
}
