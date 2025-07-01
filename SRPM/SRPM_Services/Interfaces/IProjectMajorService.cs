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
    public interface IProjectMajorService
    {
        Task<PagingResult<RS_ProjectMajor>> GetListAsync(RQ_ProjectMajorQuery query);
        Task<RS_ProjectMajor> CreateAsync(RQ_ProjectMajor request);
        Task<RS_ProjectMajor?> UpdateAsync(Guid projectId, Guid majorId, RQ_ProjectMajor request);
        Task<bool> DeleteAsync(Guid projectId, Guid majorId);
    }

}
