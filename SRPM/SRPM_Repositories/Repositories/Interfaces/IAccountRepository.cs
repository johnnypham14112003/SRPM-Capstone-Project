using SRPM_Repositories.Models;

namespace SRPM_Repositories.Repositories.Interfaces;

public interface IAccountRepository : IGenericRepository<Account>
{
    Task<Account> GetValidEmailAccountAsync(string email);
}
