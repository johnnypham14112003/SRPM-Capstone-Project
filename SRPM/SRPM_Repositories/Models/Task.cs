using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;

public class Task
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(30)] public string Code { get; set; } = null!;
    [Required] public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Objective { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    [MaxLength(30)] public string? Priority { get; set; }
    public decimal Cost { get; set; } = 0m;
    [Required] public decimal Progress { get; set; } = 0m;//100.00
    [Required] public int Overdue { get; set; } = 0;
    public string? MeetingUrl { get; set; }
    public string? Note { get; set; }
    [Required, MaxLength(30)] public string Status { get; set; } = "created";

    // Foreign keys
    [Required] public Guid MilestoneId { get; set; }
    [Required] public Guid CreatorId { get; set; }

    // Navigation properties
    public Milestone Milestone { get; set; } = null!;
    public UserRole Creator { get; set; } = null!;
    public ICollection<MemberTask>? MemberTasks { get; set; }
    public virtual ICollection<Notification>? Notifications { get; set; }
}
