using HtmlAgilityPack;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Services.BusinessModels;
using SRPM_Services.BusinessModels.Others;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Extensions.Enumerables;
using SRPM_Services.Extensions.Exceptions;
using SRPM_Services.Extensions.MicrosoftBackgroundService;
using SRPM_Services.Extensions.OpenAI;
using SRPM_Services.Extensions.Utils;
using SRPM_Services.Interfaces;
using System.Text.RegularExpressions;

namespace SRPM_Services.Implements;

public class ProjectService : IProjectService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;
    private readonly ITaskQueueHandler _taskQueueHandler;

    public ProjectService(IUnitOfWork unitOfWork, IUserContextService userContextService, ITaskQueueHandler taskQueueHandler)
    {
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
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

        var isMember = userRoles!.Any();

        // Group roles: Leader, Secretary, etc.
        var groupRoles = userRoles!
            .Where(ur => ur.Role != null && ur.Role.IsGroupRole == true)
            .Select(ur => ur.Role.Name)
            .Distinct()
            .ToList();

        // Non-group roles: Researcher, Principal, etc.
        var fallbackRoles = userRoles!
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
               (string.IsNullOrWhiteSpace(query.Code) || p.Code == query.Code) &&
               (string.IsNullOrWhiteSpace(query.Category) || p.Category == query.Category) &&
               (string.IsNullOrWhiteSpace(query.Type) || p.Type == query.Type) &&
               (query.Genres == null || query.Genres.Count == 0 || query.Genres.Contains(p.Genre)) &&
                (query.Statuses == null || query.Statuses.Count == 0 || query.Statuses.Contains(p.Status)) &&
               (string.IsNullOrWhiteSpace(query.Language) || p.Language == query.Language) &&
               (!query.MajorId.HasValue ||
                   p.ProjectMajors!.Any(pm => pm.MajorId == query.MajorId)) &&
               (!query.FieldId.HasValue ||
                   p.ProjectMajors!.Any(pm => pm.Major.FieldId == query.FieldId)) &&
               (query.TagNames == null ||
                query.TagNames.Count == 0 ||
                !query.TagNames.Any(tag => !string.IsNullOrWhiteSpace(tag)) ||
                p.ProjectTags!.Any(t => query.TagNames.Any(tag => tag == t.Name && !string.IsNullOrWhiteSpace(tag)))),
    include: q =>
    {
        q = q.Include(p => p.ProjectMajors!)
                .ThenInclude(pm => pm.Major)
                    .ThenInclude(m => m.Field);

        q = q.Include(p => p.ProjectTags); // always included — adjust if needed

        if (query.IncludeCreator)
            q = q.Include(p => p.Creator).ThenInclude(c => c.Role)
            .Include(p => p.Creator).ThenInclude(a => a.Account);

        if (query.IncludeMembers)
            q = q.Include(p => p.Members!).ThenInclude(m => m.Account);

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
            "englishtitle" => query.Desc ? projects!.OrderByDescending(p => p.EnglishTitle).ToList() : projects!.OrderBy(p => p.EnglishTitle).ToList(),
            "vietnamesetitle" => query.Desc ? projects!.OrderByDescending(p => p.VietnameseTitle).ToList() : projects!.OrderBy(p => p.VietnameseTitle).ToList(),
            "createdate" => query.Desc ? projects!.OrderByDescending(p => p.CreatedAt).ToList() : projects!.OrderBy(p => p.CreatedAt).ToList(),
            _ => projects!.OrderBy(p => p.CreatedAt).ToList()
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
        var hostRole = userRoles.FirstOrDefault(r => r.Role?.Name == "Host Institution" && r.Status != Status.Deleted.ToString().ToLowerInvariant() && r.ProjectId == null && r.AppraisalCouncil == null);
        var staffRole = userRoles.FirstOrDefault(r => r.Role?.Name == "Staff" && r.Status != Status.Deleted.ToString().ToLowerInvariant() && r.ProjectId == null && r.AppraisalCouncil == null);

        var role = hostRole ?? staffRole;

        if (role == null)
        {
            throw new Exception("User must be either Host Institution or Staff to create a project.");
        }

        entity.Genre = (role == hostRole) ? "normal" : "propose";
        entity.CreatorId = role.Id;

        entity.Status = Status.Created.ToString().ToLowerInvariant();

        await _unitOfWork.GetProjectRepository().AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return entity.Adapt<RS_Project>();
    }

    public async Task<RS_Project?> UpdateAsync(Guid id, RQ_Project request, [FromBody] string status)
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

        var entity = await repo.GetOneAsync(
            p => p.Id == id,
            include: q => q
                .Include(p => p.Members)
                .Include(p => p.Notifications)
                .Include(p => p.Milestones)
                .Include(p => p.Evaluations)
                .Include(p => p.ProjectsSimilarity)
                .Include(p => p.ProjectMajors)
                .Include(p => p.ProjectTags)
                .Include(p => p.Documents)
                .Include(p => p.Transactions)
                .Include(p => p.ProjectResult),
            hasTrackings: true
        );

        if (entity == null) return false;

        // Delete related entities
        var memberRepo = _unitOfWork.GetUserRoleRepository();
        var notificationRepo = _unitOfWork.GetNotificationRepository();
        var milestoneRepo = _unitOfWork.GetMilestoneRepository();
        var evaluationRepo = _unitOfWork.GetEvaluationRepository();
        var similarityRepo = _unitOfWork.GetProjectSimilarityRepository();
        var majorRepo = _unitOfWork.GetProjectMajorRepository();
        var tagRepo = _unitOfWork.GetProjectTagRepository();
        var documentRepo = _unitOfWork.GetDocumentRepository();
        var transactionRepo = _unitOfWork.GetTransactionRepository();
        var resultRepo = _unitOfWork.GetProjectResultRepository();

        if (entity.Members?.Any() == true)
            await memberRepo.DeleteRangeAsync(entity.Members);

        if (entity.Notifications?.Any() == true)
            await notificationRepo.DeleteRangeAsync(entity.Notifications);

        if (entity.Milestones?.Any() == true)
            await milestoneRepo.DeleteRangeAsync(entity.Milestones);

        if (entity.Evaluations?.Any() == true)
            await evaluationRepo.DeleteRangeAsync(entity.Evaluations);

        if (entity.ProjectsSimilarity?.Any() == true)
            await similarityRepo.DeleteRangeAsync(entity.ProjectsSimilarity);

        if (entity.ProjectMajors?.Any() == true)
            await majorRepo.DeleteRangeAsync(entity.ProjectMajors);

        if (entity.ProjectTags?.Any() == true)
            await tagRepo.DeleteRangeAsync(entity.ProjectTags);

        if (entity.Documents?.Any() == true)
            await documentRepo.DeleteRangeAsync(entity.Documents);

        if (entity.Transactions?.Any() == true)
            await transactionRepo.DeleteRangeAsync(entity.Transactions);

        if (entity.ProjectResult != null)
            await resultRepo.DeleteAsync(entity.ProjectResult);

        // Delete the main project
        await repo.DeleteAsync(entity);

        // Save all changes
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
        var matchingRoles = userRoles!
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

        var overviewList = projects!
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
        var hasPrincipalRole = principal.UserRoles!.Any(ur => ur.Role.Name == "Principal Investigator");

        if (!hasPrincipalRole)
            throw new UnauthorizedAccessException("Account does not have Principal Investigator role.");

        // Step 3: Check for existing proposals with same info
        var similarProjects = await _unitOfWork.GetProjectRepository().GetListAsync(
            p => p.CreatorId == sourceProject.CreatorId &&
                 p.Genre == "proposal" &&
                 p.EnglishTitle == sourceProject.EnglishTitle &&
                 p.VietnameseTitle == sourceProject.VietnameseTitle &&
                 p.StartDate == sourceProject.StartDate,
            include: q => q.Include(p => p.Members),
            hasTrackings: false,
            useSplitQuery: true
        );

        var enrolledProject = similarProjects?
            .FirstOrDefault(project => project.Members.Any(m => m.AccountId == principalId));

        if (enrolledProject != null)
        {
            return enrolledProject.Adapt<RS_Project>();
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
            Code = $"UR-{DateTime.Now:yyyy_MM_dd}_{principalId.ToString().Substring(0, 6).ToUpperInvariant()}",
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
    //Create Milestone, task
    public async Task<bool> CreateFromDocumentAsync(RQ_MilestoneTaskContent content)
    {
        if (string.IsNullOrWhiteSpace(content.DocumentContent)) throw new NotFoundException("Not found document content");
        var contentHtml = content.DocumentContent;

        var doc = new HtmlDocument();
        doc.LoadHtml(contentHtml);

        // Find section match SectionTitle
        var sectionNode = doc.DocumentNode.SelectSingleNode($"//*[contains(text(), '{content.SectionTitle}')]") ??
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

        if (!columnMap.TryGetValue(content.Description, out var descriptionIndex))
            throw new NotFoundException($"Not found column '{content.Description}' in table.");
        if (!columnMap.TryGetValue(content.Objective, out var objectiveIndex))
            throw new NotFoundException($"Not found column '{content.Objective}' in table.");
        if (!columnMap.TryGetValue(content.TimeEstimate, out var timeIndex))
            throw new NotFoundException($"Not found column '{content.TimeEstimate}' in table.");
        if (!columnMap.TryGetValue(content.CostEstimate, out var costIndex))
            throw new NotFoundException($"Not found column '{content.CostEstimate}' in table.");

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
            var currentRowHtml = cells[descriptionIndex].InnerHtml.Trim();

            //Get data from Document
            var description = cells.ElementAtOrDefault(descriptionIndex)?.InnerText.Trim() ?? "";
            var title = description.Length <= 20 ? description : description.Substring(0, 20).Trim() + "...";
            var objective = cells.ElementAtOrDefault(objectiveIndex)?.InnerText.Trim() ?? "";
            var timeRaw = cells.ElementAtOrDefault(timeIndex)?.InnerText.Trim() ?? "";
            var costRaw = cells.ElementAtOrDefault(costIndex)?.InnerText.Trim() ?? "";


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

            if (currentRowHtml.Contains("<b>")) // Milestone
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
                    ProjectId = content.ProjectId,
                    CreatorId = content.CreatorId
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
                    CreatorId = content.CreatorId
                });
            }
        }

        if (milestonesFromDoc is null || milestonesFromDoc.Count == 0) throw new BadRequestException("Cannot read data in document, please check again if the data is correct format!");
        // Insert into database
        await _unitOfWork.GetMilestoneRepository().AddRangeAsync(milestonesFromDoc);
        await _unitOfWork.GetTaskRepository().AddRangeAsync(tasksFromDoc);
        return await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<RS_ProjectOverview>> GetHostProjectHistory()
    {
        try
        {
            Guid accountId = Guid.Parse(_userContextService.GetCurrentUserId());

            // Step 1: Get user roles for Host Institution
            var userRoles = await _unitOfWork.GetUserRoleRepository()
                .GetOneAsync(r =>
                    r.AccountId == accountId &&
                    r.Role != null &&
                    r.Role.Name == "Host Institution" &&
                    r.Status.ToLower() != Status.Deleted.ToString().ToLower() &&
                    r.ProjectId == null &&
                    r.AppraisalCouncilId == null,
                    hasTrackings: false
                );

            if (userRoles == null)
                throw new NotFoundException("No Host Institution roles found for this account.");

            var projects = await _unitOfWork.GetProjectRepository()
                .GetListAsync(p =>
                    p.CreatorId == userRoles.Id &&
                    p.Genre == "normal" &&
                    p.Status.ToLower() != Status.Deleted.ToString().ToLower(),
                    hasTrackings: false
                );

            if (projects == null || !projects.Any())
                throw new NotFoundException("No host projects found for this account.");

            return projects.Adapt<List<RS_ProjectOverview>>();
        }
        catch (Exception ex)
        {
            throw new Exception("Error getting host project history.", ex);
        }
    }
    public async Task<List<RS_ProjectOverview>> GetStaffProjectHistory()
    {
        try
        {
            Guid accountId = Guid.Parse(_userContextService.GetCurrentUserId());

            var userRoles = await _unitOfWork.GetUserRoleRepository()
                .GetOneAsync(r =>
                    r.AccountId == accountId &&
                    r.Role != null &&
                    r.Role.Name == "Staff" &&
                    r.Status.ToLower() != Status.Deleted.ToString().ToLower() &&
                    r.ProjectId == null &&
                    r.AppraisalCouncilId == null,
                    hasTrackings: false
                );

            if (userRoles == null)
                throw new NotFoundException("No Staff roles found for this account.");

            var projects = await _unitOfWork.GetProjectRepository()
                .GetListAsync(p =>
                    p.CreatorId == userRoles.Id &&
                    p.Genre == "propose" &&
                    p.Status.ToLower() != Status.Deleted.ToString().ToLower(),
                    hasTrackings: false
                );

            if (projects == null || !projects.Any())
                throw new NotFoundException("No staff projects found for this account.");

            return projects.Adapt<List<RS_ProjectOverview>>();
        }
        catch (Exception ex)
        {
            throw new Exception("Error getting staff project history. " + ex);
        }
    }
    public async Task<bool> ApproveProposalAsync(Guid proposalProjectId)
    {
        var repo = _unitOfWork.GetProjectRepository();
        var invalidStatuses = new[]
{
            Status.InProgress.ToString().ToLower(),
            Status.Completed.ToString().ToLower(),
            Status.Cancelled.ToString().ToLower(),
            Status.Deleted.ToString().ToLower()
        };
        // Step 1: Get the proposal project
        var proposal = await repo.GetByIdAsync(proposalProjectId, hasTrackings: true);
        if (proposal == null)
            throw new KeyNotFoundException("Proposal project not found.");

        // Step 2: Check proposal status
        if (proposal.Status != Status.Submitted.ToString().ToLower())
            throw new InvalidOperationException("Only submitted proposals can be approved.");

        // Step 3: Check if another proposal is already approved
        var alreadyApproved = await repo.AnyAsync(
            p => p.Code == proposal.Code &&
                 p.Id != proposal.Id &&
                 p.Genre == "proposal" &&
                 invalidStatuses.Contains(p.Status)
        );

        if (alreadyApproved)
            throw new InvalidOperationException("Another proposal has already been approved for this project.");

        // Step 4: Check source project status
        var sourceProject = await repo.GetOneAsync(
            p => p.Code == proposal.Code &&
                 (p.Genre.ToLower() == "propose" || p.Genre.ToLower() == "normal")
        );

        if (sourceProject != null)
        {

            if (invalidStatuses.Contains(sourceProject.Status))
                throw new InvalidOperationException($"Cannot approve proposal. Source project is already {sourceProject.Status}.");

            // Step 5: Update source project to InProgress
            sourceProject.Status = Status.InProgress.ToString().ToLower();
        }

        // Step 6: Approve this proposal
        proposal.Status = Status.Approved.ToString().ToLower();

        // Step 7: Reject other submitted proposals
        var otherProposals = await repo.GetListAsync(
            p => p.Code == proposal.Code &&
                 p.Id != proposal.Id &&
                 p.Status == Status.Submitted.ToString().ToLower(),
            hasTrackings: true
        );

        foreach (var other in otherProposals)
        {
            other.Status = Status.Rejected.ToString().ToLower();
        }

        // Step 8: Save changes
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
    public async Task<(Guid id, bool isEnrolled)> CheckIsEnrollInProject(Guid sourceProjectId)
    {
        var principalId = Guid.Parse(_userContextService.GetCurrentUserId()); // Your method to get current user ID

        var sourceProject = await _unitOfWork.GetProjectRepository().GetOneAsync(
            p => p.Id == sourceProjectId,
            hasTrackings: false
        );

        if (sourceProject == null)
            throw new KeyNotFoundException("Source project not found.");

        var similarProposals = await _unitOfWork.GetProjectRepository().GetListAsync(
            p => p.CreatorId == sourceProject.CreatorId &&
                 p.Genre == "proposal" &&
                 p.EnglishTitle == sourceProject.EnglishTitle &&
                 p.VietnameseTitle == sourceProject.VietnameseTitle &&
                 p.StartDate == sourceProject.StartDate,
            include: q => q.Include(p => p.Members),
            hasTrackings: false,
            useSplitQuery: true
        );

        var enrolledProposal = similarProposals?
            .FirstOrDefault(p => p.Members.Any(m => m.AccountId == principalId));

        return enrolledProposal != null
            ? (enrolledProposal.Id, true)
            : (Guid.Empty, false);
    }
}