using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;

public class AccountNotification
{
    [Required] public bool IsRead { get; set; } = false;
    [Required] public DateTime CreateDate { get; set; } = DateTime.Now;

    // Composite Primary Key
    [Required] public Guid AccountId { get; set; }
    [Required] public Guid NotificationId { get; set; }

    // Navigation properties
    public virtual Account Account { get; set; } = null!;
    public virtual Notification Notification { get; set; } = null!;
}