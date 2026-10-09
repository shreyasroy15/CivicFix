using CivicFix.Application.DTOs.Departments;
using CivicFix.Application.Interfaces;
using CivicFix.Domain.Entities;
using CivicFix.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CivicFix.Infrastructure.Services;

public class DepartmentService : IDepartmentService
{
    private readonly CivicFixDbContext _context;

    public DepartmentService(CivicFixDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DepartmentDto>> GetAllDepartmentsAsync()
    {
        return await _context.Departments
            .Select(d => new DepartmentDto { Id = d.Id, Name = d.Name, Description = d.Description })
            .ToListAsync();
    }

    public async Task<DepartmentDto?> GetDepartmentByIdAsync(int id)
    {
        var d = await _context.Departments.FindAsync(id);
        if (d == null) return null;
        return new DepartmentDto { Id = d.Id, Name = d.Name, Description = d.Description };
    }

    public async Task<DepartmentDto> CreateDepartmentAsync(CreateDepartmentDto request)
    {
        var dept = new Department { Name = request.Name, Description = request.Description };
        _context.Departments.Add(dept);
        await _context.SaveChangesAsync();
        return new DepartmentDto { Id = dept.Id, Name = dept.Name, Description = dept.Description };
    }

    public async Task<DepartmentDto> UpdateDepartmentAsync(int id, UpdateDepartmentDto request)
    {
        var dept = await _context.Departments.FindAsync(id);
        if (dept == null) throw new KeyNotFoundException("Department not found");

        dept.Name = request.Name;
        dept.Description = request.Description;
        await _context.SaveChangesAsync();

        return new DepartmentDto { Id = dept.Id, Name = dept.Name, Description = dept.Description };
    }

    public async Task DeleteDepartmentAsync(int id)
    {
        var dept = await _context.Departments.FindAsync(id);
        if (dept != null)
        {
            _context.Departments.Remove(dept);
            await _context.SaveChangesAsync();
        }
    }
}
