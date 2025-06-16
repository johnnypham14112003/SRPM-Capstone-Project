using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;

namespace SRPM_Repositories.Repositories.Implements;

public class ProjectTagRepository : GenericRepository<ProjectTag>, IProjectTagRepository
{
    private readonly SRPMDbContext _context;
    public ProjectTagRepository(SRPMDbContext context) : base(context)
    {
        _context = context;
    }
}
