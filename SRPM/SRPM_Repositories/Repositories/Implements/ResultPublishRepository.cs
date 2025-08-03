using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;

namespace SRPM_Repositories.Repositories.Implements;

public class ResultPublishRepository : GenericRepository<ResultPublish>, IResultPublishRepository
{
    private readonly SRPMDbContext _context;
    public ResultPublishRepository(SRPMDbContext context) : base(context)
    {
        _context = context;
    }
}