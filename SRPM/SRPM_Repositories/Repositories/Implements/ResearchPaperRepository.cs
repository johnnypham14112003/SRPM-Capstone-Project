using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;

namespace SRPM_Repositories.Repositories.Implements;

public class ResearchPaperRepository : GenericRepository<ResearchPaper>, IResearchPaperRepository
{
    private readonly SRPMDbContext _context;
    public ResearchPaperRepository(SRPMDbContext context) : base(context)
    {
        _context = context;
    }
}
