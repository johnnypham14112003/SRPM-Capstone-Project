using Microsoft.EntityFrameworkCore;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;
using System.Text.RegularExpressions;

namespace SRPM_Repositories.Repositories.Implements;

public class AccountRepository : GenericRepository<Account>, IAccountRepository
{
    private readonly SRPMDbContext _context;
    public AccountRepository(SRPMDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Account> GetValidEmailAccountAsync(string email)
    {
        return await _context.Account
                             .Where(a => !string.IsNullOrEmpty(a.Email)
                                      && a.Email == email
                                      && a.Status != "deleted")
                             .FirstOrDefaultAsync();
    }


}
