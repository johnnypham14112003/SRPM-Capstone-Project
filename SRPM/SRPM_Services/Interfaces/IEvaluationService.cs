using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.Interfaces
{
    public interface IEvaluationService
    {
        Task<RS_Evaluation?> GetByIdAsync(Guid id);
        Task<List<RS_Evaluation>> GetListAsync();
        Task<RS_Evaluation> CreateAsync(RQ_Evaluation request);
        Task<RS_Evaluation?> UpdateAsync(Guid id, RQ_Evaluation request);
        Task<bool> DeleteAsync(Guid id);
    }

}
