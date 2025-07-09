using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;

namespace SRPM_Repositories.Repositories.Implements;

public class TableRowRepository : GenericRepository<TableRow>, ITableRowRepository
{
    private readonly SRPMDbContext _context;
    public TableRowRepository(SRPMDbContext context) : base(context)
    {
        _context = context;
    }
}