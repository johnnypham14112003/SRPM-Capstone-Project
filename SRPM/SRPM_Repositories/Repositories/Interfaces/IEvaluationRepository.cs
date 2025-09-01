using SRPM_Repositories.Models;

namespace SRPM_Repositories.Repositories.Interfaces;

public interface IEvaluationRepository : IGenericRepository<Evaluation>
{
    Task<Evaluation?> GetDetailWithInclude(Guid id, byte includeNo);
    Task<List<Evaluation>?> FilterByEvaAndCouncil(Guid projectId, Guid councilId);
    Task<(List<Evaluation>? listEvaluation, int totalFound)> ListPaging
        (string? keyWord, string? status,
        DateTime? fromDate, DateTime? toDate, byte? rating,
        Guid? projectId, Guid? appraisalCouncilId,
        byte sortBy, int pageIndex, int pageSize);
}