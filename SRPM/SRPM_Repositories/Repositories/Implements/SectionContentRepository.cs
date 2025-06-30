using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;

namespace SRPM_Repositories.Repositories.Implements;

public class SectionContentRepository : GenericRepository<SectionContent>, ISectionContentRepository
{
    private readonly SRPMDbContext _context;
    public SectionContentRepository(SRPMDbContext context) : base(context)
    {
        _context = context;
    }
}