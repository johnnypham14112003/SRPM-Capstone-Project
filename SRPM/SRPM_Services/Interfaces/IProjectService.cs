using SRPM_Repositories.Models;
using SRPM_Services.BusinessModels;
using SRPM_Services.BusinessModels.Others;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Extensions.Enumerables;
using SRPM_Services.Extensions.Exceptions;
using SRPM_Services.Implements;

namespace SRPM_Services.Interfaces;

public interface IProjectService
{
    Task<object?> GetByIdAsync(Guid id);
    Task<PagingResult<RS_Project>> GetListAsync(RQ_ProjectQuery query);
    Task<RS_Project> CreateAsync(RQ_Project request);
    Task<RS_Project?> UpdateAsync(Guid id, RQ_Project request, string status);
    Task<RS_Project?> ToggleStatusAsync(Guid id);
    Task<bool> DeleteAsync(Guid id);
    Task<List<RS_ProjectOverview>> GetAllOnlineUserProjectAsync(
        List<string>? statusList,
        List<string>? genreList);
    Task<RS_Project> EnrollAsPrincipalAsync(Guid sourceProjectId);
    Task<bool> CreateFromDocumentAsync(RQ_MilestoneTaskContent content);
    /*
     * The milestone text should be Bold
     * The task text should be normal
     * The time must be in format (start,end): dd/mm/yyyy, dd/mm/yyyy
     */

    Task<List<RS_ProjectOverview>> GetHostProjectHistory();
    Task<List<RS_ProjectOverview>> GetStaffProjectHistory();
    Task<bool> ApproveProposalAsync(Guid proposalProjectId);
    Task<(Guid id, bool isEnrolled)> CheckIsEnrollInProject(Guid sourceProject);
}