using CivicFix.Application.DTOs.Departments;
using CivicFix.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CivicFix.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SUPER_ADMIN,DEPARTMENT_ADMIN")]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentsController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    [HttpGet]
    [AllowAnonymous] // Maybe allow anyone to list departments? Or just staff. Let's keep it secure, or allow user.
    public async Task<IActionResult> GetAll()
    {
        var departments = await _departmentService.GetAllDepartmentsAsync();
        return Ok(departments);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var dept = await _departmentService.GetDepartmentByIdAsync(id);
        if (dept == null) return NotFound();
        return Ok(dept);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateDepartmentDto dto)
    {
        var dept = await _departmentService.CreateDepartmentAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = dept.Id }, dept);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateDepartmentDto dto)
    {
        try
        {
            var dept = await _departmentService.UpdateDepartmentAsync(id, dto);
            return Ok(dept);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _departmentService.DeleteDepartmentAsync(id);
        return NoContent();
    }

    [HttpGet("{departmentId}/staff")]
    public async Task<IActionResult> GetStaff(int departmentId, [FromServices] IStaffService staffService)
    {
        var staff = await staffService.GetStaffByDepartmentAsync(departmentId);
        return Ok(staff);
    }
}
