using Microsoft.EntityFrameworkCore;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;
namespace SRPM_Repositories.Repositories.Implements;
public class AccountNotificationRepository : GenericRepository<AccountNotification>, IAccountNotificationRepository
{
    private readonly SRPMDbContext _context;
    public AccountNotificationRepository(SRPMDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<Guid>?> ListIdAllAccount()
    {
        return await _context.Account
            .AsNoTracking()
            .Select(a => a.Id)
            .ToListAsync();
    }

    public async Task<(List<NotificationWithReadStatus>? listNotificationWithStatus, int totalCount)> ListAccountNotification
        (Guid accountId, string? keyWord, DateTime? fromDate, DateTime? toDate,
        bool isRead, string? type, string? status, int pageIndex, int pageSize)
    {
        var query = _context.AccountNotification
            .Include(an => an.Notification)
            .Where(an => an.AccountId == accountId)
            .AsNoTracking()
            .AsQueryable();

        // ===========================[ Apply Search ]===========================
        //Title Filter
        if (!string.IsNullOrWhiteSpace(keyWord))
            query = query.Where(an => an.Notification.Title.ToLower().Contains(keyWord.ToLower()));

        // IsRead Filter
        //* If only show all unread then isRead == false / else will show combine
        if (isRead == false) query = query.Where(an => an.IsRead == false);

        // Type Filter
        // If not null => Show only notification of a type / else show all
        if (!string.IsNullOrWhiteSpace(type))
            query = query.Where(an => an.Notification.Type.ToLower().Equals(type.ToLower()));

        //Status Filter
        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(an => an.Notification.Status.ToLower().Equals(status.ToLower()));

        //Date Filter
        if (fromDate.HasValue)
            query = query.Where(an => an.Notification.CreateDate >= fromDate.Value);
        if (toDate.HasValue)
            query = query.Where(an => an.Notification.CreateDate <= toDate.Value);

        // Sort by Date (Newer come first)
        query = query.OrderByDescending(an => an.Notification.CreateDate);

        // Sum notification of a user
        int sumAccNoti = await query.CountAsync();

        // ===========================[ Apply paging ]===========================
        var pagedList = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(an => new NotificationWithReadStatus
            {
                Notification = an.Notification,
                AccountId = accountId,
                IsRead = an.IsRead
            })
            .ToListAsync();

        return (pagedList, sumAccNoti);
    }

    public async Task<(List<NotificationWithReadStatus>? listNotificationWithStatus, int totalCount)> ListAccountNotification
            (string email, string? keyWord, DateTime? fromDate, DateTime? toDate,
            bool isRead, string? type, string? status, int pageIndex, int pageSize)
    {
        var query = _context.AccountNotification
            .Include(an => an.Notification)
            .Include(an => an.Account)
            .Where(an => an.Account.Email == email)
            .AsNoTracking()
            .AsSplitQuery()
            .AsQueryable();

        // ===========================[ Apply Search ]===========================
        //Title Filter
        if (!string.IsNullOrWhiteSpace(keyWord))
            query = query.Where(an => an.Notification.Title.ToLower().Contains(keyWord.ToLower()));

        // IsRead Filter
        //* If only show all unread then isRead == false / else will show combine
        if (isRead == false) query = query.Where(an => an.IsRead == false);

        // Type Filter
        // If not null => Show only notification of a type / else show all
        if (!string.IsNullOrWhiteSpace(type))
            query = query.Where(an => an.Notification.Type.ToLower().Equals(type.ToLower()));

        //Status Filter
        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(an => an.Notification.Status.ToLower().Equals(status.ToLower()));

        //Date Filter
        if (fromDate.HasValue)
            query = query.Where(an => an.Notification.CreateDate >= fromDate.Value);
        if (toDate.HasValue)
            query = query.Where(an => an.Notification.CreateDate <= toDate.Value);

        // Sort by Date (Newer come first)
        query = query.OrderByDescending(an => an.Notification.CreateDate);

        // Sum notification of a user
        int sumAccNoti = await query.CountAsync();

        // ===========================[ Apply paging ]===========================
        var pagedList = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(an => new NotificationWithReadStatus
            {
                Notification = an.Notification,
                AccountId = an.AccountId,
                IsRead = an.IsRead
            })
            .ToListAsync();

        return (pagedList, sumAccNoti);
    }
}

// Combine Object
public class NotificationWithReadStatus
{
    public Notification Notification { get; set; } = null!;
    public Guid AccountId { get; set; }
    public bool IsRead { get; set; }
}
