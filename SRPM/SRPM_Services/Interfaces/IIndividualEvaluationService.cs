using SRPM_Services.BusinessModels;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.RequestModels.Query;
using SRPM_Services.BusinessModels.ResponseModels;

namespace SRPM_Services.Interfaces;

public interface IIndividualEvaluationService
{
    Task<RS_IndividualEvaluation?> ViewDetail(Guid id);
    Task<PagingResult<RS_IndividualEvaluation>> GetListAsync(Q_IndividualEvaluation queryInput);
    Task<(bool success, Guid individualEvaluationId)> CreateAsync(RQ_IndividualEvaluation newIndividualEvaluation);
    Task<bool> UpdateAsync(RQ_IndividualEvaluation newIndividualEvaluation);
    Task<bool> DeleteAsync(Guid id);
}