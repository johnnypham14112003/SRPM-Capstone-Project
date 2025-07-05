using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;

namespace SRPM_Services.Interfaces
{
    public interface IEvaluationService
    {
        Task<RS_Evaluation?> GetByIdAsync(Guid id);
        Task<Dictionary<string, double>?> CheckPlagiarism(string inputText, IEnumerable<string> inputSource);
        Task<List<RS_Evaluation>> GetListAsync();
        Task<RS_Evaluation> CreateAsync(RQ_Evaluation request);
        Task<RS_Evaluation?> UpdateAsync(Guid id, RQ_Evaluation request);
        Task<bool> DeleteAsync(Guid id);
    }

}
