using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;

namespace SRPM_Repositories.Repositories.Repositories;
public class IndividualEvaluationRepository : GenericRepository<IndividualEvaluation>, IIndividualEvaluationRepository
{
    private readonly SRPMDbContext _context;
    public IndividualEvaluationRepository(SRPMDbContext context) : base(context)
    {
        _context = context;
    }
}