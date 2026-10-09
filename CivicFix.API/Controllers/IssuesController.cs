using System.Security.Claims;
using CivicFix.Application.DTOs.Issues;
using CivicFix.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CivicFix.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class IssuesController : ControllerBase
{
    private readonly IIssueService _issueService;

    public IssuesController(IIssueService issueService)
    {
        _issueService = issueService;
    }

    private Guid GetCurrentUserId()
    {
        var idClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        return idClaim != null ? Guid.Parse(idClaim) : Guid.Empty;
    }

    [HttpPost]
    public async Task<IActionResult> CreateIssue([FromBody] CreateIssueDto request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var userId = GetCurrentUserId();
            var issue = await _issueService.CreateIssueAsync(userId, request);
            return CreatedAtAction(nameof(GetIssue), new { id = issue.Id }, issue);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetIssue(Guid id)
    {
        try
        {
            var issue = await _issueService.GetIssueByIdAsync(id);
            return Ok(issue);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetIssues([FromQuery] IssueParameters parameters)
    {
        var pagedResult = await _issueService.GetIssuesAsync(parameters);
        return Ok(pagedResult);
    }

    [HttpGet("my-issues")]
    public async Task<IActionResult> GetMyIssues([FromQuery] IssueParameters parameters)
    {
        var userId = GetCurrentUserId();
        var pagedResult = await _issueService.GetUserIssuesAsync(userId, parameters);
        return Ok(pagedResult);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateIssue(Guid id, [FromBody] UpdateIssueDto request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var issue = await _issueService.UpdateIssueAsync(id, userId, request);
            return Ok(issue);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpPut("{id}/admin")]
    [Authorize(Roles = "SUPER_ADMIN,DEPARTMENT_ADMIN,STAFF")]
    public async Task<IActionResult> AdminUpdateIssue(Guid id, [FromBody] AdminUpdateIssueDto request)
    {
        try
        {
            var issue = await _issueService.AdminUpdateIssueAsync(id, request);
            return Ok(issue);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteIssue(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var isAdmin = User.IsInRole("SUPER_ADMIN");
            await _issueService.DeleteIssueAsync(id, userId, isAdmin);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
