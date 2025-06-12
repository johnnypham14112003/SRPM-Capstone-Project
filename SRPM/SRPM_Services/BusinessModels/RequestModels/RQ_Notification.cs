using System.ComponentModel.DataAnnotations;

namespace SRPM_Services.BusinessModels.RequestModels;
public class RQ_Notification
{
    [Key]
    public Guid? Id { get; set; }

    [Required, MaxLength(255)]
    public string? Title { get; set; }

    [Required, MaxLength(30)]
    public string? Type { get; set; }

    [Required]
    public bool IsGlobalSend { get; set; } = false;

    [Required]
    public DateTime CreateDate { get; set; } = DateTime.Now;

    [Required, MaxLength(30)]
    public string Status { get; set; } = "created";
    public Guid? ObjecNotificationId { get; set; }
}