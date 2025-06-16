using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;

namespace SRPM_Repositories.Repositories.Implements;

public class OTPCodeRepository : GenericRepository<OTPCode>, IOTPCodeRepository
{
    private readonly SRPMDbContext _context;
    public OTPCodeRepository(SRPMDbContext context) : base(context)
    {
        _context = context;
    }
}
