using SRPM_Services.BusinessModels;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.RequestModels.Query;
using SRPM_Services.BusinessModels.ResponseModels;

namespace SRPM_Services.Interfaces;

public interface IEvaluationService
{
    Task<RS_Evaluation?> ViewDetail(Guid id, byte includeNum);
    Task<PagingResult<RS_Evaluation>> GetListAsync(Q_Evaluation queryInput);
    Task<(bool success, Guid evaluationId)> CreateAsync(RQ_Evaluation newEvaluation);
    Task<bool> UpdateAsync(RQ_Evaluation newEvaluation);
    Task<bool> DeleteAsync(Guid id);
    Task<string> FirstAIEvaluation(Guid projectId);
    Task<string> RegenAIEvaluation(Guid projectId, Guid individualEvalutionId);
    Task<List<RS_Evaluation>?> GetListByProjectCouncilAsync(Guid projectId, Guid councilId);
}