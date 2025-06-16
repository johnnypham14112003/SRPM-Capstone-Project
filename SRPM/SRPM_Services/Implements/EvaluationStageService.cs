using Mapster;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.Implements
{
    public class EvaluationStageService : IEvaluationStageService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EvaluationStageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<RS_EvaluationStage?> GetByIdAsync(Guid id)
        {
            var stage = await _unitOfWork.GetEvaluationStageRepository().GetByIdAsync<Guid>(id);
            return stage?.Adapt<RS_EvaluationStage>();
        }

        public async Task<List<RS_EvaluationStage>> GetListByEvaluationIdAsync(Guid evaluationId)
        {
            var stages = await _unitOfWork.GetEvaluationStageRepository()
                .GetListAsync(e => e.EvaluationId == evaluationId, hasTrackings: false);
            return stages.Adapt<List<RS_EvaluationStage>>();
        }

        public async Task<RS_EvaluationStage> CreateAsync(RQ_EvaluationStage request)
        {
            var entity = request.Adapt<EvaluationStage>();
            entity.Id = Guid.NewGuid();
            entity.Status = "created";

            await _unitOfWork.GetEvaluationStageRepository().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.Adapt<RS_EvaluationStage>();
        }

        public async Task<RS_EvaluationStage?> UpdateAsync(Guid id, RQ_EvaluationStage request)
        {
            var repo = _unitOfWork.GetEvaluationStageRepository();
            var stage = await repo.GetByIdAsync<Guid>(id);
            if (stage == null) return null;

            request.Adapt(stage);
            await repo.UpdateAsync(stage);
            await _unitOfWork.SaveChangesAsync();
            return stage.Adapt<RS_EvaluationStage>();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var repo = _unitOfWork.GetEvaluationStageRepository();
            var stage = await repo.GetByIdAsync<Guid>(id);
            if (stage == null) return false;

            await repo.DeleteAsync(stage);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }

}
