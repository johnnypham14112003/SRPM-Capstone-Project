using SRPM_Services.BusinessModels;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;

namespace SRPM_Services.Interfaces;

public interface IUserRoleService
{
    Task<RS_UserRole?> GetByIdAsync(Guid id);
    Task<bool> UserHasRoleAsync(Guid accountId, string roleName);
    Task<PagingResult<RS_UserRole>> GetListAsync(RQ_UserRoleQuery query);
    Task<IEnumerable<string>> GetAllUserRole(Guid userId);
    Task<List<RS_UserRole>> GetAllAsync();
    Task<RS_UserRole> CreateAsync(RQ_UserRole request);
    Task<RS_UserRole?> UpdateAsync(Guid id, RQ_UserRole request, string? Status);
    Task<bool> DeleteAsync(Guid id);
    Task<RS_UserRole?> ToggleStatusAsync(Guid id);
}