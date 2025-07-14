using Microsoft.EntityFrameworkCore;
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

    public async Task<Project?> GetByIdAsync(Guid id, bool hasTrackings = false)
    {
        IQueryable<Project> query = _context.Set<Project>();

        if (!hasTrackings)
            query = query.AsNoTracking();

        query = query
            .Include(p => p.Creator).ThenInclude(c => c.Role)
            .Include(p => p.ProjectMajors).ThenInclude(pm => pm.Major).ThenInclude(m => m.Field)
            .Include(p => p.Members).ThenInclude(m => m.Account)
            .Include(p => p.Milestones)
            .Include(p => p.Evaluations)
            .Include(p => p.IndividualEvaluations)
            .Include(p => p.ProjectTags)
            .Include(p => p.Documents)
            .Include(p => p.Transactions)
            .Include(p => p.ResearchPaper);

        return await query.FirstOrDefaultAsync(p => p.Id == id);
    }

}
