using SRPM_Services.BusinessModels.Others;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.BusinessModels;
using System.Reflection;
using System.Runtime.Serialization;

namespace SRPM_Services.Extensions.Enumerables;

public static class EnumUtils
{
    public interface IProjectService
    {
        Task<RS_Project?> GetByIdAsync(Guid id, Guid userId); // Checks membership
        Task<PagingResult<RS_Project>> GetListAsync(RQ_ProjectQuery query);
        Task<RS_Project> CreateAsync(RQ_Project request); // Role-driven genre enforcement
        Task<RS_Project?> UpdateAsync(Guid id, RQ_Project request);
        Task<RS_Project?> ToggleStatusAsync(Guid id);
        Task<bool> DeleteAsync(Guid id);
        Task<List<RS_ProjectOverview>> GetAllOverviewsAsync();

        // 👇 NEW methods for proposals and PI actions
        Task<RS_Project> SubmitProposalAsync(Guid sourceProjectId, Guid principalId, RQ_ProposalSubmission payload); // clones, attaches documents
        Task<bool> IsUserInProjectAsync(Guid projectId, Guid userId); // for access filtering
        Task<List<RS_Project>> GetUserProjectsAsync(Guid userId); // useful for dashboards
    }

}
