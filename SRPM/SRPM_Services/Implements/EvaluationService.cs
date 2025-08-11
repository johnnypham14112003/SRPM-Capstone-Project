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
using SRPM_Services.Extensions.MicrosoftBackgroundService;
using SRPM_Services.Extensions.OpenAI;
using SRPM_Services.Interfaces;

namespace SRPM_Services.Implements;

public class EvaluationService : IEvaluationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOpenAIService _openAIService;
    private readonly ITaskQueueHandler _taskQueueHandler;

    public EvaluationService(IUnitOfWork unitOfWork, IOpenAIService openAIService, ITaskQueueHandler taskQueueHandler)
    {
        _unitOfWork = unitOfWork;
        _openAIService = openAIService;
        _taskQueueHandler = taskQueueHandler;
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
            (queryInput.KeyWord, queryInput.Status,
            queryInput.FromDate, queryInput.ToDate, queryInput.Rating,
            queryInput.ProjectId, queryInput.AppraisalCouncilId,
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
        var project = await _unitOfWork.GetProjectRepository().GetByIdAsync(newEvaluation.ProjectId)
            ?? throw new NotFoundException($"Not Found Project Id '{newEvaluation.ProjectId}' to create evaluation!");

        var datePart = DateTime.Now.ToString("ddMMyyyy");
        var abbreviations = project.Abbreviations;

        //EVA-SRPM13072025
        var formatCode = $"EVA-{abbreviations}{datePart}";

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

    public async Task<string> FirstAIEvaluation(Guid projectId)
    {
        //Return backgroundTaskI
        return await System.Threading.Tasks.Task.FromResult(

            //Wrap code logic need to run in a queue
            _taskQueueHandler.EnqueueTracked(async (serviceProvider, cancelToken, progress) =>
            {
                //Get new Scope life time serperate from constructor scope
                var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
                var openAIService = serviceProvider.GetRequiredService<IOpenAIService>();

                //===========================[ Use new scope to do task ]===========================
                var project = await unitOfWork.GetProjectRepository().GetOneAsync(
                    p => p.Id == projectId,
                    p => p.Include(pro => pro.Milestones), false)
                ?? throw new NotFoundException($"Not Found Project Id '{projectId}' to create evaluation!");

                var datePart = DateTime.Now.ToString("ddMMyyyy");
                var abbreviations = project.Abbreviations;

                //EVA-SRPM13072025
                var formatCode = $"EVA-{abbreviations}{datePart}";

                //========================[ Create Evaluation ]========================
                var existEva = await unitOfWork.GetEvaluationRepository().GetOneAsync(
                    eva => eva.Title.Equals($"Evaluation Of {abbreviations}") &&
                    eva.ProjectId == projectId);

                //Check if exist Evaluation
                Guid evaId;
                if (existEva is null)
                {
                    evaId = Guid.NewGuid();
                    await unitOfWork.GetEvaluationRepository().AddAsync(new Evaluation
                    {
                        Id = evaId,
                        Code = formatCode,
                        Title = $"Evaluation Of {abbreviations}",
                        ProjectId = projectId
                    });
                }
                else { evaId = existEva.Id; }

                //========================[ Create Evaluation Stage ]========================
                var existEvaStage = await unitOfWork.GetEvaluationStageRepository().GetOneAsync(
                    evaS => evaS.EvaluationId == evaId &&
                    evaS.Name.Equals("Outline Approval") &&
                    evaS.Phrase.Equals("Approval"));

                //Check if exist EvaluationStage
                Guid evaStageId;
                if (existEvaStage is null)
                {
                    evaStageId = Guid.NewGuid();
                    await unitOfWork.GetEvaluationStageRepository().AddAsync(new EvaluationStage
                    {
                        Id = evaStageId,
                        Name = "Outline Approval",
                        Phrase = "Approval",
                        EvaluationId = evaId
                    });
                }
                else { evaStageId = existEvaStage.Id; }

                //========================[ Create Individual Evaluation ]========================
                var existIndividualEva = await unitOfWork.GetIndividualEvaluationRepository().GetOneAsync(
                    inEva => inEva.EvaluationStageId == evaStageId &&
                    inEva.Name.Equals("AI Review") &&
                    inEva.IsAIReport == true);

                //Check if exist EvaluationStage
                Guid inEvaId;
                IndividualEvaluation indiEva;
                if (existIndividualEva is null)
                {
                    inEvaId = Guid.NewGuid();
                    indiEva = new IndividualEvaluation
                    {
                        Id = inEvaId,
                        Name = "AI Review",
                        IsAIReport = true,
                        EvaluationStageId = evaStageId
                    };
                }
                else
                {
                    inEvaId = existIndividualEva.Id;
                    indiEva = existIndividualEva;
                }


                ////========================[ Run AI ]========================
                var projectSummary = project.Adapt<RQ_ProjectContentForAI>();
                projectSummary.MilestoneContents = project.Milestones.Adapt<ICollection<RQ_MilestoneContentForAI>>();

                //Handle result AI
                List<RS_ProjectSimilarityResult>? listSimilarity = null;
                indiEva.Comment = "The proposal's description is null to generate review!";
                if (!string.IsNullOrWhiteSpace(projectSummary.Description))
                {
                    indiEva.Comment = "This proposal's milestone is null to generate review!";
                    if (projectSummary.MilestoneContents is not null)
                    {
                        var AIResult = await AIReviewAndPlagiarism(projectSummary, cancelToken, unitOfWork, openAIService);
                        indiEva.Comment = AIResult.summaryEvaluation;
                        listSimilarity = AIResult.listSimilarity;
                    }
                }

                //Create AI Evaluation as Individual Evaluation
                await unitOfWork.GetIndividualEvaluationRepository().AddAsync(indiEva);

                //Map list AI Result to Db Model
                if (listSimilarity is not null)
                {
                    var projectSimilarities = listSimilarity
                    .Select(listResult => new ProjectSimilarity
                    {
                        IndividualEvaluationId = indiEva.Id,
                        ProjectId = listResult.Id,
                        Similarity = listResult.Similarity
                    })
                    .ToList();
                    await unitOfWork.GetProjectSimilarityRepository().AddRangeAsync(projectSimilarities);
                }
                await unitOfWork.SaveChangesAsync();
            }));
    }

    public async Task<string> RegenAIEvaluation(Guid projectId, Guid individualEvalutionId)
    {
        //Return backgroundTaskI
        return await System.Threading.Tasks.Task.FromResult(

            //Wrap code logic need to run in a queue
            _taskQueueHandler.EnqueueTracked(async (serviceProvider, cancelToken, progress) =>
            {
                //Get new Scope life time serperate from constructor scope
                var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
                var openAIService = serviceProvider.GetRequiredService<IOpenAIService>();

                //Get Project
                var project = await unitOfWork.GetProjectRepository().GetOneAsync(
                    p => p.Id == projectId,
                    p => p.Include(pro => pro.Milestones), false)
                ?? throw new NotFoundException($"Not Found Project Id '{projectId}' to create evaluation!");

                //Get Individual Evaluation
                var individualEvaluation = await unitOfWork.GetIndividualEvaluationRepository().GetOneAsync(
                    ie => ie.Id == individualEvalutionId &&
                    ie.Name.Equals("AI Review") &&
                    ie.IsAIReport == true,
                    q => q.Include(iev => iev.ProjectsSimilarity))
                ?? throw new NotFoundException($"Not Found Individual Evaluation Id '{individualEvalutionId}' to regen AI review!");

                //Prepare data before send to AI
                var projectSummary = project.Adapt<RQ_ProjectContentForAI>();
                projectSummary.MilestoneContents = project.Milestones.Adapt<ICollection<RQ_MilestoneContentForAI>>();

                //Handle result AI
                List<RS_ProjectSimilarityResult>? listSimilarity = null;
                individualEvaluation.Comment = "The proposal's description is null to review!";
                if (!string.IsNullOrWhiteSpace(projectSummary.Description))
                {
                    individualEvaluation.Comment = "This proposal's milestone is null to review!";
                    if (projectSummary.MilestoneContents is not null)
                    {
                        var AIResult = await AIReviewAndPlagiarism(projectSummary, cancelToken, unitOfWork, openAIService);
                        individualEvaluation.Comment = AIResult.summaryEvaluation;
                        listSimilarity = AIResult.listSimilarity;
                    }
                }

                //If Exist Old Review
                if (individualEvaluation.ProjectsSimilarity is not null)
                {
                    //Delete Old
                    await unitOfWork.GetProjectSimilarityRepository()
                    .DeleteRangeAsync(individualEvaluation.ProjectsSimilarity);
                }

                if (listSimilarity is not null)
                {
                    var projectSimilarities = listSimilarity
                    .Select(listResult => new ProjectSimilarity
                    {
                        IndividualEvaluationId = individualEvalutionId,
                        ProjectId = listResult.Id,
                        Similarity = listResult.Similarity
                    })
                    .ToList();
                    await unitOfWork.GetProjectSimilarityRepository().AddRangeAsync(projectSimilarities);
                    individualEvaluation.SubmittedAt = DateTime.Now;
                }
                await unitOfWork.SaveChangesAsync();
            }));
    }

    //=============================================================================================
    private async Task<(List<RS_ProjectSimilarityResult>? listSimilarity, string summaryEvaluation)>
        AIReviewAndPlagiarism(
            RQ_ProjectContentForAI projectSummary,
            CancellationToken cancelToken,
            IUnitOfWork unitOfWork,
            IOpenAIService openAIService)
    {
        List<RS_ProjectSimilarityResult>? listSimilarity = [];
        float[]? vectorDescription;
        string summaryEvaluation = "";

        //Query encoded completed Project
        var projectEncoded = await unitOfWork.GetProjectRepository().GetListAdvanceAsync(
            p => p.Status.ToLower().Equals(Status.Completed.ToString().ToLower()) && !string.IsNullOrWhiteSpace(p.EncodedDescription),
            p => new { p.Id, p.EnglishTitle, p.Description, p.EncodedDescription });

        //Query all completed Project if 'projectEncoded' is null
        List<Project>? databaseSource = projectEncoded is null ?
        await unitOfWork.GetProjectRepository().GetListAsync(p => p.Status.ToLower().Equals(Status.Completed.ToString().ToLower()))
            : projectEncoded.Adapt<List<Project>>();

        //Final Source
        //syntheticSource = projectSource + online source
        var syntheticSource = databaseSource.Adapt<List<RS_ProjectSimilarityResult>>();

        vectorDescription = await openAIService.EmbedTextAsync(projectSummary.Description!, cancelToken);

        //Overview
        summaryEvaluation = await openAIService.ReviewProjectAsync(projectSummary, cancelToken);

        //Compare Similarity
        listSimilarity = await openAIService.CompareWithSourceAsync(vectorDescription!, syntheticSource, cancelToken);


        return (listSimilarity, summaryEvaluation);
    }
}
