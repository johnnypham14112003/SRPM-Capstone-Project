using Mapster;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Extensions.Enumerables;
using SRPM_Services.Interfaces;

namespace SRPM_Services.Implements
{
    public class EvaluationService : IEvaluationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EvaluationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<RS_Evaluation?> GetByIdAsync(Guid id)
        {
            var evaluation = await _unitOfWork.GetEvaluationRepository().GetByIdAsync<Guid>(id);
            return evaluation?.Adapt<RS_Evaluation>();
        }

        public async Task<List<RS_Evaluation>> GetListAsync()
        {
            var items = await _unitOfWork.GetEvaluationRepository().GetListAsync(_ => true, hasTrackings: false);
            return items.Adapt<List<RS_Evaluation>>();
        }

        public async Task<RS_Evaluation> CreateAsync(RQ_Evaluation request)
        {
            var prefix = request.Type.Equals("milestone", StringComparison.OrdinalIgnoreCase) ? "ME" : "PE";
            var code = $"{prefix}{new Random().Next(0, 999999):D6}";

            var evaluation = request.Adapt<Evaluation>();
            evaluation.Id = Guid.NewGuid();
            evaluation.Code = code;
            evaluation.CreateDate = DateTime.UtcNow;
            evaluation.Status = Status.Created.ToString().ToLowerInvariant();

            await _unitOfWork.GetEvaluationRepository().AddAsync(evaluation);
            await _unitOfWork.SaveChangesAsync();
            return evaluation.Adapt<RS_Evaluation>();
        }

        public async Task<RS_Evaluation?> UpdateAsync(Guid id, RQ_Evaluation request)
        {
            var repo = _unitOfWork.GetEvaluationRepository();
            var evaluation = await repo.GetByIdAsync<Guid>(id);
            if (evaluation == null) return null;

            request.Adapt(evaluation); // Only non-null values will be updated if config uses .IgnoreNullValues(true)

            await repo.UpdateAsync(evaluation);
            await _unitOfWork.SaveChangesAsync();
            return evaluation.Adapt<RS_Evaluation>();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var repo = _unitOfWork.GetEvaluationRepository();
            var evaluation = await repo.GetByIdAsync<Guid>(id);
            if (evaluation == null) return false;

            await repo.DeleteAsync(evaluation);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public Task<Dictionary<string, double>?> CheckPlagiarism(string inputText, IEnumerable<string> inputSource)
        {
            throw new NotImplementedException();
        }
    }


}
