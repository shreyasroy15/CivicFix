using CivicFix.Application.DTOs.Staff;
using CivicFix.Domain.Entities;
using CivicFix.Infrastructure.Data;
using CivicFix.Infrastructure.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Threading.Tasks;
using Xunit;
using System.Collections.Generic;

namespace CivicFix.Tests.Services;

public class StaffServiceTests
{
    private CivicFixDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<CivicFixDbContext>()
            .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
            .Options;
        return new CivicFixDbContext(options);
    }

    private Mock<UserManager<User>> GetMockUserManager()
    {
        var store = new Mock<IUserStore<User>>();
        return new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
    }

    [Fact]
    public async Task CreateStaffAsync_WithInvalidDepartmentId_ShouldThrowArgumentException()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var mockUserManager = GetMockUserManager();
        var service = new StaffService(mockUserManager.Object, context);
        
        var request = new CreateStaffDto { Email = "staff@test.com", DepartmentId = 999 };

        // Act & Assert
        await Assert.ThrowsAsync<System.ArgumentException>(() => service.CreateStaffAsync(request));
    }

    [Fact]
    public async Task AssignDepartmentAsync_ShouldUpdateDepartment()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var mockUserManager = GetMockUserManager();
        var service = new StaffService(mockUserManager.Object, context);
        
        var dept = new Department { Name = "Test Dept" };
        context.Departments.Add(dept);
        await context.SaveChangesAsync();
        
        var userId = System.Guid.NewGuid();
        var user = new User { Id = userId, Email = "test@test.com" };
        
        mockUserManager.Setup(x => x.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
        mockUserManager.Setup(x => x.IsInRoleAsync(user, Role.Staff)).ReturnsAsync(true);
        mockUserManager.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);
        
        var request = new AssignDepartmentDto { DepartmentId = dept.Id };

        // Act
        var result = await service.AssignDepartmentAsync(userId, request);

        // Assert
        result.Should().NotBeNull();
        result.DepartmentId.Should().Be(dept.Id);
    }
}
