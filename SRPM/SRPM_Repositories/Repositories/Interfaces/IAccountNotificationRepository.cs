using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Repositories;

namespace SRPM_Repositories.Repositories.Interfaces;
public interface IAccountNotificationRepository : IGenericRepository<AccountNotification>
{
    Task<List<Guid>?> ListIdAllAccount();
    Task<(List<NotificationWithReadStatus>? listNotificationWithStatus, int totalCount, bool isError)> ListAccountNotification
         (Guid accountId, string? keyWord, DateTime? fromDate, DateTime? toDate,
        bool isRead, string? type, string? status, int pageIndex, int pageSize);
}
