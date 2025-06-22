using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.Interfaces
{
    public interface IUserRoleService
    {
        Task<RS_UserRole?> GetByIdAsync(Guid id);
        Task<List<RS_UserRole>> GetAllAsync();
        Task<RS_UserRole> CreateAsync(RQ_UserRole request);
        Task<RS_UserRole?> UpdateAsync(Guid id, RQ_UserRole request);
        Task<bool> DeleteAsync(Guid id);
        Task<RS_UserRole?> ToggleStatusAsync(Guid id);

    }

}
