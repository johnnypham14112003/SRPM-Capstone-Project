using SRPM_Repositories.Models;
using System;
using System.Threading.Tasks;

namespace SRPM_Repositories.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Expose the generic repository for Account, for example.
        IGenericRepository<Account> AccountRepository { get; }

        // Add other repositories as needed.

        Task<bool> SaveChangesAsync();
    }
}
