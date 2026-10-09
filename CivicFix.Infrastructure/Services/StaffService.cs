using CivicFix.Application.DTOs.Staff;
using CivicFix.Application.Interfaces;
using CivicFix.Domain.Entities;
using CivicFix.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CivicFix.Infrastructure.Services;

public class StaffService : IStaffService
{
    private readonly UserManager<User> _userManager;
    private readonly CivicFixDbContext _context;

    public StaffService(UserManager<User> userManager, CivicFixDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<PaginatedStaffDto> GetStaffAsync(int pageNumber, int pageSize, string? search)
    {
        var staffUsers = await _userManager.GetUsersInRoleAsync(Role.Staff);
        
        var query = _context.Users
            .Include(u => u.Department)
            .Where(u => staffUsers.Select(s => s.Id).Contains(u.Id))
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(u => u.Email!.Contains(search) || u.UserName!.Contains(search));
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(u => u.UserName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new StaffDto
            {
                Id = u.Id,
                Email = u.Email!,
                UserName = u.UserName!,
                IsActive = u.IsActive,
                DepartmentId = u.DepartmentId,
                DepartmentName = u.Department != null ? u.Department.Name : null
            })
            .ToListAsync();

        return new PaginatedStaffDto
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<StaffDto?> GetStaffByIdAsync(Guid id)
    {
        var user = await _context.Users
            .Include(u => u.Department)
            .FirstOrDefaultAsync(u => u.Id == id);
            
        if (user == null) return null;
        
        var isStaff = await _userManager.IsInRoleAsync(user, Role.Staff);
        if (!isStaff) return null;

        return new StaffDto
        {
            Id = user.Id,
            Email = user.Email!,
            UserName = user.UserName!,
            IsActive = user.IsActive,
            DepartmentId = user.DepartmentId,
            DepartmentName = user.Department?.Name
        };
    }

    public async Task<StaffDto> CreateStaffAsync(CreateStaffDto request)
    {
        if (request.DepartmentId.HasValue)
        {
            var deptExists = await _context.Departments.AnyAsync(d => d.Id == request.DepartmentId.Value);
            if (!deptExists) throw new ArgumentException("Invalid department ID.");
        }

        // Check if user already exists
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            // Promote existing user to STAFF if they aren't already
            if (!await _userManager.IsInRoleAsync(existingUser, Role.Staff))
            {
                await _userManager.AddToRoleAsync(existingUser, Role.Staff);
            }
            
            existingUser.DepartmentId = request.DepartmentId;
            await _userManager.UpdateAsync(existingUser);
            
            var dept = existingUser.DepartmentId.HasValue ? await _context.Departments.FindAsync(existingUser.DepartmentId.Value) : null;
            return new StaffDto
            {
                Id = existingUser.Id,
                Email = existingUser.Email!,
                UserName = existingUser.UserName!,
                IsActive = existingUser.IsActive,
                DepartmentId = existingUser.DepartmentId,
                DepartmentName = dept?.Name
            };
        }

        var user = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            DepartmentId = request.DepartmentId,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        await _userManager.AddToRoleAsync(user, Role.Staff);

        var department = user.DepartmentId.HasValue ? await _context.Departments.FindAsync(user.DepartmentId.Value) : null;
        return new StaffDto
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            IsActive = user.IsActive,
            DepartmentId = user.DepartmentId,
            DepartmentName = department?.Name
        };
    }

    public async Task<StaffDto> UpdateStaffAsync(Guid id, UpdateStaffDto request)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) throw new KeyNotFoundException("Staff not found");

        var isStaff = await _userManager.IsInRoleAsync(user, Role.Staff);
        if (!isStaff) throw new KeyNotFoundException("Staff not found");

        user.IsActive = request.IsActive;
        if (!string.IsNullOrWhiteSpace(request.UserName))
        {
            user.UserName = request.UserName;
        }

        await _userManager.UpdateAsync(user);

        var dept = user.DepartmentId.HasValue ? await _context.Departments.FindAsync(user.DepartmentId.Value) : null;
        return new StaffDto
        {
            Id = user.Id,
            Email = user.Email!,
            UserName = user.UserName!,
            IsActive = user.IsActive,
            DepartmentId = user.DepartmentId,
            DepartmentName = dept?.Name
        };
    }

    public async Task<StaffDto> AssignDepartmentAsync(Guid id, AssignDepartmentDto request)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) throw new KeyNotFoundException("Staff not found");

        var isStaff = await _userManager.IsInRoleAsync(user, Role.Staff);
        if (!isStaff) throw new KeyNotFoundException("Staff not found");

        if (request.DepartmentId.HasValue)
        {
            var deptExists = await _context.Departments.AnyAsync(d => d.Id == request.DepartmentId.Value);
            if (!deptExists) throw new ArgumentException("Invalid department ID.");
        }

        user.DepartmentId = request.DepartmentId;
        await _userManager.UpdateAsync(user);

        var dept = user.DepartmentId.HasValue ? await _context.Departments.FindAsync(user.DepartmentId.Value) : null;
        return new StaffDto
        {
            Id = user.Id,
            Email = user.Email!,
            UserName = user.UserName!,
            IsActive = user.IsActive,
            DepartmentId = user.DepartmentId,
            DepartmentName = dept?.Name
        };
    }

    public async Task<IEnumerable<StaffDto>> GetStaffByDepartmentAsync(int departmentId)
    {
        var staffUsers = await _userManager.GetUsersInRoleAsync(Role.Staff);
        
        var query = _context.Users
            .Include(u => u.Department)
            .Where(u => u.DepartmentId == departmentId)
            .Where(u => staffUsers.Select(s => s.Id).Contains(u.Id))
            .AsQueryable();

        return await query
            .OrderBy(u => u.UserName)
            .Select(u => new StaffDto
            {
                Id = u.Id,
                Email = u.Email!,
                UserName = u.UserName!,
                IsActive = u.IsActive,
                DepartmentId = u.DepartmentId,
                DepartmentName = u.Department!.Name
            })
            .ToListAsync();
    }
}
