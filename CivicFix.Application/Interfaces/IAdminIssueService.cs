using CivicFix.Application.DTOs.Issues;

namespace CivicFix.Application.Interfaces;

public interface IAdminIssueService
{
    Task<PagedResult<AdminIssueDto>> GetAdminIssuesAsync(AdminIssueFilterDto filter);
    Task<AdminIssueDto?> GetAdminIssueByIdAsync(Guid id);
    Task VerifyIssueAsync(Guid issueId, Guid adminId);
    Task RejectIssueAsync(Guid issueId, RejectIssueDto request, Guid adminId);
    Task UpdatePriorityAsync(Guid issueId, UpdatePriorityDto request, Guid adminId);
    Task AssignIssueAsync(Guid issueId, AssignIssueDto request, Guid adminId);
    Task UpdateStatusAsync(Guid issueId, UpdateStatusDto request, Guid adminId);
    Task AddNoteAsync(Guid issueId, AddNoteDto request, Guid adminId);
    Task<IEnumerable<AuditLogDto>> GetIssueHistoryAsync(Guid issueId);
}
