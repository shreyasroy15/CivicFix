using System.ComponentModel.DataAnnotations;

namespace CivicFix.Application.DTOs.Issues;

public class CreateIssueDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public int CategoryId { get; set; }

    [Required]
    public double Latitude { get; set; }

    [Required]
    public double Longitude { get; set; }

    [Required]
    [MaxLength(500)]
    public string Address { get; set; } = string.Empty;

    public List<string>? ImageUrls { get; set; }
}

public class UpdateIssueDto
{
    [MaxLength(200)]
    public string? Title { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }

    public int? CategoryId { get; set; }
    
    // Status can only be updated by admins/staff usually, but keeping it here for simplicity. 
    // Wait, let's keep status out of the normal Update API for users. Or create a separate AdminUpdateIssueDto.
}

public class AdminUpdateIssueDto
{
    public string? Status { get; set; }
    public string? Priority { get; set; }
}
