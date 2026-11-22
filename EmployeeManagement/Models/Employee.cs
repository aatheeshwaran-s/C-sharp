using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EmployeeManagement.Models;

public class Employee
{
    [BsonId]
    [BsonRepresentation(BsonType.Int32)]
    public int Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("department")]
    public string Department { get; set; } = string.Empty;

    [BsonElement("salary")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Salary { get; set; }
}
