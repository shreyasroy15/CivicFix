using System;
using System.Linq;
using System.Threading.Tasks;
using CivicFix.Application.DTOs.Issues;
using CivicFix.Domain.Entities;
using CivicFix.Infrastructure.Data;
using CivicFix.Infrastructure.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CivicFix.Tests.Services;

public class IssueServiceTests
{
    private readonly CivicFixDbContext _context;
    private readonly IssueService _issueService;

    public IssueServiceTests()
    {
        var options = new DbContextOptionsBuilder<CivicFixDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new CivicFixDbContext(options);
        _issueService = new IssueService(_context);
    }

    [Fact]
    public async Task CreateIssueAsync_ValidRequest_CreatesIssue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, UserName = "testuser", Email = "test@test.com" };
        _context.Users.Add(user);
        
        var category = new Category { Id = 1, Name = "Road" };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        var request = new CreateIssueDto
        {
            Title = "Pothole",
            Description = "Big pothole on main street",
            CategoryId = 1,
            Latitude = 12.34,
            Longitude = 56.78,
            Address = "Main St",
            ImageUrls = new System.Collections.Generic.List<string> { "http://image.url" }
        };

        // Act
        var result = await _issueService.CreateIssueAsync(userId, request);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Pothole");
        result.CategoryName.Should().Be("Road");
        
        var dbIssue = await _context.Issues.Include(i => i.Images).FirstOrDefaultAsync(i => i.Id == result.Id);
        dbIssue.Should().NotBeNull();
        dbIssue!.Images.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetIssueByIdAsync_NonExistent_ThrowsKeyNotFoundException()
    {
        // Act
        Func<Task> act = async () => await _issueService.GetIssueByIdAsync(Guid.NewGuid());

        // Assert
        await act.Should().ThrowAsync<System.Collections.Generic.KeyNotFoundException>();
    }
}
