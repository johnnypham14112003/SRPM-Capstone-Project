using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.Interfaces
{
    public interface IIndividualEvaluationService
    {
        Task<RS_IndividualEvaluation?> GetByIdAsync(Guid id);
        Task<List<RS_IndividualEvaluation>> GetListByStageAsync(Guid evaluationStageId);
        Task<RS_IndividualEvaluation> CreateAsync(RQ_IndividualEvaluation request);
        Task<RS_IndividualEvaluation?> UpdateAsync(Guid id, RQ_IndividualEvaluation request);
    }

}
