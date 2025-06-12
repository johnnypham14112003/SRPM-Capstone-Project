using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;

public class Signature
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    public string? URL { get; set; }
    [Required] public DateTime SignedDate { get; set; } = DateTime.Now;

    // Foreign keys
    [Required] public Guid SignerId { get; set; }
    [Required] public Guid DocumentId { get; set; }

    // Navigation properties
    [Required] public virtual UserRole Signer { get; set; } = null!;
    [Required] public virtual Document Document { get; set; } = null!;
    public virtual ICollection<Notification>? Notifications { get; set; }
}