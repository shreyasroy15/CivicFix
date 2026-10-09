namespace CivicFix.Domain.Entities;

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public ICollection<User> Staff { get; set; } = new List<User>();
    public ICollection<Issue> AssignedIssues { get; set; } = new List<Issue>();
}
