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
}
