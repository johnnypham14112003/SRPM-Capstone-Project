namespace SRPM_Services.BusinessModels.RequestModels;

public class RQ_NotificationToUsers
{
    public List<Guid>? ListAccountId { get; set; }
    public Guid NotificationId { get; set; }
}