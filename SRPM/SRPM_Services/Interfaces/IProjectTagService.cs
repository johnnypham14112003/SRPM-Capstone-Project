using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;

namespace SRPM_Services.Interfaces;

public interface IProjectTagService
{
    Task<RS_ProjectTag?> GetByIdAsync(Guid id);
    Task<List<RS_ProjectTag>> GetByProjectIdAsync(Guid projectId);
    Task<List<RS_ProjectTag>> CreateAsync(RQ_ProjectTag request);
    Task<RS_ProjectTag?> UpdateAsync(Guid id, RQ_ProjectTag request);
    Task<bool> DeleteAsync(Guid id);
}