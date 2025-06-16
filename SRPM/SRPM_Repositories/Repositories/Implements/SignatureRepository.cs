using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;

namespace SRPM_Repositories.Repositories.Implements;

public class SignatureRepository : GenericRepository<Signature>, ISignatureRepository
{
    private readonly SRPMDbContext _context;
    public SignatureRepository(SRPMDbContext context) : base(context)
    {
        _context = context;
    }
}
