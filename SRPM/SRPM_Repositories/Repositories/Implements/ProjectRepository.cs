using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;

namespace SRPM_Repositories.Repositories.Implements;

public class ProjectRepository : GenericRepository<Project>, IProjectRepository
{
    private readonly SRPMDbContext _context;
    public ProjectRepository(SRPMDbContext context) : base(context)
    {
        _context = context;
    }
}
