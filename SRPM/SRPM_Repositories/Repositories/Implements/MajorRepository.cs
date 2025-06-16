using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;

namespace SRPM_Repositories.Repositories.Implements;

public class MajorRepository : GenericRepository<Major>, IMajorRepository
{
    private readonly SRPMDbContext _context;
    public MajorRepository(SRPMDbContext context) : base(context)
    {
        _context = context;
    }
}
