using Mapster;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Extensions.Enumerables;
using SRPM_Services.Interfaces;

namespace SRPM_Services.Implements;

public class IndividualEvaluationService : IIndividualEvaluationService
{
    private readonly IUnitOfWork _unitOfWork;

    public IndividualEvaluationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<RS_IndividualEvaluation?> GetByIdAsync(Guid id)
    {
        var item = await _unitOfWork.GetIndividualEvaluationRepository().GetByIdAsync<Guid>(id);
        return item?.Adapt<RS_IndividualEvaluation>();
    }

    public async Task<List<RS_IndividualEvaluation>> GetListByStageAsync(Guid evaluationStageId)
    {
        var items = await _unitOfWork.GetIndividualEvaluationRepository()
            .GetListAsync(i => i.EvaluationStageId == evaluationStageId, hasTrackings: false);

        return items.Adapt<List<RS_IndividualEvaluation>>();
    }

    public async Task<RS_IndividualEvaluation> CreateAsync(RQ_IndividualEvaluation request)
    {
        var entity = request.Adapt<IndividualEvaluation>();
        entity.Id = Guid.NewGuid();
        entity.SubmittedAt = DateTime.Now;
        entity.Status = Status.Created.ToString().ToLowerInvariant();

        await _unitOfWork.GetIndividualEvaluationRepository().AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return entity.Adapt<RS_IndividualEvaluation>();
    }

    public async Task<RS_IndividualEvaluation?> UpdateAsync(Guid id, RQ_IndividualEvaluation request)
    {
        var repo = _unitOfWork.GetIndividualEvaluationRepository();
        var item = await repo.GetByIdAsync<Guid>(id);
        if (item == null) return null;

        request.Adapt(item); // updates only provided fields
        await repo.UpdateAsync(item);
        await _unitOfWork.SaveChangesAsync();

        return item.Adapt<RS_IndividualEvaluation>();
    }
}