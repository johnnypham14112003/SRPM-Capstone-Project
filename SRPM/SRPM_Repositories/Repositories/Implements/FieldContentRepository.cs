using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;

namespace SRPM_Repositories.Repositories.Implements;

public class FieldContentRepository : GenericRepository<FieldContent>, IFieldContentRepository
{
    private readonly SRPMDbContext _context;
    public FieldContentRepository(SRPMDbContext context) : base(context)
    {
        _context = context;
    }
}