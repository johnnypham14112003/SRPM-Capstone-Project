using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;

namespace SRPM_Repositories.Repositories.Implements;

public class EvaluationStageRepository : GenericRepository<EvaluationStage>, IEvaluationStageRepository
{
    private readonly SRPMDbContext _context;
    public EvaluationStageRepository(SRPMDbContext context) : base(context)
    {
        _context = context;
    }
}