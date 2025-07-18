using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;

namespace SRPM_Repositories.Repositories.Implements;

public class ProjectSimilarityRepository : GenericRepository<ProjectSimilarity>, IProjectSimilarityRepository
{
    private readonly SRPMDbContext _context;
    public ProjectSimilarityRepository(SRPMDbContext context) : base(context)
    {
        _context = context;
    }
}