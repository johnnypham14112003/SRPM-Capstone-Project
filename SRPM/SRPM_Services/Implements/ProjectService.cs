using Mapster;
using Microsoft.EntityFrameworkCore;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Services.BusinessModels;
using SRPM_Services.BusinessModels.Others;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Extensions.Enumerables;
using SRPM_Services.Extensions.Exceptions;
using SRPM_Services.Interfaces;
using SRPM_Services.Extensions.OpenAI;
using SRPM_Services.Extensions.BackgroundService;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;

namespace SRPM_Services.Implements;

public class ProjectService : IProjectService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;
    private readonly IOpenAIService _openAIService;
    private readonly ITaskQueueHandler _taskQueueHandler;

    public ProjectService(IUnitOfWork unitOfWork, IUserContextService userContextService, IOpenAIService openAIService, ITaskQueueHandler taskQueueHandler)
    {
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
        _openAIService = openAIService;
        _taskQueueHandler = taskQueueHandler;
    }
    public async Task<object?> GetByIdAsync(Guid id)
    {
        var userId = Guid.Parse(_userContextService.GetCurrentUserId());
        var targetStatus = Status.Approved.ToString().ToLower();

        var userRoles = await _unitOfWork.GetUserRoleRepository()
            .GetListAsync(
                us => us.AccountId == userId
                           && us.ProjectId == id
                           && us.Status.ToLower() == targetStatus,
                include: q => q.Include(ur => ur.Role)
            );

        var isMember = userRoles.Any();

        // Group roles: Leader, Secretary, etc.
        var groupRoles = userRoles
            .Where(ur => ur.Role != null && ur.Role.IsGroupRole == true)
            .Select(ur => ur.Role.Name)
            .Distinct()
            .ToList();

        // Non-group roles: Researcher, Principal, etc.
        var fallbackRoles = userRoles
            .Where(ur => ur.Role != null && ur.Role.IsGroupRole == false)
            .Select(ur => ur.Role.Name)
            .Distinct()
            .ToList();

        var roleInProject = groupRoles.Any() ? groupRoles : fallbackRoles;

        if (!isMember)
        {
            var projectOverview = await _unitOfWork.GetProjectRepository()
                .GetByIdAsync(id);

            return new
            {
                ProjectDetail = projectOverview?.Adapt<RS_ProjectOverview>(),
                isMember,
                roleInProject = new List<string>()
            };
        }

        var entity = await _unitOfWork.GetProjectRepository()
                .GetByIdAsync(id);

        return new
        {
            ProjectDetail = entity?.Adapt<RS_ProjectDetail>(),
            isMember,
            roleInProject
        };
    }


    public async Task<PagingResult<RS_Project>> GetListAsync(RQ_ProjectQuery query)
    {
        query.PageIndex = query.PageIndex < 1 ? 1 : query.PageIndex;
        query.PageSize = query.PageSize < 1 ? 10 : query.PageSize;
        var deletedStatus = Status.Deleted.ToString();
        var projects = await _unitOfWork.GetProjectRepository().GetListAsync(
           p =>
               (string.IsNullOrWhiteSpace(query.Title) ||
                   p.EnglishTitle.Contains(query.Title) ||
                   p.VietnameseTitle.Contains(query.Title)) &&
               (string.IsNullOrWhiteSpace(query.Category) || p.Category == query.Category) &&
               (string.IsNullOrWhiteSpace(query.Type) || p.Type == query.Type) &&
               (string.IsNullOrWhiteSpace(query.Genre) || p.Genre == query.Genre) &&
               (string.IsNullOrWhiteSpace(query.Language) || p.Language == query.Language) &&
               (string.IsNullOrWhiteSpace(query.Status) || p.Status == query.Status) &&
               (!query.MajorId.HasValue ||
                   p.ProjectMajors.Any(pm => pm.MajorId == query.MajorId)) &&
               (!query.FieldId.HasValue ||
                   p.ProjectMajors.Any(pm => pm.Major.FieldId == query.FieldId)) &&
               (query.TagNames == null ||
                query.TagNames.Count == 0 ||
                !query.TagNames.Any(tag => !string.IsNullOrWhiteSpace(tag)) ||
                p.ProjectTags.Any(t => query.TagNames.Any(tag => tag == t.Name && !string.IsNullOrWhiteSpace(tag)))),
    include: q =>
    {
        q = q.Include(p => p.ProjectMajors)
                .ThenInclude(pm => pm.Major)
                    .ThenInclude(m => m.Field);

        q = q.Include(p => p.ProjectTags); // always included — adjust if needed

        if (query.IncludeCreator)
            q = q.Include(p => p.Creator).ThenInclude(c => c.Role);

        if (query.IncludeMembers)
            q = q.Include(p => p.Members).ThenInclude(m => m.Account);

        if (query.IncludeMilestones)
            q = q.Include(p => p.Milestones);

        if (query.IncludeEvaluations)
            q = q.Include(p => p.Evaluations);

        if (query.IncludeProjectSimilarity)
            q = q.Include(p => p.ProjectsSimilarity);

        if (query.IncludeDocuments)
            q = q.Include(p => p.Documents);

        if (query.IncludeTransactions)
            q = q.Include(p => p.Transactions);

        return q;
    },

            hasTrackings: false
        );

        projects = query.SortBy?.ToLower() switch
        {
            "englishtitle" => query.Desc ? projects.OrderByDescending(p => p.EnglishTitle).ToList() : projects.OrderBy(p => p.EnglishTitle).ToList(),
            "vietnamesetitle" => query.Desc ? projects.OrderByDescending(p => p.VietnameseTitle).ToList() : projects.OrderBy(p => p.VietnameseTitle).ToList(),
            "createdate" => query.Desc ? projects.OrderByDescending(p => p.CreatedAt).ToList() : projects.OrderBy(p => p.CreatedAt).ToList(),
            _ => projects.OrderBy(p => p.CreatedAt).ToList()
        };

        var total = projects.Count;
        var page = projects
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        return new PagingResult<RS_Project>
        {
            PageIndex = query.PageIndex,
            PageSize = query.PageSize,
            TotalCount = total,
            DataList = page.Adapt<List<RS_Project>>()
        };
    }


    public async Task<RS_Project> CreateAsync(RQ_Project request)
    {
        var entity = request.Adapt<Project>();
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.Now;
        entity.UpdatedAt = DateTime.Now;

        var sanitizedAbbr = (entity.Abbreviations ?? "XXX").Trim().ToUpperInvariant();
        entity.Code = $"RP-{DateTime.Now:yyyy_MM}_{sanitizedAbbr}";

        // Assign defaults if needed
        entity.Budget = entity.Budget <= 0 ? 0m : entity.Budget;
        entity.Progress = entity.Progress <= 0 ? 0m : entity.Progress;
        entity.MaximumMember = entity.MaximumMember <= 0 ? 5 : entity.MaximumMember;
        entity.Language = string.IsNullOrWhiteSpace(entity.Language) ? "English" : entity.Language;

        var accountId = Guid.Parse(_userContextService.GetCurrentUserId());

        // Fetch all roles tied to the user
        var userRoles = await _unitOfWork.GetUserRoleRepository().GetListByFilterAsync(
            accountId: accountId,
            roleId: null,
            projectId: null,
            appraisalCouncilId: null,
            status: Status.Approved.ToString().ToLowerInvariant(),
            isOfficial: null
        );

        // Validate role and assign genre + creator
        var hostRole = userRoles.FirstOrDefault(r => r.Role?.Name == "Host Institution" && r.Status != Status.Deleted.ToString().ToLowerInvariant() );
        var staffRole = userRoles.FirstOrDefault(r => r.Role?.Name == "Staff");

        if (hostRole != null)
        {
            entity.Genre = "normal";
            entity.CreatorId = hostRole.Id;
        }
        else if (staffRole != null)
        {
            entity.Genre = "propose";
            entity.CreatorId = staffRole.Id;
        }
        else
        {
            throw new("User must be either Host Institution or Staff to create a project.");
        }

        entity.Status = Status.Created.ToString().ToLowerInvariant();

        await _unitOfWork.GetProjectRepository().AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return entity.Adapt<RS_Project>();
    }

    public async Task<RS_Project?> UpdateAsync(Guid id, RQ_Project request, [FromBody]string status)
    {
        var repo = _unitOfWork.GetProjectRepository();
        var entity = await repo.GetByIdAsync<Guid>(id);
        if (entity == null) return null;
        entity.UpdatedAt = DateTime.Now;
        request.Adapt(entity);
        entity.Status = status.ToStatus().ToString().ToLowerInvariant();
        await repo.UpdateAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return entity.Adapt<RS_Project>();
    }

    public async Task<RS_Project?> ToggleStatusAsync(Guid id)
    {
        var repo = _unitOfWork.GetProjectRepository();
        var entity = await repo.GetByIdAsync<Guid>(id);
        if (entity == null) return null;

        var parsed = entity.Status.ToStatus();
        entity.Status = parsed == Status.Created
            ? Status.Deleted.ToString().ToLower()
            : Status.Created.ToString().ToLower();

        await repo.UpdateAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return entity.Adapt<RS_Project>();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var repo = _unitOfWork.GetProjectRepository();
        var entity = await repo.GetByIdAsync<Guid>(id);
        if (entity == null) return false;

        await repo.DeleteAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<List<RS_ProjectOverview>> GetAllOnlineUserProjectAsync()
    {
        var accountId = Guid.Parse(_userContextService.GetCurrentUserId());
        var currentRoleName = _userContextService.GetCurrentUserRole();

        // Fetch UserRoles with Role included (to access Name)
        var userRoles = await _unitOfWork.GetUserRoleRepository()
            .GetListAsync(
                ur => ur.AccountId == accountId,
                include: q => q.Include(ur => ur.Role)
            );

        // Filter by matching role name
        var matchingRoles = userRoles
            .Where(ur => ur.Role != null && ur.Role.Name == currentRoleName)
            .ToList();

        var projectIds = matchingRoles
            .Select(ur => ur.ProjectId)
            .Distinct()
            .ToList();

        if (!projectIds.Any())
            return new List<RS_ProjectOverview>();

        var projects = await _unitOfWork.GetProjectRepository()
            .GetListAsync(
                p => projectIds.Contains(p.Id),
                hasTrackings: false
            );

        var overviewList = projects
            .OrderByDescending(p => p.CreatedAt)
            .Adapt<List<RS_ProjectOverview>>();

        return overviewList;
    }

    public async Task<RS_Project> EnrollAsPrincipalAsync(Guid sourceProjectId)
{
    Guid principalId = Guid.Parse(_userContextService.GetCurrentUserId());

    // Step 1: Validate source project
    var sourceProject = await _unitOfWork.GetProjectRepository().GetByIdAsync(sourceProjectId, hasTrackings: false);
    if (sourceProject == null)
        throw new NotFoundException("Project to enroll not found.");

    // Step 2: Validate role of Principal Investigator
    var principal = await _unitOfWork.GetAccountRepository().GetByIdAsync(principalId);
    var hasPrincipalRole = principal.UserRoles.Any(ur => ur.Role.Name == "Principal Investigator");

    if (!hasPrincipalRole)
        throw new UnauthorizedAccessException("Account does not have Principal Investigator role.");

    // Step 3: Check for existing proposals with same info
    var similarProjects = await _unitOfWork.GetProjectRepository().GetListAsync(
        p =>
            p.CreatorId == sourceProject.CreatorId &&
            p.Genre == "proposal" &&
            p.EnglishTitle == sourceProject.EnglishTitle &&
            p.VietnameseTitle == sourceProject.VietnameseTitle &&
            p.StartDate == sourceProject.StartDate,
        hasTrackings: false,
        useSplitQuery: true
    );

    var restrictedStatuses = new[]
    {
        Status.Draft,
        Status.Created,
        Status.Submitted,
        Status.Approved,
        Status.InProgress,
        Status.Completed
    };

    var existingDraft = similarProjects.FirstOrDefault(p =>
        restrictedStatuses.Contains(p.Status.ToStatus()));

    if (existingDraft != null)
    {
        // ✅ Return the existing draft instead of throwing
        return existingDraft.Adapt<RS_Project>();
    }

        // Step 4: Clone as draft
        var draftClone = sourceProject.Adapt<Project>();
        draftClone.Id = Guid.NewGuid();
        draftClone.Genre = "proposal";
        draftClone.Status = Status.Draft.ToString().ToLowerInvariant();
        draftClone.CreatedAt = DateTime.Now;
        draftClone.UpdatedAt = DateTime.Now;

        await _unitOfWork.GetProjectRepository().AddAsync(draftClone);

    // Step 5: Attach Principal Investigator role to cloned project
    var piRole = await _unitOfWork.GetRoleRepository()
        .GetOneAsync(r => r.Name == "Principal Investigator");

    if (piRole == null)
        throw new ArgumentException("Role 'Principal Investigator' not found.");

    var userRole = new UserRole
    {
        Id = Guid.NewGuid(),
        AccountId = principalId,
        Code = $"UR-{DateTime.Now:yyyy_MM}_{principalId.ToString().Substring(0, 6).ToUpperInvariant()}",
        RoleId = piRole.Id,
        ProjectId = draftClone.Id,
        GroupName = draftClone.EnglishTitle,
        IsOfficial = false,
        ExpireDate = draftClone.CreatedAt.AddYears(1),
        CreatedAt = DateTime.Now,
        Status = Status.Approved.ToString().ToLowerInvariant()
    };

    await _unitOfWork.GetUserRoleRepository().AddAsync(userRole);
    await _unitOfWork.SaveChangesAsync();

    return draftClone.Adapt<RS_Project>();
}

    //=============================================================================================
    public async Task<bool> CreateFromDocumentAsync(Project project, Document document, Guid creatorId, RQ_MilestoneTaskContent titleContent)
    {
        var documentContent = document.ContentHtml;
        if (string.IsNullOrWhiteSpace(documentContent)) return false;

        var doc = new HtmlDocument();
        doc.LoadHtml(documentContent);

        // Find section match SectionTitle
        var sectionNode = doc.DocumentNode.SelectSingleNode($"//*[contains(text(), '{titleContent.SectionTitle}')]") ??
            throw new NotFoundException("Not found that section content to identify data!");

        // Find nearest data table
        var tableNode = sectionNode.SelectSingleNode("following::table[1]") ??
            throw new NotFoundException("Not found any data table below the title!");

        var rows = tableNode.SelectNodes(".//tr");
        if (rows is null || rows.Count == 0) throw new NotFoundException("Not found any row data in this table!");

        //Map table into array
        var headerRow = rows.FirstOrDefault(r => r.SelectNodes("th") != null) ??
            throw new Exception("Không tìm thấy dòng tiêu đề bảng!");

        var headerCells = headerRow.SelectNodes("th");
        var columnMap = new Dictionary<string, int>();
        for (int i = 0; i < headerCells.Count; i++)
        {
            var headerText = headerCells[i].InnerText.Trim();
            columnMap[headerText] = i;
        }

        if (!columnMap.TryGetValue(titleContent.Description, out var descriptionIndex))
            throw new NotFoundException($"Not found column '{titleContent.Description}' in table.");
        if (!columnMap.TryGetValue(titleContent.Objective, out var objectiveIndex))
            throw new NotFoundException($"Not found column '{titleContent.Objective}' in table.");
        if (!columnMap.TryGetValue(titleContent.TimeEstimate, out var timeIndex))
            throw new NotFoundException($"Not found column '{titleContent.TimeEstimate}' in table.");
        if (!columnMap.TryGetValue(titleContent.CostEstimate, out var costIndex))
            throw new NotFoundException($"Not found column '{titleContent.CostEstimate}' in table.");

        var milestonesFromDoc = new List<Milestone>();
        var tasksFromDoc = new List<SRPM_Repositories.Models.Task>();
        Milestone? currentMilestone = null;
        var yyyymm = DateTime.Now.ToString("yyyyMM");

        //loop through table cell skip first row (header title)
        foreach (var row in rows.Skip(1))
        {
            //Regex Code
            var sequence = new Random().Next(1, 999).ToString("D3");

            var cells = row.SelectNodes("td");
            if (cells == null || cells.Count == 0) continue;

            //Get data from Document
            var description = cells.ElementAtOrDefault(descriptionIndex)?.InnerText.Trim() ?? "";
            var title = description.Length <= 20 ? description : description.Substring(0, 20).Trim() + "...";
            var objective = cells.ElementAtOrDefault(objectiveIndex)?.InnerText.Trim() ?? "";
            var timeRaw = cells.ElementAtOrDefault(timeIndex)?.InnerText.Trim() ?? "";
            var costRaw = cells.ElementAtOrDefault(costIndex)?.InnerText.Trim() ?? "";

            var contentHtml = cells[descriptionIndex].InnerHtml.Trim();

            // Parse Time From Regex
            DateTime? startDate = null;
            DateTime? endDate = null;
            var timeMatch = Regex.Match(timeRaw, @"(\d{2}/\d{2}/\d{4})\s*,\s*(\d{2}/\d{2}/\d{4})");
            if (timeMatch.Success)
            {
                if (DateTime.TryParseExact(timeMatch.Groups[1].Value, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var start))
                    startDate = start;
                if (DateTime.TryParseExact(timeMatch.Groups[2].Value, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var end))
                    endDate = end;
            }

            // Parse Cost
            decimal cost = 0m;
            decimal.TryParse(costRaw.Replace(",", "").Replace(".", ""), out cost);

            if (contentHtml.Contains("<b>")) // Milestone
            {
                currentMilestone = new Milestone
                {
                    Code = $"MS-{yyyymm}-{sequence}",
                    Title = title,
                    Description = description,
                    Objective = objective,
                    StartDate = startDate,
                    EndDate = endDate,
                    Cost = cost,
                    ProjectId = project.Id,
                    CreatorId = creatorId
                };

                milestonesFromDoc.Add(currentMilestone);
            }
            else if (!string.IsNullOrWhiteSpace(description) && currentMilestone != null) // Task
            {
                tasksFromDoc.Add(new SRPM_Repositories.Models.Task
                {
                    Code = $"TS-{yyyymm}-{sequence}",
                    Name = title,
                    Description = description,
                    Objective = objective,
                    StartDate = startDate,
                    EndDate = endDate,
                    Cost = cost,
                    MilestoneId = currentMilestone.Id,
                    CreatorId = creatorId
                });
            }
        }

        // Insert into database
        await _unitOfWork.GetMilestoneRepository().AddRangeAsync(milestonesFromDoc);
        await _unitOfWork.GetTaskRepository().AddRangeAsync(tasksFromDoc);
        return await _unitOfWork.SaveChangesAsync();
    }

    private async Task<(List<RS_ProjectSimilarityResult>? listSimilarity, string summaryEvaluation, string bgTaskId)>
        AIReviewAndPlagiarism(Guid evaluationStageId, RQ_ProjectContentForAI projectSummary)
    {
        if (string.IsNullOrWhiteSpace(projectSummary.Description)) throw new BadRequestException("Require Project Description!");

        List<RS_ProjectSimilarityResult>? listSimilarity = [];
        float[]? vectorDescription;
        string summaryEvaluation = "";

        //BackgroundTask Id
        string? bgTaskId = _taskQueueHandler.EnqueueTracked(async cancelToken =>
        {
            //Query encoded completed Project
            var projectEncoded = await _unitOfWork.GetProjectRepository().GetListAdvanceAsync(
                p => p.Status.Equals("completed", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(p.EncodedDescription),
                p => new { p.Id, p.EnglishTitle, p.Description, p.EncodedDescription });

            //Query all completed Project if 'projectEncoded' is null
            List<Project>? databaseSource = projectEncoded is null ?
            await _unitOfWork.GetProjectRepository().GetListAsync(p => p.Status.Equals("completed", StringComparison.OrdinalIgnoreCase))
                : projectEncoded.Adapt<List<Project>>();

            //Final Source
            //syntheticSource = projectSource + online source
            var syntheticSource = databaseSource.Adapt<List<RS_ProjectSimilarityResult>>();

            vectorDescription = await _openAIService.EmbedTextAsync(projectSummary.Description, cancelToken);

            summaryEvaluation = await _openAIService.ReviewProjectAsync(projectSummary, cancelToken);

            //Then compare
            listSimilarity = await _openAIService.CompareWithSourceAsync(vectorDescription, syntheticSource, cancelToken);
        });

        return (listSimilarity, summaryEvaluation, bgTaskId);
    }
}