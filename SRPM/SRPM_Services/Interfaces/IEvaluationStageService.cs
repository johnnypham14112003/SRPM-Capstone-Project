using SRPM_Services.BusinessModels;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.RequestModels.Query;
using SRPM_Services.BusinessModels.ResponseModels;

namespace SRPM_Services.Interfaces;

public interface IEvaluationStageService
{
    Task<RS_EvaluationStage?> ViewDetail(Guid id, byte includeNum);
    Task<PagingResult<RS_EvaluationStage>> GetListPagingAsync(Q_EvaluationStage queryInput);
    Task<(bool success, Guid evaluationStageId)> CreateAsync(RQ_EvaluationStage newEvaluationStage);
    Task<bool> UpdateAsync(RQ_EvaluationStage newEvaluationStage);
    Task<bool> DeleteAsync(Guid id);
}