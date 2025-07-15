using SRPM_Repositories.Models;

namespace SRPM_Repositories.Repositories.Interfaces;

public interface IEvaluationRepository : IGenericRepository<Evaluation>
{
    Task<Evaluation?> GetDetailWithInclude(Guid id, byte includeNo);
    Task<(List<Evaluation>? listEvaluation, int totalFound)> ListPaging
        (string? keyWord, string? evaPhrase, string? evaType, string? status,
        DateTime? fromDate, DateTime? toDate, byte? rating,
        Guid? projectId, Guid? milestoneId, Guid? appraisalCouncilId,
        byte sortBy, int pageIndex, int pageSize);
}