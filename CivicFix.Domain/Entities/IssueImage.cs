namespace CivicFix.Domain.Entities;

public class IssueImage
{
    public Guid Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string PublicId { get; set; } = string.Empty;
    
    public Guid IssueId { get; set; }
    public Issue Issue { get; set; } = null!;
}
