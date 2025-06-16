using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;

namespace SRPM_Repositories.Repositories.Implements;
public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
{
    private readonly SRPMDbContext _context;
    public NotificationRepository(SRPMDbContext context) : base(context)
    {
        _context = context;
    }
}
