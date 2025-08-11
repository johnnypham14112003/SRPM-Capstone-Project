using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Services.BusinessModels;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.RequestModels.Query;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Extensions.Exceptions;
using SRPM_Services.Extensions.MicrosoftBackgroundService;
using SRPM_Services.Extensions.OpenAI;
using SRPM_Services.Interfaces;

namespace SRPM_Services.Implements;

public class AppraisalCouncilService : IAppraisalCouncilService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEvaluationService _evaluationService;
    private readonly ITaskQueueHandler _taskQueueHandler;

    public AppraisalCouncilService(IUnitOfWork unitOfWork, IEvaluationService evaluationService, ITaskQueueHandler taskQueueHandler)
    {
        _unitOfWork = unitOfWork;
        _evaluationService = evaluationService;
        _taskQueueHandler = taskQueueHandler;

    }

    //=============================================================================
    public async Task<(bool result, Guid CoucilId)> NewAppraisal(RQ_AppraisalCouncil inputData)
    {
        //Check Null Data
        bool hasInvalidFields = new[] { inputData.Code, inputData.Name }
        .Any(string.IsNullOrWhiteSpace);
        if (hasInvalidFields) throw new BadRequestException("Council code,name cannot be empty!");

        var existCouncil = await _unitOfWork.GetAppraisalCouncilRepository().GetOneAsync
            (apc => apc.Code.Equals(inputData.Code), null, false);

        if (existCouncil is not null)
            throw new ConflictException("This Council Code is existed!");

        AppraisalCouncil appraisalCouncilDTO = inputData.Adapt<AppraisalCouncil>();
        await _unitOfWork.GetAppraisalCouncilRepository().AddAsync(appraisalCouncilDTO);
        var resultSave = await _unitOfWork.GetAppraisalCouncilRepository().SaveChangeAsync();
        return (resultSave, appraisalCouncilDTO.Id);
    }

    public async Task<PagingResult<RS_AppraisalCouncil>?> ListCouncil(Q_AppraisalCouncil queryInput)
    {
        //Re-assign value if it smaller than 1
        queryInput.PageIndex = queryInput.PageIndex < 1 ? 1 : queryInput.PageIndex;
        queryInput.PageSize = queryInput.PageSize < 1 ? 1 : queryInput.PageSize;

        var dataResult = await _unitOfWork.GetAppraisalCouncilRepository().ListPaging
            (queryInput.KeyWord, queryInput.FromDate, queryInput.ToDate,
            queryInput.SortBy, queryInput.Status, queryInput.PageIndex, queryInput.PageSize);

        // Checking Result
        if (dataResult.listCouncil is null || dataResult.listCouncil.Count == 0)
            throw new NotFoundException("Not Found Any Council!");

        return new PagingResult<RS_AppraisalCouncil>
        {
            PageIndex = queryInput.PageIndex,
            PageSize = queryInput.PageSize,
            TotalCount = dataResult.totalCount,
            DataList = dataResult.listCouncil.Adapt<List<RS_AppraisalCouncil>>()
        };
    }

    public async Task<RS_AppraisalCouncil> ViewDetailCouncil(Guid? id, byte includeNum = 0)
    {
        if (id is null || id == Guid.Empty) throw new BadRequestException("Cannot view a null council Id!");
        var existCouncil = await _unitOfWork.GetAppraisalCouncilRepository().GetDetailWithInclude((Guid)id, includeNum)
            ?? throw new NotFoundException("Not found this Council Id!");

        return existCouncil.Adapt<RS_AppraisalCouncil>();
    }

    public async Task<bool> UpdateCouncilInfo(RQ_AppraisalCouncil newCouncil)
    {
        var existCouncil = await _unitOfWork.GetAppraisalCouncilRepository().GetOneAsync(ac => ac.Id == newCouncil.Id)
            ?? throw new NotFoundException("Not found any Council match this Id!");

        //If not change -> true
        var tempCouncil = newCouncil.Adapt<AppraisalCouncil>();
        if (_unitOfWork.GetAppraisalCouncilRepository().HasChanges(tempCouncil, existCouncil) == false)
            return true;

        //Transfer new Data to old Data
        newCouncil.Adapt(existCouncil);
        await _unitOfWork.GetAppraisalCouncilRepository().UpdateAsync(existCouncil);
        return await _unitOfWork.GetAppraisalCouncilRepository().SaveChangeAsync();
    }

    public async Task<bool> DeleteCouncil(Guid id)
    {
        var existCouncil = await _unitOfWork.GetAppraisalCouncilRepository().GetOneAsync(ac => ac.Id == id)
            ?? throw new NotFoundException("Not found any Council match this Id!");

        //Remove reference Key
        //...
        //await _unitOfWork.GetAppraisalCouncilRepository().DeleteAsync(existCouncil);

        existCouncil.Status = "deleted";
        return await _unitOfWork.GetAppraisalCouncilRepository().SaveChangeAsync();
    }

    public async Task<RS_AppraisalCouncil?> GetCouncilInEvaluationAsync(Guid projectId)
    {
        //Get the source project
        var appraisalCouncil = await _unitOfWork.GetAppraisalCouncilRepository()
        .GetCouncilBelongToProject(projectId);

        return appraisalCouncil.Adapt<RS_AppraisalCouncil?>();
    }

    public async Task<string> AssignCouncilToClonedStages(Guid sourceProjectId, Guid appraisalCouncilId)
    {
        //Return backgroundTaskId
        return await System.Threading.Tasks.Task.FromResult(

            //Wrap code logic need to run in a queue
            _taskQueueHandler.EnqueueTracked(async (serviceProvider, cancelToken, progress) =>
            {
                //Get new Scope life time serperate from constructor scope
                var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();

                //===========================[ Use new scope to do task ]===========================
                // Step 1: Validate council exists
                var council = await unitOfWork.GetAppraisalCouncilRepository()
                .GetOneAsync(c => c.Id == appraisalCouncilId)
                ?? throw new NotFoundException("Appraisal Council not found.");

                // Step 2: Find cloned projects
                var sourceProject = await unitOfWork.GetProjectRepository()
                    .GetOneAsync(p => p.Id == sourceProjectId && (p.Genre == "propose" || p.Genre == "normal"), hasTrackings: true)
                    ?? throw new NotFoundException("Source project not found.");

                var clonedProjects = await unitOfWork.GetProjectRepository()
                    .GetListAsync(p =>
                        p.Genre == "proposal" &&
                        (p.Status == "approved" || p.Status == "submitted" || p.Status == "inprogress") &&
                        p.Code == sourceProject.Code,
                        hasTrackings: true // We need tracking to update entities
                    );

                if (clonedProjects == null || !clonedProjects.Any())
                    throw new NotFoundException("No cloned projects found for the given source project.");

                // Step 3: Loop through each project and assign council to its evaluation stages
                foreach (var project in clonedProjects)
                {
                    var existCouncil = await GetCouncilInEvaluationAsync(project.Id);

                    //If don't have, then create
                    if (existCouncil is null)
                    {
                        var listEvaOfProjectWithoutCouncil = await unitOfWork.GetEvaluationRepository()
                        .GetListAdvanceAsync(
                            e => e.ProjectId == project.Id && e.AppraisalCouncilId == null,
                            q => q.Include(ei => ei.EvaluationStages), true);

                        //If no other eva of project -> create new
                        if (listEvaOfProjectWithoutCouncil is null || listEvaOfProjectWithoutCouncil.Count == 0)
                        {
                            var evaluation = await _evaluationService.CreateAsync(new RQ_Evaluation
                            {
                                Title = "Evaluation With Appraisal Council",
                                ProjectId = project.Id,
                                AppraisalCouncilId = appraisalCouncilId
                            });
                        }
                        else
                        {//Other exist eva don't have council
                            foreach (var eva in listEvaOfProjectWithoutCouncil)
                            {
                                eva.AppraisalCouncilId = appraisalCouncilId;

                                //Then if eva have stage which don't have council -> assign council to each stage
                                var stagesWithoutCouncil = eva.EvaluationStages?
                                    .Where(stage => stage.AppraisalCouncilId == null)
                                    .ToList();

                                if (stagesWithoutCouncil is not null && stagesWithoutCouncil.Count != 0)
                                {
                                    foreach (var stage in stagesWithoutCouncil)
                                    {
                                        stage.AppraisalCouncilId = appraisalCouncilId;
                                    }

                                }
                            }
                        }
                    }
                }

                // Step 4: Save changes
                await unitOfWork.SaveChangesAsync();
            }));
    }
}