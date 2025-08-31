using HtmlAgilityPack;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Implements;
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
using System.Data;
using System.Text.RegularExpressions;

namespace SRPM_Services.Implements;

public class ProjectService : IProjectService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;
    private readonly ITaskQueueHandler _taskQueueHandler;
    private readonly INotificationService _notificationService;
    public ProjectService(IUnitOfWork unitOfWork, IUserContextService userContextService, ITaskQueueHandler taskQueueHandler, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
        _taskQueueHandler = taskQueueHandler;
        _notificationService = notificationService;
    }
    public async Task<object?> GetByIdAsync(Guid id)
    {
        var userId = Guid.Parse(_userContextService.GetCurrentUserId());
        var targetStatus = Status.Approved.ToString().ToLower();

        var userRoles = await _unitOfWork.GetUserRoleRepository()
            .GetListAsync(
                us => us.AccountId == userId
                       && us.Status.ToLower() == targetStatus,
                include: q => q.Include(ur => ur.Role)
            );

        var isMember = userRoles!.Any(us => us.ProjectId ==id);

        var groupRoles = userRoles!
            .Where(ur => ur.Role != null && ur.Role.IsGroupRole == true && ur.ProjectId == id)
            .Select(ur => ur.Role.Name)
            .Distinct()
            .ToList();

        var fallbackRoles = userRoles!
            .Where(ur => ur.Role != null && ur.Role.IsGroupRole == false && ur.ProjectId == id)
            .Select(ur => ur.Role.Name)
            .Distinct()
            .ToList();

        var roleInProject = groupRoles.Any() ? groupRoles : fallbackRoles;

        if (!isMember)
        {
            var projectOverview = await _unitOfWork.GetProjectRepository()
                .GetOneAsync(p => p.Id == id, include: q => q.Include(q => q.ProjectTags));

            var overviewDto = projectOverview?.Adapt<RS_ProjectOverview>();

            var isStaff = userRoles
                 .Any(role => role.Role.Name.Contains("Staff"));

            if (!isStaff && overviewDto != null)
            {
                overviewDto.Budget = null;
            }

            return new
            {
                ProjectDetail = overviewDto,
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

        // Get system configurations
        var defaultProjectMembers = await _unitOfWork.GetSystemConfigurationRepository()
            .GetOneAsync(c => c.ConfigKey.Contains("project") && c.ConfigType == "member");

        var defaultProjectDuration = await _unitOfWork.GetSystemConfigurationRepository()
            .GetOneAsync(c => c.ConfigKey.Contains("duration") && c.ConfigType == "project");

        // Parse config values
        int maxAllowedMembers = int.Parse(defaultProjectMembers.ConfigValue);
        int maxAllowedDuration = int.Parse(defaultProjectDuration.ConfigValue);

        // Validate and apply system configurations with fallback defaults
        entity.Budget = entity.Budget <= 0 ? 0m : entity.Budget;
        entity.Progress = entity.Progress <= 0 ? 0m : entity.Progress;
        entity.Language = string.IsNullOrWhiteSpace(entity.Language) ? "English" : entity.Language;

        // Validate MaximumMember
        if (entity.MaximumMember <= 0)
        {
            entity.MaximumMember = maxAllowedMembers;
        }
        else if (entity.MaximumMember > maxAllowedMembers)
        {
            throw new ArgumentOutOfRangeException(nameof(entity.MaximumMember),
                $"Maximum members ({entity.MaximumMember}) exceed allowed limit ({maxAllowedMembers}).");
        }

        // Validate Duration
        if (entity.Duration <= 0)
        {
            entity.Duration = maxAllowedDuration;
        }
        else if (entity.Duration > maxAllowedDuration)
        {
            throw new ArgumentOutOfRangeException(nameof(entity.Duration),
                $"Duration ({entity.Duration}) exceeds allowed limit ({maxAllowedDuration}).");
        }

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
        var hostRole = userRoles.FirstOrDefault(r => r.Role?.Name == "Host Institution" &&
            r.Status != Status.Deleted.ToString().ToLowerInvariant() &&
            r.ProjectId == null &&
            r.AppraisalCouncil == null);

        var staffRole = userRoles.FirstOrDefault(r => r.Role?.Name == "Staff" &&
            r.Status != Status.Deleted.ToString().ToLowerInvariant() &&
            r.ProjectId == null &&
            r.AppraisalCouncil == null);

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

        var entity = await repo.GetOneAsync(
            c => c.Id == id,
            hasTrackings: true,
            include: c => c.Include(c => c.Milestones)
        );
        if (entity == null) return null;

        entity.UpdatedAt = DateTime.Now;

        request.Adapt(entity);

        entity.Status = status.ToStatus().ToString().ToLowerInvariant();

        entity.Budget = entity.Milestones?.Sum(m => m.Cost) ?? 0;

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
                .Include(p => p.Milestones).ThenInclude(t => t.Tasks)
                .Include(p => p.Evaluations).ThenInclude(e => e.EvaluationStages).ThenInclude(es => es.IndividualEvaluations)
                .Include(p => p.ProjectsSimilarity)
                .Include(p => p.ProjectMajors)
                .Include(p => p.ProjectTags)
                .Include(p => p.Documents)
                .Include(p => p.Transactions)
                .Include(p => p.ProjectResult).AsSplitQuery(),
            hasTrackings: true
        );

        if (entity == null) return false;

        // Get repositories
        var memberRepo = _unitOfWork.GetUserRoleRepository();
        var notificationRepo = _unitOfWork.GetNotificationRepository();
        var milestoneRepo = _unitOfWork.GetMilestoneRepository();
        var evaluationRepo = _unitOfWork.GetEvaluationRepository();
        var evaluationStageRepo = _unitOfWork.GetEvaluationStageRepository(); // You'll need this
        var individualEvaluationRepo = _unitOfWork.GetIndividualEvaluationRepository(); // And this if applicable
        var taskRepo = _unitOfWork.GetTaskRepository(); // Add task repository
        var similarityRepo = _unitOfWork.GetProjectSimilarityRepository();
        var majorRepo = _unitOfWork.GetProjectMajorRepository();
        var tagRepo = _unitOfWork.GetProjectTagRepository();
        var documentRepo = _unitOfWork.GetDocumentRepository();
        var transactionRepo = _unitOfWork.GetTransactionRepository();
        var resultRepo = _unitOfWork.GetProjectResultRepository();

        // Delete in the correct order (child entities first)

        // 1. First, handle documents that reference IndividualEvaluations
        if (entity.Evaluations?.Any() == true)
        {
            var individualEvaluationIds = entity.Evaluations
                .SelectMany(e => e.EvaluationStages)
                .SelectMany(es => es.IndividualEvaluations)
                .Select(ie => ie.Id)
                .ToList();

            if (individualEvaluationIds.Any())
            {
                // Find documents that reference these individual evaluations
                var documentsLinkedToIndividualEvaluations = await documentRepo.GetListAsync(
                    d => d.IndividualEvaluationId.HasValue && individualEvaluationIds.Contains(d.IndividualEvaluationId.Value)
                );

                if (documentsLinkedToIndividualEvaluations.Any())
                {
                    await documentRepo.DeleteRangeAsync(documentsLinkedToIndividualEvaluations);
                }
            }
        }

        // 2. Delete IndividualEvaluations
        if (entity.Evaluations?.Any() == true)
        {
            var individualEvaluations = entity.Evaluations
                .SelectMany(e => e.EvaluationStages)
                .SelectMany(es => es.IndividualEvaluations)
                .ToList();

            if (individualEvaluations.Any())
                await individualEvaluationRepo.DeleteRangeAsync(individualEvaluations);
        }

        // 3. Delete EvaluationStages
        if (entity.Evaluations?.Any() == true)
        {
            var evaluationStages = entity.Evaluations
                .SelectMany(e => e.EvaluationStages)
                .ToList();

            if (evaluationStages.Any())
                await evaluationStageRepo.DeleteRangeAsync(evaluationStages);
        }

        // 4. Now delete Evaluations
        if (entity.Evaluations?.Any() == true)
            await evaluationRepo.DeleteRangeAsync(entity.Evaluations);

        // Delete other related entities
        if (entity.Members?.Any() == true)
            await memberRepo.DeleteRangeAsync(entity.Members);

        if (entity.Notifications?.Any() == true)
            await notificationRepo.DeleteRangeAsync(entity.Notifications);

        // Delete Tasks linked to Milestones
        if (entity.Milestones?.Any() == true)
        {
            var tasks = entity.Milestones
                .SelectMany(m => m.Tasks)
                .ToList();

            if (tasks.Any())
                await taskRepo.DeleteRangeAsync(tasks);
        }

        if (entity.Milestones?.Any() == true)
            await milestoneRepo.DeleteRangeAsync(entity.Milestones);

        if (entity.ProjectsSimilarity?.Any() == true)
            await similarityRepo.DeleteRangeAsync(entity.ProjectsSimilarity);

        if (entity.ProjectMajors?.Any() == true)
            await majorRepo.DeleteRangeAsync(entity.ProjectMajors);

        if (entity.ProjectTags?.Any() == true)
            await tagRepo.DeleteRangeAsync(entity.ProjectTags);

        // 5. Delete remaining documents directly linked to the project
        if (entity.Documents?.Any() == true)
        {
            // Filter out documents that were already deleted above
            var remainingDocuments = entity.Documents.Where(d => !d.IndividualEvaluationId.HasValue).ToList();
            if (remainingDocuments.Any())
                await documentRepo.DeleteRangeAsync(remainingDocuments);
        }

        if (entity.Transactions?.Any() == true)
            await transactionRepo.DeleteRangeAsync(entity.Transactions);

        if (entity.ProjectResult != null)
            await resultRepo.DeleteAsync(entity.ProjectResult);

        // Finally, delete the main project
        await repo.DeleteAsync(entity);

        // Save all changes
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<List<RS_ProjectOverview>> GetAllOnlineUserProjectAsync(
        List<string>? statusList,
        List<string>? genreList)
    {
        try
        {
            var accountId = Guid.Parse(_userContextService.GetCurrentUserId());
            var currentRole = _userContextService.GetCurrentUserRole();
            var deletedStatus = Status.Deleted.ToString().ToLower();

            if (currentRole == "Host Institution" || currentRole == "Staff")
            {
                var userRole = await _unitOfWork
                    .GetUserRoleRepository()
                    .GetOneAsync(r =>
                        r.AccountId == accountId &&
                        r.Role != null &&
                        r.Role.Name == currentRole &&
                        r.Status.ToLower() != deletedStatus &&
                        r.ProjectId == null &&
                        r.AppraisalCouncilId == null,
                        hasTrackings: false);

                if (userRole == null)
                    throw new NotFoundException($"No {currentRole} role found for this account.");

                var sourceGenre = currentRole == "Host Institution" ? "normal" : "propose";
                var completedStatus = Status.Completed.ToString().ToLower();

                var sourceProjects = await _unitOfWork
                    .GetProjectRepository()
                    .GetListAsync(p =>
                        p.CreatorId == userRole.Id &&
                        p.Genre.ToLower() == sourceGenre &&
                        p.Status.ToLower() != deletedStatus,
                        hasTrackings: false);

                if (sourceProjects == null || !sourceProjects.Any())
                    return new List<RS_ProjectOverview>();

                var completedSourceProjects = sourceProjects
                    .Where(p => p.Status.ToLower() == completedStatus)
                    .ToList();

                var nonCompletedSourceProjects = sourceProjects
                    .Where(p => p.Status.ToLower() != completedStatus)
                    .ToList();

                var resultProjects = new List<Project>();

                resultProjects.AddRange(nonCompletedSourceProjects);

                if (completedSourceProjects.Any())
                {
                    var completedProjectCodes = completedSourceProjects.Select(p => p.Code).ToList();

                    var proposalProjects = await _unitOfWork
                        .GetProjectRepository()
                        .GetListAsync(p =>
                            p.Genre.ToLower() == "proposal" &&
                            p.Code != null &&
                            completedProjectCodes.Contains(p.Code),
                            hasTrackings: false);

                    if (proposalProjects != null && proposalProjects.Any())
                    {
                        resultProjects.AddRange(proposalProjects);
                    }
                }

                return resultProjects.Adapt<List<RS_ProjectOverview>>();
            }
            var allRoles = await _unitOfWork
                .GetUserRoleRepository()
                .GetListAsync(
                    ur => ur.AccountId == accountId,
                    include: q => q.Include(ur => ur.Role));

            var matchingRoles = allRoles
                .Where(ur => ur.Role != null && ur.Role.Name == currentRole)
                .ToList();

            var projectIds = matchingRoles
                .Select(ur => ur.ProjectId)
                .Where(id => id != null)
                .Distinct()
                .ToList();

            if (!projectIds.Any())
                return new List<RS_ProjectOverview>();

            var normalizedStatus = statusList?.Select(s => s.ToLower()).ToList();
            var normalizedGenre = genreList?.Select(g => g.ToLower()).ToList();

            var genericProjects = await _unitOfWork
                .GetProjectRepository()
                .GetListAsync(p =>
                    projectIds.Contains(p.Id) &&
                    (normalizedStatus == null || !normalizedStatus.Any() || normalizedStatus.Contains(p.Status.ToLower())) &&
                    (normalizedGenre == null || !normalizedGenre.Any() || normalizedGenre.Contains(p.Genre.ToLower())),
                    hasTrackings: false);

            var overviewList = genericProjects!
                .OrderByDescending(p => p.CreatedAt)
                .Adapt<List<RS_ProjectOverview>>();
            return overviewList;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception("Error getting user project history.", ex);
        }
    }

    public async Task<RS_Project> EnrollAsPrincipalAsync(Guid sourceProjectId)
    {
        Guid principalId = Guid.Parse(_userContextService.GetCurrentUserId());

        var sourceProject = await _unitOfWork
            .GetProjectRepository()
            .GetOneAsync(
                p => p.Id == sourceProjectId,
                include: q => q
                    .Include(p => p.ProjectMajors)
                    .Include(p => p.ProjectTags),
                hasTrackings: false);

        if (sourceProject == null)
            throw new NotFoundException("Project to enroll not found.");

        var principal = await _unitOfWork.GetAccountRepository()
            .GetByIdAsync(principalId);

        if (principal.UserRoles?.All(ur => ur.Role.Name != "Principal Investigator") ?? true)
            throw new UnauthorizedAccessException("Account does not have Principal Investigator role.");

        await _unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            var alreadyEnrolled = await _unitOfWork.GetProjectRepository()
                .GetOneAsync(
                    p => p.CreatorId == sourceProject.CreatorId
                         && p.Genre == "proposal"
                         && p.EnglishTitle == sourceProject.EnglishTitle
                         && p.VietnameseTitle == sourceProject.VietnameseTitle
                         && p.StartDate == sourceProject.StartDate
                         && p.Members.Any(m => m.AccountId == principalId),
                    include: q => q.Include(p => p.Members)
                    .Include(p => p.ProjectMajors).ThenInclude(pm => pm.Major).ThenInclude(m => m.Field)
                    .Include(p => p.ProjectTags),
                    hasTrackings: false);

            if (alreadyEnrolled != null)
            {
                await _unitOfWork.CommitAsync();
                return alreadyEnrolled.Adapt<RS_Project>();
            }

            var draftClone = sourceProject.Adapt<Project>();
            draftClone.Id = Guid.NewGuid();
            draftClone.Genre = "proposal";
            draftClone.Status = Status.Draft.ToString().ToLowerInvariant();
            draftClone.CreatedAt = DateTime.Now;
            draftClone.UpdatedAt = DateTime.Now;

            // Clone ProjectMajors
            draftClone.ProjectMajors = sourceProject.ProjectMajors?
                .Select(pm => new ProjectMajor
                {
                    ProjectId = draftClone.Id,
                    MajorId = pm.MajorId,
                    Major = pm.Major
                }).ToList();

            // Clone ProjectTags
            draftClone.ProjectTags = sourceProject.ProjectTags?
                .Select(pt => new ProjectTag
                {
                    Id = Guid.NewGuid(),
                    ProjectId = draftClone.Id,
                    Name = pt.Name
                }).ToList();

            await _unitOfWork.GetProjectRepository().AddAsync(draftClone);
            await _unitOfWork.GetProjectMajorRepository().AddRangeAsync(draftClone.ProjectMajors);

            var piRole = await _unitOfWork.GetRoleRepository()
                .GetOneAsync(r => r.Name == "Principal Investigator");

            if (piRole == null)
                throw new ArgumentException("Role 'Principal Investigator' not found.");

            var userRole = new UserRole
            {
                Id = Guid.NewGuid(),
                AccountId = principalId,
                Code = $"UR-{DateTime.Now:yyyy_MM_dd}_{principalId:N}".Substring(0, 6),
                RoleId = piRole.Id,
                ProjectId = draftClone.Id,
                GroupName = draftClone.EnglishTitle,
                IsOfficial = false,
                ExpireDate = draftClone.CreatedAt.AddYears(1),
                CreatedAt = DateTime.Now,
                Status = Status.Approved.ToString().ToLowerInvariant()
            };

            await _unitOfWork.GetUserRoleRepository().AddAsync(userRole);

            var saved = await _unitOfWork.SaveChangesAsync();
            if (!saved)
                throw new Exception("Failed to save changes during enrollment.");

            await _unitOfWork.CommitAsync();
            return draftClone.Adapt<RS_Project>();
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
    //=============================================================================================
    //Create Milestone, task
    public async Task<bool> CreateFromDocumentAsync(RQ_MilestoneTaskContent content)
    {
        var existProject = await _unitOfWork.GetProjectRepository().GetByIdAsync(content.ProjectId, true)
            ?? throw new NotFoundException("Not found this project id to insert milestone!");
        if (string.IsNullOrWhiteSpace(content.DocumentContent)) throw new NotFoundException("Not found document content");
        var contentHtml = content.DocumentContent;

        var doc = new HtmlDocument();
        doc.LoadHtml(contentHtml);

        // Find section match SectionTitle
        var sectionTitle = HtmlAgilityPack.HtmlEntity.DeEntitize(content.SectionTitle);//Incase user type special character
        /*var sectionNode = doc.DocumentNode.SelectSingleNode($"//*[contains(text(), '{content.SectionTitle}')]") ??
           throw new NotFoundException("Not found that section content to identify data!");*/

        // Find nearest data table
        /*var tableNode = sectionNode.SelectSingleNode("following::table[0]") ??
            throw new NotFoundException("Not found any data table below the title!");*/

        // Find element sibling right after sectionNode or nearest parent
        var candidates = doc.DocumentNode.Descendants().Where(n => !string.IsNullOrWhiteSpace(n.InnerText) &&
                HtmlEntity.DeEntitize(n.InnerText)
                   .Contains(sectionTitle, StringComparison.OrdinalIgnoreCase));

        string[] preferTags = { "h1", "h2", "h3", "h4", "h5", "h6", "p", "strong", "span", "td", "th", "caption", "label" };
        string[] containerTags = { "section", "article", "div", "main", "header", "footer", "aside", "body" };

        var anchor = candidates
            .Where(n => preferTags.Contains(n.Name))
            .OrderByDescending(n => n.Ancestors().Count())                         // sâu nhất trước
            .ThenBy(n => HtmlEntity.DeEntitize(n.InnerText).Length)                // text ngắn (gần tiêu đề) trước
            .FirstOrDefault();

        if (anchor == null)
        {
            var first = candidates.First(); // fallback: refine xuống descendant “đẹp” hơn
            anchor = first.Descendants()
                .Where(n => preferTags.Contains(n.Name) &&
                            HtmlEntity.DeEntitize(n.InnerText)
                               .Contains(sectionTitle, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(n => n.Ancestors().Count())
                .ThenBy(n => HtmlEntity.DeEntitize(n.InnerText).Length)
                .FirstOrDefault()
                ?? first; // nếu không có thì dùng container luôn (ít gặp)
        }

        // --- table: ưu tiên bảng gần nhất sau anchor theo document order ---
        var tableNode = anchor.SelectSingleNode("following::table[1]");

        // nếu chưa thấy, bó trong container (section/div/...) và chọn table có StreamPosition > anchor
        if (tableNode == null)
        {
            var container = anchor.Ancestors().FirstOrDefault(a => containerTags.Contains(a.Name));
            if (container != null)
            {
                var anchorPos = anchor.StreamPosition;
                tableNode = container.SelectNodes(".//table")
                             ?.Where(t => t.StreamPosition > anchorPos)
                             .OrderBy(t => t.StreamPosition)
                             .FirstOrDefault();
            }
        }

        var rows = tableNode.SelectNodes(".//tr");
        if (rows is null || rows.Count == 0) throw new NotFoundException("Not found any row data in this table!");

        //Map table into array
        var headerRow = rows.FirstOrDefault(r => r.SelectNodes("th") != null) ??
            throw new Exception("Không tìm thấy dòng tiêu đề bảng!");

        var headerCells = headerRow.SelectNodes("th");
        var columnMap = headerCells.Select((cell, i) =>
        new
        {
            Text = HtmlEntity.DeEntitize(cell.InnerText).Replace('\u00A0', ' ').Trim(),
            Index = i
        }).ToDictionary(x => x.Text, x => x.Index);

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
        decimal totalMoney = 0m;

        //loop through table cell skip first row (header title)
        foreach (var row in rows.Skip(1))
        {
            //Regex Code
            var sequence = new Random().Next(1, 999).ToString("D3");

            var cells = row.SelectNodes("td");
            if (cells == null || cells.Count == 0) continue;
            var currentRowHtml = cells[descriptionIndex].InnerHtml.Trim();

            //Get data from Document
            var description = StringUtils.DecodeHtmlEntitiesText(cells.ElementAtOrDefault(descriptionIndex)?.InnerText.Trim());
            var title = description;//.Length <= 20 ? description : description.Substring(0, 20).Trim() + "...";
            var objective = StringUtils.DecodeHtmlEntitiesText(cells.ElementAtOrDefault(objectiveIndex)?.InnerText.Trim());
            var timeRaw = StringUtils.DecodeHtmlEntitiesText(cells.ElementAtOrDefault(timeIndex)?.InnerText.Trim());
            var costRaw = StringUtils.DecodeHtmlEntitiesText(cells.ElementAtOrDefault(costIndex)?.InnerText.Trim());


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

            //If the content is bold -> then it is milestone
            if (currentRowHtml.Contains("<strong>") ||
                currentRowHtml.Contains("<b>") ||
                currentRowHtml.Contains("weight=\"bold\"") ||
                currentRowHtml.Contains("weight:bold") ||
                currentRowHtml.Contains("<em>"))
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

                totalMoney += cost;
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
        existProject.Budget = totalMoney;
        await _unitOfWork.GetMilestoneRepository().AddRangeAsync(milestonesFromDoc);
        await _unitOfWork.GetTaskRepository().AddRangeAsync(tasksFromDoc);
        return await _unitOfWork.SaveChangesAsync();
    }

    
    public async Task<bool> ApproveProposalAsync(Guid proposalProjectId)
    {
        var repo = _unitOfWork.GetProjectRepository();
        var invalidStates = new[]
        {
        Status.InProgress.ToString().ToLower(),
        Status.Completed.ToString().ToLower(),
        Status.Cancelled.ToString().ToLower(),
        Status.Deleted.ToString().ToLower()
    };

        var proposal = await repo.GetByIdAsync(proposalProjectId, hasTrackings: true)
            ?? throw new KeyNotFoundException("Proposal project not found.");

        if (proposal.Status != Status.Submitted.ToString().ToLower())
            throw new InvalidOperationException("Only submitted proposals can be approved.");

        var alreadyApproved = await repo.AnyAsync(p =>
            p.Code == proposal.Code &&
            p.Id != proposal.Id &&
            p.Genre == "proposal" &&
            invalidStates.Contains(p.Status)
        );

        if (alreadyApproved)
            throw new InvalidOperationException("Another proposal has already been approved.");

        var sourceProject = await repo.GetOneAsync(p =>
            p.Code == proposal.Code &&
            (p.Genre.ToLower() == "propose" || p.Genre.ToLower() == "normal")
        );

        if (sourceProject != null && invalidStates.Contains(sourceProject.Status))
            throw new InvalidOperationException($"Cannot approve—source is {sourceProject.Status}.");

        sourceProject!.Status = Status.InProgress.ToString().ToLower();
        proposal.Status = Status.Approved.ToString().ToLower();

        await _unitOfWork.SaveChangesAsync();

        _taskQueueHandler.EnqueueTracked(async (serviceProvider, token, progress) =>
        {

            var scopedUow = serviceProvider.GetRequiredService<IUnitOfWork>();
            var notifService = serviceProvider.GetRequiredService<INotificationService>();
            var projRepo = scopedUow.GetProjectRepository();

            var others = await projRepo.GetListAsync(
                p => p.Code == proposal.Code &&
                     p.Id != proposal.Id &&
                     p.Status == Status.Submitted.ToString().ToLower(),
                hasTrackings: true,
                include: q => q.Include(p => p.Members)
            );

            foreach (var other in others)
            {
                other.Status = Status.Rejected.ToString().ToLower();
                await notifService.CreateNew(new RQ_Notification
                {
                    Title = $"Proposal Rejected {other.EnglishTitle}",
                    Type = "project",
                    ObjecNotificationId = other.Id,
                    IsGlobalSend = false,
                    ListAccountId = other.Members.Select(m => m.AccountId).Distinct().ToList()
                });

                progress.Report(20); 
            }

            await notifService.CreateNew(new RQ_Notification
            {
                Title = $"Proposal Approved {proposal.EnglishTitle}",
                Type = "project",
                ObjecNotificationId = proposal.Id,
                IsGlobalSend = false,
                ListAccountId = proposal.Members.Select(m => m.AccountId).Distinct().ToList()
            });

            progress.Report(60);

            var approvedMembers = await scopedUow.GetUserRoleRepository()
                                                 .GetListAsync(pm => pm.ProjectId == proposal.Id);
            foreach (var member in approvedMembers)
            {
                var account = await scopedUow.GetAccountRepository().GetByIdAsync(member.AccountId);
                if (!string.IsNullOrEmpty(account?.Email))
                {
                    await notifService.SendNotificationMail(new RQ_NotificationEmail
                    {
                        Subject = "[SRPM] Proposal Approval Notification",
                        ReceiverEmailAddress = account.Email,
                        Title = "Your Proposal Has Been Approved",
                        RefContent = $"Dear {account.FullName ?? "Researcher"},\n\n" +
                              $"We are pleased to inform you that your proposal titled \"{proposal.EnglishTitle}\" has been officially approved by the SRPM committee.\n\n" +
                              $"This decision reflects the quality and relevance of your work, and we look forward to seeing its impact in the upcoming stages of the project.\n\n" +
                              $"You may now proceed with the next steps outlined in your project plan. Please ensure all documentation and milestones are updated accordingly.\n\n" +
                              $"If you have any questions or require further assistance, feel free to reach out to your assigned coordinator.\n\n" +
                              $"Best regards,\nSRPM Administration Team",
                        CreatedDate = DateTime.Now,
                        RefTitle = "Proposal Approved",
                        Content = $"Project: {proposal.EnglishTitle}\nStatus: Approved",
                        RefButton = "View Proposal",
                        RefUrl = $"https://srpm.com/proposals/{proposal.Id}"
                    });
                }
            }

            progress.Report(100);
            await scopedUow.SaveChangesAsync();
        });

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