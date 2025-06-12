using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SRPM_Repositories.Models;
public class MemberTask
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid MemberId { get; set; }

    [Required]
    public Guid TaskId { get; set; }

    [Required]
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public DateTime? DeliveryDate { get; set; }

    [Required]
    [Column(TypeName = "decimal(5,2)")]
    public decimal Progress { get; set; } = 0;

    public int? Overdue { get; set; } = 0;

    public string? Note { get; set; }

    [Required]
    [MaxLength(30)]
    public string Status { get; set; } = "draft";

    // Navigation properties
    public virtual UserRole Member { get; set; } = null!;
    public virtual Task Task { get; set; } = null!;
    public virtual ICollection<Notification>? Notifications { get; set; }
}