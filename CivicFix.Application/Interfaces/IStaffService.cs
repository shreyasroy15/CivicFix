using CivicFix.Application.DTOs.Staff;

namespace CivicFix.Application.Interfaces;

public interface IStaffService
{
    Task<PaginatedStaffDto> GetStaffAsync(int pageNumber, int pageSize, string? search);
    Task<StaffDto?> GetStaffByIdAsync(Guid id);
    Task<StaffDto> CreateStaffAsync(CreateStaffDto request);
    Task<StaffDto> UpdateStaffAsync(Guid id, UpdateStaffDto request);
    Task<StaffDto> AssignDepartmentAsync(Guid id, AssignDepartmentDto request);
    Task<IEnumerable<StaffDto>> GetStaffByDepartmentAsync(int departmentId);
}
