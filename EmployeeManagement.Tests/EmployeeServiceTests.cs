using EmployeeManagement.Models;
using EmployeeManagement.Services;
using Xunit;

namespace EmployeeManagement.Tests;

public class EmployeeServiceTests
{
    private readonly EmployeeService _sut;

    public EmployeeServiceTests()
    {
        _sut = new EmployeeService();
    }

    [Fact]
    public async Task GetAll_ReturnsInitialEmployees()
    {
        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Contains(result, e => e.Name == "Alice Johnson");
        Assert.Contains(result, e => e.Name == "Bob Smith");
    }

    [Fact]
    public async Task GetById_WhenEmployeeExists_ReturnsEmployee()
    {
        // Arrange
        int existingId = 101;

        // Act
        var result = await _sut.GetByIdAsync(existingId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existingId, result.Id);
        Assert.Equal("Alice Johnson", result.Name);
    }

    [Fact]
    public async Task GetById_WhenEmployeeDoesNotExist_ReturnsNull()
    {
        // Arrange
        int nonExistingId = 999;

        // Act
        var result = await _sut.GetByIdAsync(nonExistingId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Create_ValidEmployee_AddsAndReturnsEmployee()
    {
        // Arrange
        var newEmployee = new Employee
        {
            Id = 103,
            Name = "Charlie Brown",
            Department = "Finance",
            Salary = 80000
        };

        // Act
        var created = await _sut.CreateAsync(newEmployee);
        var retrieved = await _sut.GetByIdAsync(103);

        // Assert
        Assert.NotNull(created);
        Assert.Equal(103, created.Id);
        Assert.NotNull(retrieved);
        Assert.Equal("Charlie Brown", retrieved.Name);
    }

    [Fact]
    public async Task Create_NegativeSalary_ThrowsArgumentException()
    {
        // Arrange
        var invalidEmployee = new Employee
        {
            Id = 104,
            Name = "Dave",
            Department = "IT",
            Salary = -5000
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateAsync(invalidEmployee));
        Assert.Equal("Salary cannot be negative.", ex.Message);
    }

    [Fact]
    public async Task Create_DuplicateId_ThrowsInvalidOperationException()
    {
        // Arrange
        var duplicateEmployee = new Employee
        {
            Id = 101,
            Name = "Imposter",
            Department = "Security",
            Salary = 50000
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CreateAsync(duplicateEmployee));
        Assert.Contains("already exists", ex.Message);
    }

    [Fact]
    public async Task Update_ExistingEmployee_ReturnsTrueAndUpdatesDetails()
    {
        // Arrange
        int idToUpdate = 101;
        var updatedData = new Employee
        {
            Id = idToUpdate,
            Name = "Alice Senior Johnson",
            Department = "Executive",
            Salary = 120000
        };

        // Act
        var isUpdated = await _sut.UpdateAsync(idToUpdate, updatedData);
        var employee = await _sut.GetByIdAsync(idToUpdate);

        // Assert
        Assert.True(isUpdated);
        Assert.NotNull(employee);
        Assert.Equal("Alice Senior Johnson", employee.Name);
        Assert.Equal(120000, employee.Salary);
    }

    [Fact]
    public async Task Update_NonExistingEmployee_ReturnsFalse()
    {
        // Arrange
        int nonExistingId = 999;
        var updatedData = new Employee { Id = nonExistingId, Name = "Nobody", Salary = 50000 };

        // Act
        var isUpdated = await _sut.UpdateAsync(nonExistingId, updatedData);

        // Assert
        Assert.False(isUpdated);
    }

    [Fact]
    public async Task Delete_ExistingEmployee_ReturnsTrueAndRemoves()
    {
        // Arrange
        int idToDelete = 102;

        // Act
        var isDeleted = await _sut.DeleteAsync(idToDelete);
        var searchResult = await _sut.GetByIdAsync(idToDelete);

        // Assert
        Assert.True(isDeleted);
        Assert.Null(searchResult);
    }

    [Fact]
    public async Task Delete_NonExistingEmployee_ReturnsFalse()
    {
        // Arrange
        int nonExistingId = 999;

        // Act
        var isDeleted = await _sut.DeleteAsync(nonExistingId);

        // Assert
        Assert.False(isDeleted);
    }
}
