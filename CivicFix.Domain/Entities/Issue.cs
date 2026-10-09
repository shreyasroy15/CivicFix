namespace CivicFix.Domain.Entities;

public class Issue
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "PENDING"; // PENDING, VERIFIED, ASSIGNED, IN_PROGRESS, RESOLVED, REJECTED, CLOSED
    public string Priority { get; set; } = "MEDIUM"; // LOW, MEDIUM, HIGH, CRITICAL

    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Address { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }

    public Guid? AssignedStaffId { get; set; }
    public User? AssignedStaff { get; set; }

    public string? AdminNote { get; set; }
    public string? RejectionReason { get; set; }

    public ICollection<IssueImage> Images { get; set; } = new List<IssueImage>();
}
