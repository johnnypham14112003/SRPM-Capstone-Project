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
    public interface IMajorService
    {
        Task<RS_Major?> GetByIdAsync(Guid id);
        Task<PagingResult<RS_Major>> GetListAsync(RQ_MajorQuery query);
        Task<RS_Major> CreateAsync(RQ_Major request);
        Task<RS_Major?> UpdateAsync(Guid id, RQ_Major request);
        Task<bool> DeleteAsync(Guid id);
    }
}
