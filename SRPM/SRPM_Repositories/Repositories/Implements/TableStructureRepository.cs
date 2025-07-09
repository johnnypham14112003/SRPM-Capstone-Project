using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;

namespace SRPM_Repositories.Repositories.Implements;

public class TableStructureRepository : GenericRepository<TableStructure>, ITableStructureRepository
{
    private readonly SRPMDbContext _context;
    public TableStructureRepository(SRPMDbContext context) : base(context)
    {
        _context = context;
    }
}