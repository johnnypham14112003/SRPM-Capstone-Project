using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;

namespace SRPM_Repositories.Repositories.Repositories;
public class SystemConfigurationRepository : GenericRepository<SystemConfiguration>, ISystemConfigurationRepository
{
    private readonly SRPMDbContext _context;
    public SystemConfigurationRepository(SRPMDbContext context) : base(context)
    {
        _context = context;
    }
}