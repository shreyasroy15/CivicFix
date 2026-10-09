namespace CivicFix.Domain.Entities;

using Microsoft.AspNetCore.Identity;

public class User : IdentityUser<Guid>
{
    // IdentityUser already has Email, UserName, PasswordHash
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    
    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Issue> Issues { get; set; } = new List<Issue>();
    public ICollection<Issue> AssignedIssues { get; set; } = new List<Issue>();
}

public class Role : IdentityRole<Guid>
{
    public const string User = "USER";
    public const string Staff = "STAFF";
    public const string DepartmentAdmin = "DEPARTMENT_ADMIN";
    public const string SuperAdmin = "SUPER_ADMIN";
}
