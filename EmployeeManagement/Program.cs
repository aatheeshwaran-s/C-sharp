namespace EmployeeManagement;

class Program
{
    static void Main(string[] args)
    {
        // Day 1 - Topic 1: Variables and Data Types
        int employeeId = 101;
        string employeeName = "Alice Johnson";
        decimal salary = 75000.50m;
        bool isFullTime = true;

        Console.WriteLine($"ID: {employeeId}");
        Console.WriteLine($"Name: {employeeName}");
        Console.WriteLine($"Salary: {salary}");
        Console.WriteLine($"Full-Time: {isFullTime}");
    }
}