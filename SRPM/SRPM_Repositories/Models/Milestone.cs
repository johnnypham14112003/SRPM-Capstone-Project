using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;

public class Milestone
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(30)] public string Code { get; set; } = null!;
    [MaxLength(255)] public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? Objective { get; set; }
    [Required] public decimal Cost { get; set; } = 0m; //suffix of specific assign data type
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    [Required] public string Type { get; set; } = "normal";// meeting
    [Required] public DateTime CreatedAt { get; set; } = DateTime.Now;
    [Required, MaxLength(30)] public string Status { get; set; } = "created";

    // Foreign keys
    [Required] public Guid ProjectId { get; set; }
    [Required] public Guid CreatorId { get; set; }

    // Navigation properties
    public Project Project { get; set; } = null!;
    public UserRole Creator { get; set; } = null!;
    public ICollection<Evaluation>? Evaluations { get; set; }
    public ICollection<Task>? Tasks { get; set; }
}