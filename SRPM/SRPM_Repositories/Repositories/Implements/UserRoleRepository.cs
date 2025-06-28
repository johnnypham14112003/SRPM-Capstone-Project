using Microsoft.EntityFrameworkCore;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;

namespace SRPM_Repositories.Repositories.Implements;
public class UserRoleRepository : GenericRepository<UserRole>, IUserRoleRepository
{
    private readonly SRPMDbContext _context;
    public UserRoleRepository(SRPMDbContext context) : base(context)
    {
        _context = context;
    }
    public async Task<List<UserRole>> GetListByFilterAsync(
    Guid? accountId,
    Guid? roleId,
    Guid? projectId,
    Guid? appraisalCouncilId,
    string? status,
    bool? isOfficial)
    {
        IQueryable<UserRole> query = _context.Set<UserRole>().Include(r => r.Role);

        if (accountId.HasValue)
            query = query.Where(x => x.AccountId == accountId.Value);

        if (roleId.HasValue)
            query = query.Where(x => x.RoleId == roleId.Value);

        if (projectId.HasValue)
            query = query.Where(x => x.ProjectId == projectId.Value);

        if (appraisalCouncilId.HasValue)
            query = query.Where(x => x.AppraisalCouncilId == appraisalCouncilId.Value);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(x => x.Status == status);

        if (isOfficial.HasValue)
            query = query.Where(x => x.IsOfficial == isOfficial.Value);

        return await query.AsNoTracking().ToListAsync();
    }
}
