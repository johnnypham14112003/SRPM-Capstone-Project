using Mapster;
using Microsoft.EntityFrameworkCore;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Services.BusinessModels;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.RequestModels.Query;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Extensions.Exceptions;

using SRPM_Services.Interfaces;

namespace SRPM_Services.Implements;

public class IndividualEvaluationService : IIndividualEvaluationService
{
    private readonly IUnitOfWork _unitOfWork;

    public IndividualEvaluationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    //=============================================================================
    public async Task<RS_IndividualEvaluation?> ViewDetail(Guid id)
    {
        if (id == Guid.Empty) throw new BadRequestException("Cannot view a null IndividualEvaluation Id!");

        var existIndividualEvaluation = await _unitOfWork.GetIndividualEvaluationRepository()
            .GetOneAsync(ie => ie.Id == id,
            ie =>
            {
                ie.Include(iei => iei.Documents);
                ie.Include(iei => iei.ProjectsSimilarity)
                    .ThenInclude(ps => ps.Project)
                    .AsSplitQuery();
                return ie;
            }, false)
            ?? throw new NotFoundException($"Not found this IndividualEvaluation Id: '{id}'!");

        //Main mapping
        var individualEva = existIndividualEvaluation.Adapt<RS_IndividualEvaluation>();

        //ProjectsSimilarity => ProjectsSimilarityResult
        individualEva.ProjectsSimilarityResult = existIndividualEvaluation.ProjectsSimilarity?//if not null -> select
        .Select(ps => new RS_ProjectSimilarityResult
        {
            Id = ps.ProjectId,
            EnglishTitle = ps.Project.EnglishTitle,
            Similarity = ps.Similarity
        }).ToList();
        return individualEva;
    }

    public async Task<PagingResult<RS_IndividualEvaluation>> GetListAsync(Q_IndividualEvaluation queryInput)
    {
        //Re-assign value if it smaller than 1
        queryInput.PageIndex = queryInput.PageIndex < 1 ? 1 : queryInput.PageIndex;
        queryInput.PageSize = queryInput.PageSize < 1 ? 1 : queryInput.PageSize;

        var dataResult = await _unitOfWork.GetIndividualEvaluationRepository().GetListPagingAsync
            (queryInput.KeyWord, queryInput.TotalRate, queryInput.FromDate, queryInput.ToDate,
            queryInput.IsApproved, queryInput.ReviewerResult, queryInput.IsAIReport,
            queryInput.Status, queryInput.EvaluationStageId, queryInput.ReviewerId,
            queryInput.SortBy, queryInput.PageIndex, queryInput.PageSize);

        // Checking Result
        if (dataResult.listIndividualEvaluations is null || dataResult.listIndividualEvaluations.Count == 0)
            throw new NotFoundException("Not Found Any Evaluation!");

        return new PagingResult<RS_IndividualEvaluation>
        {
            PageIndex = queryInput.PageIndex,
            PageSize = queryInput.PageSize,
            TotalCount = dataResult.totalFound,
            DataList = dataResult.listIndividualEvaluations.Adapt<List<RS_IndividualEvaluation>>()
        };
    }

    public async Task<(bool success, Guid individualEvaluationId)> CreateAsync(RQ_IndividualEvaluation newIndividualEvaluation)
    {
        //Check available EvaluationStageId
        var existEvaluationStage = await _unitOfWork.GetEvaluationStageRepository()
            .GetByIdAsync(newIndividualEvaluation.EvaluationStageId)
            ?? throw new NotFoundException("This EvaluationStageId is not exist to create its IndividualEvaluation");

        //Check name
        if (string.IsNullOrWhiteSpace(newIndividualEvaluation.Name))
            throw new BadRequestException("Cannot create a null tilte name of individual evaluation");

        //AI or Person
        if (newIndividualEvaluation.IsAIReport == false && newIndividualEvaluation.ReviewerId is null)
            throw new BadRequestException("Must be created by a specific person if not by AI");

        IndividualEvaluation individualEvaluationDTO = newIndividualEvaluation.Adapt<IndividualEvaluation>();
        await _unitOfWork.GetIndividualEvaluationRepository().AddAsync(individualEvaluationDTO);
        var resultSave = await _unitOfWork.GetIndividualEvaluationRepository().SaveChangeAsync();
        return (resultSave, individualEvaluationDTO.Id);
    }

    public async Task<bool> UpdateAsync(RQ_IndividualEvaluation newIndividualEvaluation)
    {
        var existIndividualEvaluation = await _unitOfWork.GetIndividualEvaluationRepository().GetByIdAsync(newIndividualEvaluation.Id)
            ?? throw new NotFoundException("Not found any IndividualEvaluation match this Id!");

        //Check name
        if (string.IsNullOrWhiteSpace(newIndividualEvaluation.Name))
            throw new BadRequestException("Cannot create a null tilte name of individual evaluation");

        //AI or Person
        if (newIndividualEvaluation.IsAIReport == false && newIndividualEvaluation.ReviewerId is null)
            throw new BadRequestException("Must be created by a specific person if not by AI");

        //Transfer new Data to old Data
        newIndividualEvaluation.Adapt(existIndividualEvaluation);
        return await _unitOfWork.GetEvaluationRepository().SaveChangeAsync();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existIndividualEvaluation = await _unitOfWork.GetIndividualEvaluationRepository().GetByIdAsync(id)
            ?? throw new NotFoundException("Not found any IndividualEvaluation match this Id!");

        //Remove reference Key
        var relateDocs = await _unitOfWork.GetDocumentRepository().GetListAsync(d => d.IndividualEvaluationId == id);
        if (relateDocs is not null) await _unitOfWork.GetDocumentRepository().DeleteRangeAsync(relateDocs);

        var relateNotis = await _unitOfWork.GetNotificationRepository().GetListAsync(n => n.IndividualEvaluationId == id);
        if (relateNotis is not null) await _unitOfWork.GetNotificationRepository().DeleteRangeAsync(relateNotis);

        var relateProjectSimilarity = await _unitOfWork.GetProjectSimilarityRepository().GetListAsync(ps => ps.IndividualEvaluationId == id);
        if (relateProjectSimilarity is not null) await _unitOfWork.GetProjectSimilarityRepository().DeleteRangeAsync(relateProjectSimilarity);

        await _unitOfWork.GetIndividualEvaluationRepository().DeleteAsync(existIndividualEvaluation);
        //existEvaluation.Status = "deleted";
        return await _unitOfWork.SaveChangesAsync();
    }
}