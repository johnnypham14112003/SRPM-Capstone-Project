using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;

namespace SRPM_Repositories.Repositories.Implements;

public class ProjectResultRepository : GenericRepository<ProjectResult>, IProjectResultRepository
{
    private readonly SRPMDbContext _context;
    public ProjectResultRepository(SRPMDbContext context) : base(context)
    {
        _context = context;
    }
}
