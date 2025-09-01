using Mapster;
using Microsoft.EntityFrameworkCore;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Implements;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Services.BusinessModels;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.RequestModels.Query;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Extensions.Exceptions;
using SRPM_Services.Interfaces;

namespace SRPM_Services.Implements;

public class EvaluationStageService : IEvaluationStageService
{
    private readonly IUnitOfWork _unitOfWork;

    public EvaluationStageService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    //=============================================================================
    public async Task<RS_EvaluationStage?> ViewDetail(Guid id, byte includeNum)
    {
        if (id == Guid.Empty) throw new BadRequestException("Cannot view a null EvaluationStage Id!");

        var existEvaluationStage = await _unitOfWork.GetEvaluationStageRepository().GetStageDetailWithInclude(id, includeNum)
            ?? throw new NotFoundException("Not found this EvaluationStage Id!");

        return existEvaluationStage.Adapt<RS_EvaluationStage>();
    }

    public async Task<PagingResult<RS_EvaluationStage>> GetListPagingAsync(Q_EvaluationStage queryInput)
    {
        //Re-assign value if it smaller than 1
        queryInput.PageIndex = queryInput.PageIndex < 1 ? 1 : queryInput.PageIndex;
        queryInput.PageSize = queryInput.PageSize < 1 ? 1 : queryInput.PageSize;

        var dataResult = await _unitOfWork.GetEvaluationStageRepository().ListStagePaging
            (queryInput.KeyWord, queryInput.Phrase, queryInput.Type, queryInput.Status,
            queryInput.MilestoneId, queryInput.EvaluationId, queryInput.AppraisalCouncilId,
            queryInput.SortBy, queryInput.PageIndex, queryInput.PageSize);

        // Checking Result
        if (dataResult.listStage is null || dataResult.listStage.Count == 0)
            throw new NotFoundException("Not Found Any EvaluationStage!");

        return new PagingResult<RS_EvaluationStage>
        {
            PageIndex = queryInput.PageIndex,
            PageSize = queryInput.PageSize,
            TotalCount = dataResult.totalFound,
            DataList = dataResult.listStage.Adapt<List<RS_EvaluationStage>>()
        };
    }

    public async Task<(bool success, Guid evaluationStageId)> CreateAsync(RQ_EvaluationStage newEvaluationStage)
    {
        //Check Null Data
        bool hasInvalidFields = new[] { newEvaluationStage.Name, newEvaluationStage.Phrase, newEvaluationStage.Type, newEvaluationStage.Status }
        .Any(string.IsNullOrWhiteSpace);
        if (hasInvalidFields) throw new BadRequestException("EvaluationStage Name or Status cannot be empty!");

        //Validate Parent "Evaluation"
        _ = await _unitOfWork.GetEvaluationRepository().GetByIdAsync(newEvaluationStage.EvaluationId)
            ?? throw new BadRequestException("This EvaluationId is not exist to create its stages");

        //Validate milestone stage of a evaluation
        if (newEvaluationStage.MilestoneId.HasValue && newEvaluationStage.Type.ToLower().Trim().Equals("milestone"))
        {
            //Validate milestone
            _ = await _unitOfWork.GetMilestoneRepository().GetOneAsync(t => t.Id == newEvaluationStage.MilestoneId, null, false)
                ?? throw new NotFoundException("Not found this Milestone to create evaluation stage!");

            //Find eva stage by eva Id and milestone Id
            var existEvaluationStage = await _unitOfWork.GetEvaluationStageRepository()
                .GetOneAsync(es =>
                es.EvaluationId == newEvaluationStage.EvaluationId &&
                es.MilestoneId == newEvaluationStage.MilestoneId);

            if (existEvaluationStage is not null)
                throw new BadRequestException("Cannot create more than 1 evaluation stage of a milestone!");
        }

        //List other stage
        var listStageExist = await _unitOfWork.GetEvaluationStageRepository()
            .GetListAsync(es => es.EvaluationId == newEvaluationStage.EvaluationId);

        //Default order || sort list
        if (listStageExist is null)
        { newEvaluationStage.StageOrder = 1; }
        else
        {
            // If RQ didn’t specify an order, append to end
            if (newEvaluationStage.StageOrder == null)
            {
                var maxOrder = listStageExist.Max(s => s.StageOrder);
                newEvaluationStage.StageOrder = maxOrder + 1;
            }

            listStageExist = listStageExist.OrderBy(s => s.StageOrder).ToList();

            // Shift Stages (>=newOrder) - loop in condition list
            foreach (var stage in listStageExist.Where(es => es.StageOrder >= newEvaluationStage.StageOrder))
            {
                stage.StageOrder++;
                await _unitOfWork.GetEvaluationStageRepository().UpdateAsync(stage);
            }
        }

        //Then add
        var evaluationStageDTO = newEvaluationStage.Adapt<EvaluationStage>();

        await _unitOfWork.GetEvaluationStageRepository().AddAsync(evaluationStageDTO);
        var resultSave = await _unitOfWork.GetEvaluationStageRepository().SaveChangeAsync();
        return (resultSave, evaluationStageDTO.Id);
    }

