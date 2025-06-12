using SRPM_Services.BusinessModels;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;

namespace SRPM_Services.Interfaces;
public interface INotificationService
{
    Task<(bool, Guid)> CreateNew(RQ_Notification newNotification);//Return NotificationId
    Task<bool> NotificateToUser(List<Guid>? ListAccountId, Guid notificationId);
    Task<PagingResult<RS_AccountNotification>?> ListNotificationOfUser(RQ_QueryAccountNotification queryInput);
    Task<bool> UpdateNotification(RQ_Notification newNotification);
    Task<bool> DeleteNotification(Guid id);
}