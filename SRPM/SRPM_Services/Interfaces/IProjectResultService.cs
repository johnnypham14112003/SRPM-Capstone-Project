using SRPM_Services.BusinessModels;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.RequestModels.Query;
using SRPM_Services.BusinessModels.ResponseModels;

namespace SRPM_Services.Interfaces
{
    public interface IProjectResultService
    {
        Task<RS_ProjectResult> CreateAsync(RQ_ProjectResult request);
        Task<RS_ProjectResult> UpdateAsync(RQ_ProjectResult request);
        Task<RS_ProjectResult?> GetByIdAsync(Guid id);
        Task<PagingResult<RS_ProjectResult>> GetListAsync(Q_ProjectResult query);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> DeleteResultPublishAsync(Guid id);
    }
}
