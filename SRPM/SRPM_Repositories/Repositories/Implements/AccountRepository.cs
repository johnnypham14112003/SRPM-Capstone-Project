
using SRPM_Repositories.DBContext;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;

namespace SRPM_Repositories.Repositories.Repositories;

public class AccountRepository : GenericRepository<Account>, IAccountRepository
{
    private readonly SRPMDbContext _context;
    public AccountRepository(SRPMDbContext context) : base(context)
    {
        _context = context;
    }
}
