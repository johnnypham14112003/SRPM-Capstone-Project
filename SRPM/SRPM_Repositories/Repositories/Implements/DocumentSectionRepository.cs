using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;

namespace SRPM_Repositories.Repositories.Implements;

public class DocumentSectionRepository : GenericRepository<DocumentSection>, IDocumentSectionRepository
{
    private readonly SRPMDbContext _context;
    public DocumentSectionRepository(SRPMDbContext context) : base(context)
    {
        _context = context;
    }
}