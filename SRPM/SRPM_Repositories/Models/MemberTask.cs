using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;

public class MemberTask
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    [Required] public decimal Progress { get; set; } = 0m;//100.00
    [Required] public int Overdue { get; set; } = 0;
    public string? Note { get; set; }
    public DateTime? DeliveryDate { get; set; }
    [Required] public DateTime JoinedAt { get; set; } = DateTime.Now;
    [Required, MaxLength(30)] public string Status { get; set; } = "created";

    // Foreign keys
    [Required] public Guid MemberId { get; set; }
    [Required] public Guid TaskId { get; set; }

    // Navigation properties
    public virtual UserRole Member { get; set; } = null!;
    public virtual Task Task { get; set; } = null!;
    public virtual ICollection<Notification>? Notifications { get; set; }
}