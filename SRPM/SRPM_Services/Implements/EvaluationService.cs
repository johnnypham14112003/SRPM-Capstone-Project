using Mapster;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Services.BusinessModels;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.RequestModels.Query;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Extensions.Exceptions;
using SRPM_Services.Interfaces;

namespace SRPM_Services.Implements;

public class EvaluationService : IEvaluationService
{
    private readonly IUnitOfWork _unitOfWork;

    public EvaluationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    //=============================================================================
    public async Task<RS_Evaluation?> ViewDetail(Guid id, byte includeNum)
    {
        if (id == Guid.Empty) throw new BadRequestException("Cannot view a null Evaluation Id!");

        var existEvaluation = await _unitOfWork.GetEvaluationRepository().GetDetailWithInclude(id, includeNum)
            ?? throw new NotFoundException("Not found this Evaluation Id!");

        return existEvaluation.Adapt<RS_Evaluation>();
    }

    public async Task<PagingResult<RS_Evaluation>> GetListAsync(Q_Evaluation queryInput)
    {
        //Re-assign value if it smaller than 1
        queryInput.PageIndex = queryInput.PageIndex < 1 ? 1 : queryInput.PageIndex;
        queryInput.PageSize = queryInput.PageSize < 1 ? 1 : queryInput.PageSize;

        var dataResult = await _unitOfWork.GetEvaluationRepository().ListPaging
            (queryInput.KeyWord, queryInput.Phrase,queryInput.Type, queryInput.Status,
            queryInput.FromDate, queryInput.ToDate, queryInput.Rating,
            queryInput.SortBy, queryInput.PageIndex, queryInput.PageSize);

        // Checking Result
        if (dataResult.listEvaluation is null || dataResult.listEvaluation.Count == 0)
            throw new NotFoundException("Not Found Any Evaluation!");

        return new PagingResult<RS_Evaluation>
        {
            PageIndex = queryInput.PageIndex,
            PageSize = queryInput.PageSize,
            TotalCount = dataResult.totalFound,
            DataList = dataResult.listEvaluation.Adapt<List<RS_Evaluation>>()
        };
    }

    public async Task<(bool success, Guid evaluationId)> CreateAsync(RQ_Evaluation newEvaluation)
    {
        //Identify project or milestone
        bool isProject = newEvaluation.Type.Equals("project", StringComparison.OrdinalIgnoreCase);
        if (newEvaluation.ProjectId is null && newEvaluation.MilestoneId is null)
            throw new NotFoundException("Cannot create Evaluation of null Project and MileStone");

        Project? project = default;
        Milestone? milestone = default;

        if (isProject)
        { project = await _unitOfWork.GetProjectRepository().GetByIdAsync(newEvaluation.ProjectId); }
        else
        { milestone = await _unitOfWork.GetMilestoneRepository().GetByIdAsync(newEvaluation.MilestoneId); }

        var prefix = isProject ? "PE" : "ME";
        var datePart = DateTime.Now.ToString("ddMMyyyy");
        var codePart = isProject ? project!.Code.Substring(0, 4) : milestone!.Code.Substring(0, 6);
        var uniquePart = Guid.NewGuid();//new instance Guid.ToString("N") : size = 32

        //PE 13072025 0f8f 28950e
        var formatCode = $"{prefix}{datePart}{codePart}{uniquePart.ToString("N").Substring(26, 6)}";

        var evaluationDTO = newEvaluation.Adapt<Evaluation>();
        evaluationDTO.Code = formatCode;

        await _unitOfWork.GetEvaluationRepository().AddAsync(evaluationDTO);
        var resultSave = await _unitOfWork.GetEvaluationRepository().SaveChangeAsync();
        return (resultSave, evaluationDTO.Id);
    }

    public async Task<bool> UpdateAsync(RQ_Evaluation newEvaluation)
    {
        var existEvaluation = await _unitOfWork.GetEvaluationRepository().GetOneAsync(e => e.Id == newEvaluation.Id)
            ?? throw new NotFoundException("Not found any Evaluation match this Id!");

        //Transfer new Data to old Data
        newEvaluation.Adapt(existEvaluation);
        return await _unitOfWork.GetEvaluationRepository().SaveChangeAsync();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existEvaluation = await _unitOfWork.GetEvaluationRepository().GetOneAsync(e => e.Id == id)
            ?? throw new NotFoundException("Not found any Evaluation match this Id!");

        //Remove reference Key
        var relateDocs = await _unitOfWork.GetDocumentRepository().GetListAsync(d => d.EvaluationId == id);
        if (relateDocs is not null) await _unitOfWork.GetDocumentRepository().DeleteRangeAsync(relateDocs);

        var relateStage = await _unitOfWork.GetEvaluationStageRepository().GetListAsync(es => es.EvaluationId == id);
        if (relateStage is not null) await _unitOfWork.GetEvaluationStageRepository().DeleteRangeAsync(relateStage);

        var relateNotis = await _unitOfWork.GetNotificationRepository().GetListAsync(n => n.EvaluationId == id);
        if (relateNotis is not null) await _unitOfWork.GetNotificationRepository().DeleteRangeAsync(relateNotis);
        //...

        await _unitOfWork.GetEvaluationRepository().DeleteAsync(existEvaluation);
        //existEvaluation.Status = "deleted";
        return await _unitOfWork.SaveChangesAsync();
    }
}
