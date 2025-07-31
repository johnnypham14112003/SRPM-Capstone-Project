using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Implements;

namespace SRPM_Repositories.Repositories.Interfaces;

public interface IAccountNotificationRepository : IGenericRepository<AccountNotification>
{
    Task<List<Guid>?> ListIdAllAccount();
    Task<(List<NotificationWithReadStatus>? listNotificationWithStatus, int totalCount)> ListAccountNotification
         (Guid accountId, string? keyWord, DateTime? fromDate, DateTime? toDate,
        bool isRead, string? type, string? status, int pageIndex, int pageSize);
    Task<(List<NotificationWithReadStatus>? listNotificationWithStatus, int totalCount)> ListAccountNotification
            (string email, string? keyWord, DateTime? fromDate, DateTime? toDate,
            bool isRead, string? type, string? status, int pageIndex, int pageSize);
}
