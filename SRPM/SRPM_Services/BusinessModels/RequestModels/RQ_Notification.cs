namespace SRPM_Services.BusinessModels.RequestModels;

public class RQ_Notification
{
    public Guid? Id { get; set; }
    public string? Title { get; set; }
    public string? Type { get; set; }
    public bool IsGlobalSend { get; set; } = false;
    public DateTime CreateDate { get; set; } = DateTime.Now;
    public string Status { get; set; } = "created";
    public Guid? ObjecNotificationId { get; set; }
}