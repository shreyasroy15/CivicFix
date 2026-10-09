using CivicFix.Application.DTOs.Issues;

namespace CivicFix.Application.Interfaces;

public interface IIssueService
{
    Task<IssueResponseDto> CreateIssueAsync(Guid userId, CreateIssueDto request);
    Task<IssueResponseDto> GetIssueByIdAsync(Guid id);
    Task<PagedResult<IssueResponseDto>> GetIssuesAsync(IssueParameters parameters);
    Task<PagedResult<IssueResponseDto>> GetUserIssuesAsync(Guid userId, IssueParameters parameters);
    Task<IssueResponseDto> UpdateIssueAsync(Guid id, Guid userId, UpdateIssueDto request);
    Task<IssueResponseDto> AdminUpdateIssueAsync(Guid id, AdminUpdateIssueDto request);
    Task DeleteIssueAsync(Guid id, Guid userId, bool isAdmin);
}
