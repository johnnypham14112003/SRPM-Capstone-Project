using SRPM_Services.BusinessModels;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.RequestModels.Query;
using SRPM_Services.BusinessModels.ResponseModels;

namespace SRPM_Services.Interfaces;

public interface IAppraisalCouncilService
{
    Task<(bool result, Guid CoucilId)> NewAppraisal(RQ_AppraisalCouncil inputData);
    Task<PagingResult<RS_AppraisalCouncil>?> ListCouncil(Q_AppraisalCouncil queryInput);
    Task<RS_AppraisalCouncil> ViewDetailCouncil(Guid? id, byte includeNum = 0);
    Task<bool> UpdateCouncilInfo(RQ_AppraisalCouncil newCouncil);
    Task<bool> DeleteCouncil(Guid id);
    Task<List<RS_ProjectsOfCouncil>?> GetProjectsFromCouncilAsync(Guid councilId);
    Task<RS_AppraisalCouncil?> GetCouncilInEvaluationAsync(Guid projectId);
    Task<bool> AssignCouncilToClonedStages(Guid sourceProjectId, Guid appraisalCouncilId);
    Task<PagingResult<RS_AppraisalCouncil>> GetAllOnlineUserAppraisalCouncilAsync(int pageIndex, int pageSize);
}