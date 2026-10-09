using CivicFix.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CivicFix.Infrastructure.Data;

public class CivicFixDbContext : IdentityDbContext<User, Role, Guid>
{
    public CivicFixDbContext(DbContextOptions<CivicFixDbContext> options) : base(options)
    {
    }

    public DbSet<Issue> Issues { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<IssueImage> IssueImages { get; set; } = null!;
    public DbSet<Department> Departments { get; set; } = null!;
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<Issue>()
            .HasOne(i => i.Category)
            .WithMany(c => c.Issues)
            .HasForeignKey(i => i.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Issue>()
            .HasOne(i => i.User)
            .WithMany(u => u.Issues)
            .HasForeignKey(i => i.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<IssueImage>()
            .HasOne(i => i.Issue)
            .WithMany(i => i.Images)
            .HasForeignKey(i => i.IssueId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Issue>()
            .HasOne(i => i.Department)
            .WithMany(d => d.AssignedIssues)
            .HasForeignKey(i => i.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Issue>()
            .HasOne(i => i.AssignedStaff)
            .WithMany(u => u.AssignedIssues)
            .HasForeignKey(i => i.AssignedStaffId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<User>()
            .HasOne(u => u.Department)
            .WithMany(d => d.Staff)
            .HasForeignKey(u => u.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
