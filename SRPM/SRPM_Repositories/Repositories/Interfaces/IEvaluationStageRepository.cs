using SRPM_Repositories.Models;

namespace SRPM_Repositories.Repositories.Interfaces;

public interface IEvaluationStageRepository : IGenericRepository<EvaluationStage>
{
    Task<EvaluationStage?> GetStageDetailWithInclude(Guid id, byte includeNo);
    Task<(List<EvaluationStage>? listStage, int totalFound)> ListStagePaging(
    string? keyWord, string? status,
    Guid? evaluationId, Guid? appraisalCouncilId,
    byte sortBy, int pageIndex, int pageSize);
}