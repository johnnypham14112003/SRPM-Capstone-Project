using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;

namespace SRPM_Repositories.Repositories.Repositories;
public class EvaluationRepository : GenericRepository<Evaluation>, IEvaluationRepository
{
    private readonly SRPMDbContext _context;
    public EvaluationRepository(SRPMDbContext context) : base(context)
    {
        _context = context;
    }
}