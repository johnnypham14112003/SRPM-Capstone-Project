using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;

namespace SRPM_Repositories.Repositories.Implements;

public class MilestoneRepository : GenericRepository<Milestone>, IMilestoneRepository
{
    private readonly SRPMDbContext _context;
    public MilestoneRepository(SRPMDbContext context) : base(context)
    {
        _context = context;
    }
}
