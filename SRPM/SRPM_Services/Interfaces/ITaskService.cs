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
    public interface ITaskService
    {
        Task<RS_Task?> GetByIdAsync(Guid id);
        Task<PagingResult<RS_Task>> GetListAsync(RQ_TaskQuery query);

        Task<RS_Task> CreateAsync(RQ_Task request);
        Task<RS_Task?> UpdateAsync(Guid id, RQ_Task request);
        Task<RS_Task?> ToggleStatusAsync(Guid id);
        Task<bool> DeleteAsync(Guid id);
    }
}
