using SRPM_Repositories.Models;

namespace SRPM_Repositories.Repositories.Interfaces;

public interface IIndividualEvaluationRepository : IGenericRepository<IndividualEvaluation>
{
    Task<(List<IndividualEvaluation>? listIndividualEvaluations, int totalFound)> GetListPagingAsync(
        string? keyword, byte? totalRate, DateTime? fromDate, DateTime? toDate,
        bool? isApproved, bool? reviewerResult, bool? isAIReport,
        string? status, Guid? evaluationStageId, Guid? reviewerId,
        byte sortBy, int pageIndex, int pageSize);
}
