using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;

namespace SRPM_Repositories.Repositories.Implements;

public class RoleRepository : GenericRepository<Role>, IRoleRepository
{
    private readonly SRPMDbContext _context;
    public RoleRepository(SRPMDbContext context) : base(context)
    {
        _context = context;
    }
}
