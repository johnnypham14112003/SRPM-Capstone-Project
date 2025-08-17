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
    private readonly IUserContextService _userContextService;

    public IndividualEvaluationService(IUnitOfWork unitOfWork, IUserContextService userContextService)
    {
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    //=============================================================================
    public async Task<RS_IndividualEvaluation?> ViewDetail(Guid id)
    {
        if (id == Guid.Empty)
            throw new BadRequestException("Cannot view a null IndividualEvaluation Id!");

        var list = await _unitOfWork.GetIndividualEvaluationRepository()
            .GetListAsync(
                 ie => ie.Id == id,
                include: ie =>
                {
                    ie = ie.Include(a => a.Reviewer).ThenInclude(b => b.Account);
                    ie = ie.Include(a => a.EvaluationStage);
                    ie = ie.Include(a => a.Documents);
                    ie = ie.Include(c => c.ProjectsSimilarity).ThenInclude(d => d.Project);
                    return ie;
                },
                hasTrackings: false
            );

        var existIndividualEvaluation = list.FirstOrDefault()
            ?? throw new NotFoundException($"Not found this IndividualEvaluation Id: '{id}'!");

        var individualEva = existIndividualEvaluation.Adapt<RS_IndividualEvaluation>();

        individualEva.ProjectsSimilarityResult = existIndividualEvaluation.ProjectsSimilarity?
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
        var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());
        var existEvaluationStage = await _unitOfWork.GetEvaluationStageRepository()
            .GetOneAsync(ie => ie.Id == newIndividualEvaluation.EvaluationStageId)
            ?? throw new NotFoundException("This EvaluationStageId is not exist to create its IndividualEvaluation");
        var existUserRole = await _unitOfWork.GetUserRoleRepository()
            .GetOneAsync(ur => ur.AppraisalCouncilId == existEvaluationStage.AppraisalCouncilId && ur.AccountId == currentUserId);
        //default get by current session || use id on parameter
        Guid userRoleId = newIndividualEvaluation.ReviewerId is null ? existUserRole!.Id : existUserRole!.Id;
        if (userRoleId == Guid.Empty)
            throw new BadRequestException("Unknown Who Is Create This Evaluation!");
        newIndividualEvaluation.ReviewerId = userRoleId;
        //Check name
        if (string.IsNullOrWhiteSpace(newIndividualEvaluation.Name))
            throw new BadRequestException("Cannot create a null tilte name of individual evaluation");

        //AI or Person
        if (newIndividualEvaluation.IsAIReport == false && newIndividualEvaluation.ReviewerId is null)
            throw new BadRequestException("Must be created by a specific person if not by AI");

        IndividualEvaluation individualEvaluationDTO = newIndividualEvaluation.Adapt<IndividualEvaluation>();
        individualEvaluationDTO.ReviewerId = userRoleId;
        await _unitOfWork.GetIndividualEvaluationRepository().AddAsync(individualEvaluationDTO);
        var resultSave = await _unitOfWork.GetIndividualEvaluationRepository().SaveChangeAsync();
        return (resultSave, individualEvaluationDTO.Id);
    }

    public async Task<bool> UpdateAsync(RQ_IndividualEvaluation newIndividualEvaluation)
    {
        var existIndividualEvaluation = await _unitOfWork.GetIndividualEvaluationRepository()
            .GetOneAsync(oe => oe.Id == newIndividualEvaluation.Id, include: q => q.Include(q => q.EvaluationStage))
            ?? throw new NotFoundException("Not found any IndividualEvaluation match this Id!");

        var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());

        // Validate name
        if (string.IsNullOrWhiteSpace(newIndividualEvaluation.Name))
            throw new BadRequestException("Cannot update with a null title name of individual evaluation");

        // Resolve reviewer ID
        var existUserRole = await _unitOfWork.GetUserRoleRepository()
            .GetOneAsync(ur => ur.AppraisalCouncilId == existIndividualEvaluation.EvaluationStage.AppraisalCouncilId && ur.AccountId == currentUserId);

        Guid userRoleId = newIndividualEvaluation.ReviewerId ?? existUserRole?.Id ?? Guid.Empty;
        if (userRoleId == Guid.Empty)
            throw new BadRequestException("Unknown who is updating this evaluation!");

        newIndividualEvaluation.ReviewerId = userRoleId;
        newIndividualEvaluation.EvaluationStageId = existIndividualEvaluation.EvaluationStageId;
        if (newIndividualEvaluation.IsAIReport == true && newIndividualEvaluation.ReviewerId is null)
            throw new BadRequestException("Must be updated by a specific person if not by AI");

        // Transfer new data
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

    private async Task<Guid> GetCurrentMainUserRoleId()
    {
        _ = Guid.TryParse(_userContextService.GetCurrentUserId(), out Guid accId);

        var existAccount = await _unitOfWork.GetAccountRepository().GetOneAsync(a => a.Id == accId, null, false) ??
            throw new NotFoundException("Your current session account Id is not exist in database! Can't find your cv");

        var defaultUserRole = await _unitOfWork.GetUserRoleRepository().GetOneAsync(ur =>
            ur.AccountId == accId &&
            ur.ProjectId == null &&
            ur.AppraisalCouncilId == null, null, false) ??
        throw new NotFoundException("Not found your base role Id or it is expired in system!");

        return defaultUserRole.Id;
    }
}