    public async Task<bool> UpdateAsync(RQ_EvaluationStage newEvaluationStage)
    {
        //Get evaluationStage need update
        var eStageToUpdate = await _unitOfWork.GetEvaluationStageRepository().GetByIdAsync(newEvaluationStage.Id)
            ?? throw new NotFoundException("Not found any EvaluationStage match this Id!");

        //Get other evaluationStage for shift order
        var listStageExist = await _unitOfWork.GetEvaluationStageRepository()
        .GetListAsync(es => es.EvaluationId == eStageToUpdate.EvaluationId)
        ?? throw new NotFoundException("Not found any EvaluationStage of this Evaluation to update");

        listStageExist = listStageExist.OrderBy(s => s.StageOrder).ToList();

        //Default Order if only it in an Evaluation
        if (listStageExist.Count() == 1) newEvaluationStage.StageOrder = 1;

        int oldOrder = eStageToUpdate.StageOrder;
        int newOrder = 0;
        // If RQ didn’t specify an order, append to end
        if (newEvaluationStage.StageOrder == null)
        {
            var maxOrder = listStageExist.Max(s => s.StageOrder);
            newEvaluationStage.StageOrder = maxOrder + 1;
        }
        else newOrder = (int)newEvaluationStage.StageOrder;

        //Pretend Order in array [1, 2, 3, 4, 5]
        //if new order move left, then shift all from newOrder to oldOrder -> shift right(++)
        if (newOrder < oldOrder)
        {
            foreach (var stage in listStageExist.Where(es => es.StageOrder >= newOrder && es.StageOrder < oldOrder))
            {
                stage.StageOrder++;
                await _unitOfWork.GetEvaluationStageRepository().UpdateAsync(stage);
            }
        }

        //Seperate if for case ==
        //if new order move right, then shift all from oldOrder to newOrder -> shift left(--)
        if (newOrder > oldOrder)
        {
            foreach (var stage in listStageExist.Where(es => es.StageOrder <= newOrder && es.StageOrder > oldOrder))
            {
                stage.StageOrder--;
                await _unitOfWork.GetEvaluationStageRepository().UpdateAsync(stage);
            }
        }

        //Transfer new Data to old Data
        newEvaluationStage.Adapt(eStageToUpdate);
        return await _unitOfWork.GetEvaluationStageRepository().SaveChangeAsync();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        //Get evaluationStage need delete
        var eStageToDelete = await _unitOfWork.GetEvaluationStageRepository().GetByIdAsync(id)
            ?? throw new NotFoundException("Not found any EvaluationStage match this Id!");

        //Get other evaluationStage for shift order
        var existEvaluationStages = await _unitOfWork.GetEvaluationStageRepository()
        .GetListAsync(es => es.EvaluationId == eStageToDelete.EvaluationId)
        ?? throw new NotFoundException("Not found any EvaluationStage of this Evaluation to delete");
        existEvaluationStages = existEvaluationStages.OrderBy(s => s.StageOrder).ToList();

        //Remove reference Key
        var relateTrans = await _unitOfWork.GetTransactionRepository().GetListAsync(tr => tr.EvaluationStageId == id);
        if (relateTrans is not null) await _unitOfWork.GetTransactionRepository().DeleteRangeAsync(relateTrans);

        var relateNotis = await _unitOfWork.GetNotificationRepository().GetListAsync(n => n.EvaluationStageId == id);
        if (relateNotis is not null) await _unitOfWork.GetNotificationRepository().DeleteRangeAsync(relateNotis);

        var relateIndiEvas = await _unitOfWork.GetIndividualEvaluationRepository().GetListAsync(ie => ie.EvaluationStageId == id);
        if (relateIndiEvas is not null) await _unitOfWork.GetIndividualEvaluationRepository().DeleteRangeAsync(relateIndiEvas);

        //Delete first
        await _unitOfWork.GetEvaluationStageRepository().DeleteAsync(eStageToDelete);

        //Then shift after
        foreach (var stage in existEvaluationStages.Where(es => es.StageOrder > eStageToDelete.StageOrder))
        {
            stage.StageOrder--;
            await _unitOfWork.GetEvaluationStageRepository().UpdateAsync(stage);
        }
        return await _unitOfWork.SaveChangesAsync();
    }
}
