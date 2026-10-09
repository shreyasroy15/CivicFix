using CivicFix.Application.DTOs.Staff;
using CivicFix.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CivicFix.API.Controllers.Admin;

[ApiController]
[Route("api/admin/staff")]
[Authorize(Roles = "SUPER_ADMIN,DEPARTMENT_ADMIN")]
public class StaffController : ControllerBase
{
    private readonly IStaffService _staffService;

    public StaffController(IStaffService staffService)
    {
        _staffService = staffService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
    {
        var staff = await _staffService.GetStaffAsync(pageNumber, pageSize, search);
        return Ok(staff);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var staff = await _staffService.GetStaffByIdAsync(id);
        if (staff == null) return NotFound();
        return Ok(staff);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateStaffDto dto)
    {
        try
        {
            var staff = await _staffService.CreateStaffAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = staff.Id }, staff);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateStaffDto dto)
    {
        try
        {
            var staff = await _staffService.UpdateStaffAsync(id, dto);
            return Ok(staff);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }

    [HttpPut("{id}/department")]
    public async Task<IActionResult> AssignDepartment(Guid id, AssignDepartmentDto dto)
    {
        try
        {
            var staff = await _staffService.AssignDepartmentAsync(id, dto);
            return Ok(staff);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}
