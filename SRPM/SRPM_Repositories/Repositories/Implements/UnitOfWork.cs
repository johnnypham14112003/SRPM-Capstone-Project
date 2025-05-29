using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Repositories.Repositories.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SRPMDbContext _context;
        private IGenericRepository<Account> _accountRepository;

        public UnitOfWork(SRPMDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<Account> AccountRepository =>
            _accountRepository ??= new GenericRepository<Account>(_context);

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
