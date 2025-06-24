using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.Interfaces
{
    public interface IMemberTaskService
    {
        Task<RS_MemberTask?> GetByIdAsync(Guid id);
        Task<PagingResult<RS_MemberTask>> GetListAsync(RQ_MemberTaskQuery query);
        Task<RS_MemberTask> CreateAsync(RQ_MemberTask request);
        Task<RS_MemberTask?> UpdateAsync(Guid id, RQ_MemberTask request);
        Task<RS_MemberTask?> ToggleStatusAsync(Guid id);
        Task<bool> DeleteAsync(Guid id);
    }

}
