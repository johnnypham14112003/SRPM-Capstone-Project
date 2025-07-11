using Microsoft.EntityFrameworkCore;
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
    public async Task<List<ProjectMajor>> GetListWithIncludesAsync(
    Guid? projectId,
    Guid? majorId)
    {
        IQueryable<ProjectMajor> query = _context.Set<ProjectMajor>()
            .Include(pm => pm.Project)
            .Include(pm => pm.Major)
            .ThenInclude(m => m.Field);

        if (projectId.HasValue)
            query = query.Where(pm => pm.ProjectId == projectId.Value);

        if (majorId.HasValue)
            query = query.Where(pm => pm.MajorId == majorId.Value);

        return await query.AsNoTracking().ToListAsync();
    }

}
