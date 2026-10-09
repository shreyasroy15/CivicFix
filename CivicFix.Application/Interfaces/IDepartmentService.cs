using CivicFix.Application.DTOs.Departments;

namespace CivicFix.Application.Interfaces;

public interface IDepartmentService
{
    Task<IEnumerable<DepartmentDto>> GetAllDepartmentsAsync();
    Task<DepartmentDto?> GetDepartmentByIdAsync(int id);
    Task<DepartmentDto> CreateDepartmentAsync(CreateDepartmentDto request);
    Task<DepartmentDto> UpdateDepartmentAsync(int id, UpdateDepartmentDto request);
    Task DeleteDepartmentAsync(int id);
}
