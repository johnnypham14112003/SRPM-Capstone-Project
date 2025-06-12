using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;
public class AccountNotification
{
    [Required]
    public Guid AccountId { get; set; }

    [Required]
    public Guid NotificationId { get; set; }

    [Required]
    public bool IsRead { get; set; } = false;
    public DateTime CreateDate { get; set; } = DateTime.Now;

    // Navigation properties
    public virtual Account Account { get; set; } = null!;
    public virtual Notification Notification { get; set; } = null!;
}