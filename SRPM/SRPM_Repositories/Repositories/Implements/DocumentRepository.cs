using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;

namespace SRPM_Repositories.Repositories.Implements;
public class DocumentRepository : GenericRepository<Document>, IDocumentRepository
{
    private readonly SRPMDbContext _context;
    public DocumentRepository(SRPMDbContext context) : base(context)
    {
        _context = context;
    }
}