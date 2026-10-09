using CivicFix.Application.DTOs.Issues;
using CivicFix.Domain.Entities;
using CivicFix.Infrastructure.Data;
using CivicFix.Infrastructure.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CivicFix.Tests.Services;

public class AdminIssueServiceTests
{
    private CivicFixDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<CivicFixDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new CivicFixDbContext(options);
    }

    [Fact]
    public async Task VerifyIssueAsync_ShouldUpdateStatusAndLog()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var adminId = Guid.NewGuid();
        var issue = new Issue 
        { 
            Id = Guid.NewGuid(), 
            Title = "Test", 
            Description = "Test",
            Address = "Test",
            Status = "PENDING", 
            Priority = "LOW",
            UserId = Guid.NewGuid()
        };
        context.Issues.Add(issue);
        await context.SaveChangesAsync();

        var service = new AdminIssueService(context);

        // Act
        await service.VerifyIssueAsync(issue.Id, adminId);

        // Assert
        var updatedIssue = await context.Issues.FindAsync(issue.Id);
        updatedIssue!.Status.Should().Be("VERIFIED");
        
        var logs = await context.AuditLogs.ToListAsync();
        logs.Should().ContainSingle();
        logs[0].Action.Should().Be("Verified");
        logs[0].UserId.Should().Be(adminId);
    }

    [Fact]
    public async Task RejectIssueAsync_WithoutReason_ShouldThrowArgumentException()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var adminId = Guid.NewGuid();
        var issueId = Guid.NewGuid();
        
        var service = new AdminIssueService(context);
        var request = new RejectIssueDto { Reason = "" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.RejectIssueAsync(issueId, request, adminId));
    }
}
