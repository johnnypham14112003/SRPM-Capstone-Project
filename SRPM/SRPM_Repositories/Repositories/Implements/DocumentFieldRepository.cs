using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;

namespace SRPM_Repositories.Repositories.Implements;

public class DocumentFieldRepository : GenericRepository<DocumentField>, IDocumentFieldRepository
{
    private readonly SRPMDbContext _context;
    public DocumentFieldRepository(SRPMDbContext context) : base(context)
    {
        _context = context;
    }
}