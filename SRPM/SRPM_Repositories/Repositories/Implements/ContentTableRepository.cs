using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;

namespace SRPM_Repositories.Repositories.Implements;

public class ContentTableRepository : GenericRepository<ContentTable>, IContentTableRepository
{
    private readonly SRPMDbContext _context;
    public ContentTableRepository(SRPMDbContext context) : base(context)
    {
        _context = context;
    }
}