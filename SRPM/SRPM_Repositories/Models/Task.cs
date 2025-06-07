using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SRPM_Repositories.Models;
public class Task
{
    [Key]
    public Guid Id { get; set; }

    [MaxLength(30)]
    public string Code { get; set; } = null!;

    [Required, MaxLength(255)]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? DeliveryDate { get; set; }

    [MaxLength(30)]
    public string? Priority { get; set; }

    public decimal? Progress { get; set; } = 0;
    public int? Overdue { get; set; } = 0;
    public string? Note { get; set; }

    [Required, MaxLength(30)]
    public string Status { get; set; } = "draft";

    [Required]
    public Guid MilestoneId { get; set; }
    public Milestone Milestone { get; set; } = null!;

    [Required]
    public Guid CreateBy { get; set; }
    // Navigation properties
    [ForeignKey("CreateBy")]
    public UserRole CreateByAccount { get; set; } = null!;

    public ICollection<MemberTask>? MemberTasks { get; set; }
    public virtual ICollection<Notification>? Notifications { get; set; }
}
