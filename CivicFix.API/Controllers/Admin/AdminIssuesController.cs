using CivicFix.Application.DTOs.Issues;
using CivicFix.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CivicFix.API.Controllers.Admin;

[ApiController]
[Route("api/admin/issues")]
[Authorize(Roles = "SUPER_ADMIN,DEPARTMENT_ADMIN,STAFF")]
public class AdminIssuesController : ControllerBase
{
    private readonly IAdminIssueService _adminIssueService;

    public AdminIssuesController(IAdminIssueService adminIssueService)
    {
        _adminIssueService = adminIssueService;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] AdminIssueFilterDto filter)
    {
        var result = await _adminIssueService.GetAdminIssuesAsync(filter);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var issue = await _adminIssueService.GetAdminIssueByIdAsync(id);
        if (issue == null) return NotFound();
        return Ok(issue);
    }

    [HttpPut("{id}/verify")]
    public async Task<IActionResult> Verify(Guid id)
    {
        try
        {
            await _adminIssueService.VerifyIssueAsync(id, GetUserId());
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }

    [HttpPut("{id}/reject")]
    public async Task<IActionResult> Reject(Guid id, RejectIssueDto dto)
    {
        try
        {
            await _adminIssueService.RejectIssueAsync(id, dto, GetUserId());
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }

    [HttpPut("{id}/priority")]
    public async Task<IActionResult> UpdatePriority(Guid id, UpdatePriorityDto dto)
    {
        try
        {
            await _adminIssueService.UpdatePriorityAsync(id, dto, GetUserId());
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }

    [HttpPut("{id}/assign")]
    public async Task<IActionResult> Assign(Guid id, AssignIssueDto dto)
    {
        try
        {
            await _adminIssueService.AssignIssueAsync(id, dto, GetUserId());
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, UpdateStatusDto dto)
    {
        try
        {
            await _adminIssueService.UpdateStatusAsync(id, dto, GetUserId());
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }

    [HttpPost("{id}/notes")]
    public async Task<IActionResult> AddNote(Guid id, AddNoteDto dto)
    {
        try
        {
            await _adminIssueService.AddNoteAsync(id, dto, GetUserId());
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }

    [HttpGet("{id}/history")]
    public async Task<IActionResult> GetHistory(Guid id)
    {
        var history = await _adminIssueService.GetIssueHistoryAsync(id);
        return Ok(history);
    }

    [HttpGet("../dashboard")]
    public async Task<IActionResult> GetDashboard(CancellationToken cancellationToken)
    {
        var stats = await _adminIssueService.GetDashboardStatsAsync(cancellationToken);
        return Ok(stats);
    }
}
