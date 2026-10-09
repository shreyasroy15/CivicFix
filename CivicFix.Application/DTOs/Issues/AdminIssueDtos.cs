namespace CivicFix.Application.DTOs.Issues;

public class AdminIssueFilterDto
{
    public string? Search { get; set; }
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public int? CategoryId { get; set; }
    public int? DepartmentId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class RejectIssueDto
{
    public string Reason { get; set; } = string.Empty;
}

public class UpdatePriorityDto
{
    public string Priority { get; set; } = string.Empty;
}

public class AssignIssueDto
{
    public int DepartmentId { get; set; }
    public Guid? AssignedStaffId { get; set; }
}

public class UpdateStatusDto
{
    public string Status { get; set; } = string.Empty;
}

public class AddNoteDto
{
    public string Note { get; set; } = string.Empty;
}

public class AdminIssueDto : IssueResponseDto
{
    public int? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public Guid? AssignedStaffId { get; set; }
    public string? AssignedStaffName { get; set; }
    public string? AdminNote { get; set; }
    public string? RejectionReason { get; set; }
}

public class AuditLogDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Details { get; set; } = string.Empty;
}

public class DashboardStatsDto
{
    public int TotalIssues { get; set; }
    public int PendingIssues { get; set; }
    public int VerifiedIssues { get; set; }
    public int AssignedIssues { get; set; }
    public int InProgressIssues { get; set; }
    public int ResolvedIssues { get; set; }
    public int RejectedIssues { get; set; }
    public int CriticalIssues { get; set; }
    
    public int IssuesLast7Days { get; set; }
    public int IssuesLast30Days { get; set; }
    public double AverageResolutionTimeHours { get; set; }

    public Dictionary<string, int> IssuesByCategory { get; set; } = new();
    public Dictionary<string, int> IssuesByStatus { get; set; } = new();
    
    public List<DailyIssueCount> DailyIssuesLast7Days { get; set; } = new();
    public List<AdminIssueDto> RecentIssues { get; set; } = new();
}

public class DailyIssueCount
{
    public string Date { get; set; } = string.Empty;
    public int Count { get; set; }
}
