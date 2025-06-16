using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;

namespace SRPM_Repositories.Repositories.Implements;
public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
{
    private readonly SRPMDbContext _context;
    public TransactionRepository(SRPMDbContext context) : base(context)
    {
        _context = context;
    }
}