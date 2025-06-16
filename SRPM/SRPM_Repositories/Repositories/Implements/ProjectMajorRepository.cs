using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;

namespace SRPM_Repositories.Repositories.Implements;

public class ProjectMajorRepository : GenericRepository<ProjectMajor>, IProjectMajorRepository
{
    private readonly SRPMDbContext _context;
    public ProjectMajorRepository(SRPMDbContext context) : base(context)
    {
        _context = context;
    }
}
