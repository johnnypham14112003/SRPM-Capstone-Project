using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.Interfaces
{
    public interface IEvaluationStageService
    {
        Task<RS_EvaluationStage?> GetByIdAsync(Guid id);
        Task<List<RS_EvaluationStage>> GetListByEvaluationIdAsync(Guid evaluationId);
        Task<RS_EvaluationStage> CreateAsync(RQ_EvaluationStage request);
        Task<RS_EvaluationStage?> UpdateAsync(Guid id, RQ_EvaluationStage request);
        Task<bool> DeleteAsync(Guid id);
    }

}
