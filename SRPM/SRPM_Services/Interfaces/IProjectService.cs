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
    Task<List<RS_ProjectOverview>> GetAllOnlineUserProjectAsync();
    Task<RS_Project> EnrollAsPrincipalAsync(Guid sourceProjectId);
}