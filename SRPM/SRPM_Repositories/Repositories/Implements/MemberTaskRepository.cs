using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;

namespace SRPM_Repositories.Repositories.Implements;
public class MemberTaskRepository : GenericRepository<MemberTask>, IMemberTaskRepository
{
    private readonly SRPMDbContext _context;
    public MemberTaskRepository(SRPMDbContext context) : base(context)
    {
        _context = context;
    }
}