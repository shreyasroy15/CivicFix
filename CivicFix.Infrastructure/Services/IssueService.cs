using CivicFix.Application.DTOs.Issues;
using CivicFix.Application.Interfaces;
using CivicFix.Domain.Entities;
using CivicFix.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CivicFix.Infrastructure.Services;

public class IssueService : IIssueService
{
    private readonly CivicFixDbContext _context;

    public IssueService(CivicFixDbContext context)
    {
        _context = context;
    }

    public async Task<IssueResponseDto> CreateIssueAsync(Guid userId, CreateIssueDto request)
    {
        var category = await _context.Categories.FindAsync(request.CategoryId);
        if (category == null) throw new ArgumentException("Invalid category ID");

        var issue = new Issue
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            CategoryId = request.CategoryId,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Address = request.Address,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            Status = "PENDING",
            Priority = "MEDIUM"
        };

        if (request.ImageUrls != null && request.ImageUrls.Any())
        {
            foreach (var url in request.ImageUrls)
            {
                issue.Images.Add(new IssueImage { Id = Guid.NewGuid(), Url = url });
            }
        }

        _context.Issues.Add(issue);
        await _context.SaveChangesAsync();

        return await GetIssueByIdAsync(issue.Id);
    }

    public async Task<IssueResponseDto> GetIssueByIdAsync(Guid id)
    {
        var issue = await _context.Issues
            .Include(i => i.Category)
            .Include(i => i.User)
            .Include(i => i.Images)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (issue == null) throw new KeyNotFoundException("Issue not found");

        return MapToDto(issue);
    }

    public async Task<PagedResult<IssueResponseDto>> GetIssuesAsync(IssueParameters parameters)
    {
        return await GetPagedIssuesAsync(_context.Issues, parameters);
    }

    public async Task<PagedResult<IssueResponseDto>> GetUserIssuesAsync(Guid userId, IssueParameters parameters)
    {
        var query = _context.Issues.Where(i => i.UserId == userId);
        return await GetPagedIssuesAsync(query, parameters);
    }

    public async Task<IssueResponseDto> UpdateIssueAsync(Guid id, Guid userId, UpdateIssueDto request)
    {
        var issue = await _context.Issues.Include(i => i.Category).Include(i => i.User).Include(i => i.Images).FirstOrDefaultAsync(i => i.Id == id);
        if (issue == null) throw new KeyNotFoundException("Issue not found");
        if (issue.UserId != userId) throw new UnauthorizedAccessException("You can only update your own issues");

        if (request.Title != null) issue.Title = request.Title;
        if (request.Description != null) issue.Description = request.Description;
        if (request.CategoryId.HasValue) issue.CategoryId = request.CategoryId.Value;
        
        issue.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return MapToDto(issue);
    }

    public async Task<IssueResponseDto> AdminUpdateIssueAsync(Guid id, AdminUpdateIssueDto request)
    {
        var issue = await _context.Issues.Include(i => i.Category).Include(i => i.User).Include(i => i.Images).FirstOrDefaultAsync(i => i.Id == id);
        if (issue == null) throw new KeyNotFoundException("Issue not found");

        if (request.Status != null) issue.Status = request.Status;
        if (request.Priority != null) issue.Priority = request.Priority;
        
        issue.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return MapToDto(issue);
    }

    public async Task DeleteIssueAsync(Guid id, Guid userId, bool isAdmin)
    {
        var issue = await _context.Issues.FindAsync(id);
        if (issue == null) throw new KeyNotFoundException("Issue not found");

        if (!isAdmin && issue.UserId != userId) throw new UnauthorizedAccessException("You can only delete your own issues");

        _context.Issues.Remove(issue);
        await _context.SaveChangesAsync();
    }

    private async Task<PagedResult<IssueResponseDto>> GetPagedIssuesAsync(IQueryable<Issue> query, IssueParameters parameters)
    {
        if (!string.IsNullOrEmpty(parameters.Status))
        {
            query = query.Where(i => i.Status == parameters.Status);
        }

        if (parameters.CategoryId.HasValue)
        {
            query = query.Where(i => i.CategoryId == parameters.CategoryId.Value);
        }

        if (!string.IsNullOrEmpty(parameters.SearchTerm))
        {
            query = query.Where(i => i.Title.Contains(parameters.SearchTerm) || i.Description.Contains(parameters.SearchTerm));
        }

        var totalCount = await query.CountAsync();

        var issues = await query
            .Include(i => i.Category)
            .Include(i => i.User)
            .Include(i => i.Images)
            .OrderByDescending(i => i.CreatedAt)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        var dtoList = issues.Select(MapToDto).ToList();

        return new PagedResult<IssueResponseDto>
        {
            Items = dtoList,
            TotalCount = totalCount,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        };
    }

    private static IssueResponseDto MapToDto(Issue issue)
    {
        return new IssueResponseDto
        {
            Id = issue.Id,
            Title = issue.Title,
            Description = issue.Description,
            Status = issue.Status,
            Priority = issue.Priority,
            Latitude = issue.Latitude,
            Longitude = issue.Longitude,
            Address = issue.Address,
            CreatedAt = issue.CreatedAt,
            UpdatedAt = issue.UpdatedAt,
            UserId = issue.UserId,
            UserName = issue.User?.UserName ?? string.Empty,
            CategoryId = issue.CategoryId,
            CategoryName = issue.Category?.Name ?? string.Empty,
            ImageUrls = issue.Images.Select(img => img.Url).ToList()
        };
    }
}
