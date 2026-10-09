namespace CivicFix.Application.DTOs.Staff;

public class StaffDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
}

public class CreateStaffDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public int? DepartmentId { get; set; }
}

public class UpdateStaffDto
{
    public bool IsActive { get; set; }
    public string? UserName { get; set; }
}

public class AssignDepartmentDto
{
    public int? DepartmentId { get; set; }
}

public class PaginatedStaffDto
{
    public IEnumerable<StaffDto> Items { get; set; } = new List<StaffDto>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
