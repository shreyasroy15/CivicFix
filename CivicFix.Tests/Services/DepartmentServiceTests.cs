using CivicFix.Application.DTOs.Departments;
using CivicFix.Infrastructure.Data;
using CivicFix.Infrastructure.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;

namespace CivicFix.Tests.Services;

public class DepartmentServiceTests
{
    private CivicFixDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<CivicFixDbContext>()
            .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
            .Options;
        return new CivicFixDbContext(options);
    }

    [Fact]
    public async Task CreateDepartmentAsync_ShouldAddDepartmentAndReturnDto()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new DepartmentService(context);
        var request = new CreateDepartmentDto { Name = "Sanitation", Description = "Cleans roads" };

        // Act
        var result = await service.CreateDepartmentAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Name.Should().Be("Sanitation");
        
        var deptInDb = await context.Departments.FindAsync(result.Id);
        deptInDb.Should().NotBeNull();
        deptInDb!.Name.Should().Be("Sanitation");
    }

    [Fact]
    public async Task GetAllDepartmentsAsync_ShouldReturnAll()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        context.Departments.Add(new Domain.Entities.Department { Name = "D1" });
        context.Departments.Add(new Domain.Entities.Department { Name = "D2" });
        await context.SaveChangesAsync();
        var service = new DepartmentService(context);

        // Act
        var result = await service.GetAllDepartmentsAsync();

        // Assert
        result.Should().HaveCount(2);
    }
}
