using SRPM_Services.BusinessModels;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.Interfaces
{
    public interface IProjectService
    {
        Task<RS_Project?> GetByIdAsync(Guid id);
        Task<PagingResult<RS_Project>> GetListAsync(RQ_ProjectQuery query);
        Task<RS_Project> CreateAsync(RQ_Project request);
        Task<RS_Project?> UpdateAsync(Guid id, RQ_Project request);
        Task<RS_Project?> ToggleStatusAsync(Guid id);
        Task<bool> DeleteAsync(Guid id);
    }


}
