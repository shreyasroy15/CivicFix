using CivicFix.Application.DTOs.Issues;
using CivicFix.Application.Interfaces;
using CivicFix.Domain.Entities;
using CivicFix.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CivicFix.Infrastructure.Services;

public class AdminIssueService : IAdminIssueService
{
    private readonly CivicFixDbContext _context;

    public AdminIssueService(CivicFixDbContext context)
    {
        _context = context;
    }

    private async Task LogActionAsync(Guid issueId, Guid userId, string action, object? details = null)
    {
        var log = new AuditLog
        {
            Id = Guid.NewGuid(),
            EntityId = issueId.ToString(),
            EntityName = "Issue",
            UserId = userId,
            Action = action,
            Timestamp = DateTime.UtcNow,
            Details = details != null ? JsonSerializer.Serialize(details) : "{}"
        };
        _context.AuditLogs.Add(log);
    }

    public async Task<PagedResult<AdminIssueDto>> GetAdminIssuesAsync(AdminIssueFilterDto filter)
    {
        var query = _context.Issues
            .Include(i => i.Category)
            .Include(i => i.Images)
            .Include(i => i.Department)
            .Include(i => i.AssignedStaff)
            .Include(i => i.User)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            query = query.Where(i => i.Title.Contains(filter.Search) || i.Description.Contains(filter.Search) || i.Id.ToString() == filter.Search);
        }
        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            query = query.Where(i => i.Status == filter.Status);
        }
        if (!string.IsNullOrWhiteSpace(filter.Priority))
        {
            query = query.Where(i => i.Priority == filter.Priority);
        }
        if (filter.CategoryId.HasValue)
        {
            query = query.Where(i => i.CategoryId == filter.CategoryId.Value);
        }
        if (filter.DepartmentId.HasValue)
        {
            query = query.Where(i => i.DepartmentId == filter.DepartmentId.Value);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(i => i.CreatedAt)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(i => MapToAdminDto(i))
            .ToListAsync();

        return new PagedResult<AdminIssueDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<AdminIssueDto?> GetAdminIssueByIdAsync(Guid id)
    {
        var issue = await _context.Issues
            .Include(i => i.Category)
            .Include(i => i.Images)
            .Include(i => i.Department)
            .Include(i => i.AssignedStaff)
            .Include(i => i.User)
            .FirstOrDefaultAsync(i => i.Id == id);
            
        if (issue == null) return null;
        return MapToAdminDto(issue);
    }

    public async Task VerifyIssueAsync(Guid issueId, Guid adminId)
    {
        var issue = await _context.Issues.FindAsync(issueId);
        if (issue == null) throw new KeyNotFoundException("Issue not found");
        
        if (issue.Status != "PENDING") throw new InvalidOperationException("Only PENDING issues can be verified");

        issue.Status = "VERIFIED";
        issue.UpdatedAt = DateTime.UtcNow;
        
        await LogActionAsync(issueId, adminId, "Verified");
        await _context.SaveChangesAsync();
    }

    public async Task RejectIssueAsync(Guid issueId, RejectIssueDto request, Guid adminId)
    {
        if (string.IsNullOrWhiteSpace(request.Reason)) throw new ArgumentException("Rejection reason is required");
        
        var issue = await _context.Issues.FindAsync(issueId);
        if (issue == null) throw new KeyNotFoundException("Issue not found");
        
        if (issue.Status != "PENDING" && issue.Status != "VERIFIED") throw new InvalidOperationException("Issue cannot be rejected from its current status");

        issue.Status = "REJECTED";
        issue.RejectionReason = request.Reason;
        issue.UpdatedAt = DateTime.UtcNow;
        
        await LogActionAsync(issueId, adminId, "Rejected", new { Reason = request.Reason });
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePriorityAsync(Guid issueId, UpdatePriorityDto request, Guid adminId)
    {
        var issue = await _context.Issues.FindAsync(issueId);
        if (issue == null) throw new KeyNotFoundException("Issue not found");

        var oldPriority = issue.Priority;
        issue.Priority = request.Priority;
        issue.UpdatedAt = DateTime.UtcNow;
        
        await LogActionAsync(issueId, adminId, "PriorityChanged", new { Old = oldPriority, New = request.Priority });
        await _context.SaveChangesAsync();
    }

    public async Task AssignIssueAsync(Guid issueId, AssignIssueDto request, Guid adminId)
    {
        var issue = await _context.Issues.FindAsync(issueId);
        if (issue == null) throw new KeyNotFoundException("Issue not found");
        
        var dept = await _context.Departments.FindAsync(request.DepartmentId);
        if (dept == null) throw new ArgumentException("Department not found");

        if (request.AssignedStaffId.HasValue)
        {
            var staff = await _context.Users.FindAsync(request.AssignedStaffId.Value);
            if (staff == null) throw new ArgumentException("Staff not found");
            if (staff.DepartmentId != request.DepartmentId) throw new ArgumentException("Staff does not belong to the specified department");
            if (!staff.IsActive) throw new ArgumentException("Staff is not active");
        }

        issue.DepartmentId = request.DepartmentId;
        issue.AssignedStaffId = request.AssignedStaffId;
        if (issue.Status == "VERIFIED" || issue.Status == "PENDING") 
        {
            issue.Status = "ASSIGNED";
        }
        issue.UpdatedAt = DateTime.UtcNow;
        
        await LogActionAsync(issueId, adminId, "Assigned", new { DepartmentId = request.DepartmentId, StaffId = request.AssignedStaffId });
        await _context.SaveChangesAsync();
    }

    public async Task UpdateStatusAsync(Guid issueId, UpdateStatusDto request, Guid adminId)
    {
        var issue = await _context.Issues.FindAsync(issueId);
        if (issue == null) throw new KeyNotFoundException("Issue not found");

        var oldStatus = issue.Status;
        issue.Status = request.Status;
        issue.UpdatedAt = DateTime.UtcNow;
        
        await LogActionAsync(issueId, adminId, "StatusChanged", new { Old = oldStatus, New = request.Status });
        await _context.SaveChangesAsync();
    }

    public async Task AddNoteAsync(Guid issueId, AddNoteDto request, Guid adminId)
    {
        var issue = await _context.Issues.FindAsync(issueId);
        if (issue == null) throw new KeyNotFoundException("Issue not found");

        issue.AdminNote = request.Note;
        issue.UpdatedAt = DateTime.UtcNow;
        
        await LogActionAsync(issueId, adminId, "NoteAdded");
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<AuditLogDto>> GetIssueHistoryAsync(Guid issueId)
    {
        var logs = await _context.AuditLogs
            .Where(a => a.EntityId == issueId.ToString() && a.EntityName == "Issue")
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();

        return logs.Select(l => new AuditLogDto
        {
            Id = l.Id,
            UserId = l.UserId,
            Action = l.Action,
            EntityName = l.EntityName,
            EntityId = l.EntityId,
            Timestamp = l.Timestamp,
            Details = l.Details
        });
    }

    private static AdminIssueDto MapToAdminDto(Issue i)
    {
        return new AdminIssueDto
        {
            Id = i.Id,
            Title = i.Title,
            Description = i.Description,
            Status = i.Status,
            Priority = i.Priority,
            Latitude = i.Latitude,
            Longitude = i.Longitude,
            Address = i.Address,
            CreatedAt = i.CreatedAt,
            UpdatedAt = i.UpdatedAt,
            UserId = i.UserId,
            UserName = i.User?.UserName ?? "Unknown",
            CategoryId = i.CategoryId,
            CategoryName = i.Category?.Name ?? "Unknown",
            ImageUrls = i.Images.Select(img => img.Url).ToList(),
            DepartmentId = i.DepartmentId,
            DepartmentName = i.Department?.Name,
            AssignedStaffId = i.AssignedStaffId,
            AssignedStaffName = i.AssignedStaff?.UserName,
            AdminNote = i.AdminNote,
            RejectionReason = i.RejectionReason
        };
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var sevenDaysAgo = now.AddDays(-7);
        var thirtyDaysAgo = now.AddDays(-30);

        var statuses = await _context.Issues
            .GroupBy(i => i.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(k => k.Status, v => v.Count, cancellationToken);

        var categories = await _context.Issues
            .Include(i => i.Category)
            .GroupBy(i => i.Category!.Name)
            .Select(g => new { Category = g.Key, Count = g.Count() })
            .ToDictionaryAsync(k => k.Category, v => v.Count, cancellationToken);

        var totalIssues = statuses.Values.Sum();
        var criticalIssues = await _context.Issues.CountAsync(i => i.Priority == "CRITICAL", cancellationToken);
        var last7Days = await _context.Issues.CountAsync(i => i.CreatedAt >= sevenDaysAgo, cancellationToken);
        var last30Days = await _context.Issues.CountAsync(i => i.CreatedAt >= thirtyDaysAgo, cancellationToken);

        var resolvedIssues = await _context.Issues
            .Where(i => i.Status == "RESOLVED" && i.UpdatedAt != null)
            .Select(i => new { i.CreatedAt, i.UpdatedAt })
            .ToListAsync(cancellationToken);

        double avgResolutionTime = 0;
        if (resolvedIssues.Count > 0)
        {
            avgResolutionTime = resolvedIssues
                .Average(i => (i.UpdatedAt!.Value - i.CreatedAt).TotalHours);
        }

        var dailyCounts = await _context.Issues
            .Where(i => i.CreatedAt >= sevenDaysAgo)
            .Select(i => new { i.CreatedAt.Date })
            .GroupBy(i => i.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        // Fill in missing dates for the last 7 days
        var dailyIssuesList = new List<DailyIssueCount>();
        for (int i = 6; i >= 0; i--)
        {
            var date = now.AddDays(-i).Date;
            var match = dailyCounts.FirstOrDefault(d => d.Date == date);
            dailyIssuesList.Add(new DailyIssueCount
            {
                Date = date.ToString("yyyy-MM-dd"),
                Count = match?.Count ?? 0
            });
        }

        var recentIssues = await _context.Issues
            .Include(i => i.Category)
            .Include(i => i.Images)
            .Include(i => i.Department)
            .Include(i => i.AssignedStaff)
            .Include(i => i.User)
            .OrderByDescending(i => i.CreatedAt)
            .Take(5)
            .Select(i => MapToAdminDto(i))
            .ToListAsync(cancellationToken);

        return new DashboardStatsDto
        {
            TotalIssues = totalIssues,
            PendingIssues = statuses.GetValueOrDefault("PENDING", 0),
            VerifiedIssues = statuses.GetValueOrDefault("VERIFIED", 0),
            AssignedIssues = statuses.GetValueOrDefault("ASSIGNED", 0),
            InProgressIssues = statuses.GetValueOrDefault("IN_PROGRESS", 0),
            ResolvedIssues = statuses.GetValueOrDefault("RESOLVED", 0),
            RejectedIssues = statuses.GetValueOrDefault("REJECTED", 0),
            CriticalIssues = criticalIssues,
            IssuesLast7Days = last7Days,
            IssuesLast30Days = last30Days,
            AverageResolutionTimeHours = Math.Round(avgResolutionTime, 1),
            IssuesByCategory = categories,
            IssuesByStatus = statuses,
            DailyIssuesLast7Days = dailyIssuesList,
            RecentIssues = recentIssues
        };
    }
}
