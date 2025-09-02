using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Services.BusinessModels;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.RequestModels.Query;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Extensions.Enumerables;
using SRPM_Services.Extensions.Exceptions;
using SRPM_Services.Interfaces;

namespace SRPM_Services.Implements;

public class AppraisalCouncilService : IAppraisalCouncilService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEvaluationService _evaluationService;
    private readonly IUserContextService _userContextService;
    public AppraisalCouncilService(IUnitOfWork unitOfWork, IEvaluationService evaluationService, IUserContextService userContextService)
    {
        _unitOfWork = unitOfWork;
        _evaluationService = evaluationService;
        _userContextService = userContextService;
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

    public async Task<List<RS_ProjectsOfCouncil>?> GetProjectsFromCouncilAsync(Guid councilId)
    {
        var listPro = await _unitOfWork.GetAppraisalCouncilRepository().GetProjectOfCouncil(councilId);

        if (!string.IsNullOrWhiteSpace(listPro.error)) throw new NotFoundException(listPro.error);

        var result = listPro.srcProject?.Select(p =>
        {
            var mapped = p.Adapt<RS_ProjectsOfCouncil>();

            mapped.Proposals = listPro.proposals?
                .Where(x => x.Code == p.Code && x.Genre == "proposal")
                .Adapt<List<RS_Project>>() ?? new List<RS_Project>();

            return mapped;
        }).ToList();

        return result;
    }
    public async Task<List<RS_Project>> GetProposalsFromCouncilAsync(Guid councilId)
    {
        var listPro = await _unitOfWork
            .GetAppraisalCouncilRepository()
            .GetProjectOfCouncil(councilId);

        if (!string.IsNullOrWhiteSpace(listPro.error))
            throw new NotFoundException(listPro.error);

        var proposals = listPro.proposals?
            .Where(x => x.Genre == "proposal")
            .Adapt<List<RS_Project>>()    
            ?? new List<RS_Project>();

        return proposals;
    }
    public async Task<RS_AppraisalCouncil?> GetCouncilInEvaluationAsync(Guid projectId, int? stageOrder)
    {
        var result = await _unitOfWork.GetAppraisalCouncilRepository()
        .GetCouncilBelongToProject(projectId, stageOrder);

        if (result.council is null) throw new NotFoundException(result.error);

        return result.council.Adapt<RS_AppraisalCouncil?>();
    }

    public async Task<bool> AssignCouncilToClonedStages(Guid sourceProjectId, Guid appraisalCouncilId)
    {
        try
        {
            // Step 1: Validate council exists
            var council = await _unitOfWork.GetAppraisalCouncilRepository()
                .GetOneAsync(c => c.Id == appraisalCouncilId)
                ?? throw new NotFoundException("Appraisal Council not found.");

            // Step 2: Find cloned projects
            var sourceProject = await _unitOfWork.GetProjectRepository()
                .GetOneAsync(p => p.Id == sourceProjectId && (p.Genre == "propose" || p.Genre == "normal"), hasTrackings: true)
                ?? throw new NotFoundException("Source project not found.");

            var clonedProjects = await _unitOfWork.GetProjectRepository()
                .GetListAsync(p =>
                    p.Genre == "proposal" &&
                    (p.Status == "approved" || p.Status == "submitted" || p.Status == "inprogress") &&
                    p.Code == sourceProject.Code,
                    hasTrackings: true // We need tracking to update entities
                );

            if (clonedProjects == null || !clonedProjects.Any())
                throw new NotFoundException("No cloned projects found for the given source project.");

            // Step 3: Loop through each project and assign council to first stage of evaluations
            foreach (var project in clonedProjects)
            {
                var resultCouncil = await _unitOfWork.GetAppraisalCouncilRepository()
                .GetCouncilBelongToProject(project.Id);

                //If don't have, then create
                if (resultCouncil.council is null)
                {
                    var listEvaOfProjectWithoutCouncil = await _unitOfWork.GetEvaluationRepository()
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
                            AppraisalCouncilId = null
                        });
                    }
                    else
                    {//Other exist eva don't have council
                        foreach (var eva in listEvaOfProjectWithoutCouncil)
                        {

                            //Find only the first stage (Outline Approval) and assign council
                            var firstStage = eva.EvaluationStages?
                                .Where(stage =>
                                    stage.Name == "Outline Approval" &&
                                    stage.StageOrder == 1 &&
                                    stage.Type == "project" &&
                                    stage.Status == "created" &&
                                    stage.AppraisalCouncilId == null)
                                .FirstOrDefault();

                            if (firstStage is not null)
                            {
                                firstStage.AppraisalCouncilId = appraisalCouncilId;
                            }
                        }
                    }
                }
            }

            // Step 4: Save changes
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch (NotFoundException)
        {
            throw; // Re-throw to handle in controller
        }
        catch (Exception ex)
        {
            throw new BadRequestException($"Failed to assign Appraisal Council: {ex.Message}");
        }
    }
    public async Task<PagingResult<RS_AppraisalCouncil>> GetAllOnlineUserAppraisalCouncilAsync(int pageIndex, int pageSize)
    {
        var accountId = Guid.Parse(_userContextService.GetCurrentUserId());
        var currentRoleName = _userContextService.GetCurrentUserRole();

        // Fetch all UserRoles for current user with Role included
        var userRoles = await _unitOfWork.GetUserRoleRepository()
            .GetListAsync(
                ur => ur.AccountId == accountId && ur.Status.ToLower() != Status.Deleted.ToString().ToLower() ,
                include: q => q.Include(ur => ur.Role)
            );

        // Filter roles by matching role name and ensure AppraisalCouncilId is present
        var matchingRoles = userRoles!
            .Where(ur => ur.Role != null && ur.Role.Name == currentRoleName && ur.AppraisalCouncilId.HasValue)
            .ToList();

        var appraisalCouncilIds = matchingRoles
            .Select(ur => ur.AppraisalCouncilId!.Value)
            .Distinct()
            .ToList();

        if (!appraisalCouncilIds.Any())
        {
            return new PagingResult<RS_AppraisalCouncil>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = 0,
                DataList = []
            };
        }

        // Fetch AppraisalCouncils by IDs
        var councils = await _unitOfWork.GetAppraisalCouncilRepository()
            .GetListAsync(
                ac => appraisalCouncilIds.Contains(ac.Id),
                hasTrackings: false
            );

        var totalCount = councils!.Count;

        var pagedCouncils = councils
            .OrderByDescending(ac => ac.CreatedAt)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Adapt<List<RS_AppraisalCouncil>>();

        return new PagingResult<RS_AppraisalCouncil>
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalCount = totalCount,
            DataList = pagedCouncils
        };
    }
}