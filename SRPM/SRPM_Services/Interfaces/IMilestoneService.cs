using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SRPM_Services.BusinessModels.Others;
using SRPM_Repositories.Models;

namespace SRPM_Services.Interfaces
{
    public interface IMilestoneService
    {
        Task<RS_Milestone?> GetByIdAsync(Guid id);
        Task<PagingResult<RS_Milestone>> GetListAsync(RQ_MilestoneQuery query);
        Task<List<RS_Milestone>> GetByProjectAsync(Guid projectId);
        Task<RS_Milestone> CreateAsync(RQ_Milestone request);
        Task<RS_Milestone?> UpdateAsync(Guid id, RQ_Milestone request);
        Task<RS_Milestone?> ToggleStatusAsync(Guid id);
        Task<bool> DeleteAsync(Guid id);
    }

}
