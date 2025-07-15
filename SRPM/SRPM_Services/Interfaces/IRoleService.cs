using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;

namespace SRPM_Services.Interfaces;

public interface IRoleService
{
    Task<RS_Role?> GetByIdAsync(Guid id);
    Task<List<RS_Role>> GetAllAsync();
    Task<RS_Role> CreateAsync(RQ_Role request);
    Task<RS_Role?> UpdateAsync(Guid id, RQ_Role request);
    Task<bool> DeleteAsync(Guid id);
    Task<RS_Role?> ToggleStatusAsync(Guid id);

}