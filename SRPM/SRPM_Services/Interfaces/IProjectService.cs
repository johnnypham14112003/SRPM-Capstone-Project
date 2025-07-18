using SRPM_Services.BusinessModels;
using SRPM_Services.BusinessModels.Others;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;

namespace SRPM_Services.Interfaces;

public interface IProjectService
{
    Task<object?> GetByIdAsync(Guid id);
    Task<PagingResult<RS_Project>> GetListAsync(RQ_ProjectQuery query);
    Task<RS_Project> CreateAsync(RQ_Project request);
    Task<RS_Project?> UpdateAsync(Guid id, RQ_Project request);
    Task<RS_Project?> ToggleStatusAsync(Guid id);
    Task<bool> DeleteAsync(Guid id);
    Task<List<RS_ProjectOverview>> GetAllOverviewsAsync();
    //Task<RS_Project> SubmitProposalAsync(Guid sourceProjectId, RQ_ProposalSubmission payload); 
    //Task<bool> IsUserInProjectAsync(Guid projectId); 
    //Task<List<RS_Project>> GetUserProjectsAsync(); 
}