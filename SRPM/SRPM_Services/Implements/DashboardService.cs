using Microsoft.EntityFrameworkCore;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Services.Extensions.Enumerables;
using SRPM_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.Implements
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContextService;

        public DashboardService(IUnitOfWork unitOfWork, IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _userContextService = userContextService;
        }

        public async Task<object> GetSystemStatsAsync(DateTime from, DateTime to)
        {
            var totalProjects = await _unitOfWork.GetProjectRepository().CountAsync(p => p.CreatedAt >= from && p.CreatedAt <= to);
            var completedProjects = await _unitOfWork.GetProjectRepository().CountAsync(p => p.Status == Status.Completed.ToString().ToLowerInvariant() && p.UpdatedAt >= from && p.UpdatedAt <= to);
            var proposalsSubmitted = await _unitOfWork.GetDocumentRepository().CountAsync(p => p.UploadAt  >= from && p.UploadAt <= to && p.IsTemplate == false  && p.Status == Status.Submitted.ToString().ToLowerInvariant());
            var activeUsers = await _unitOfWork.GetAccountRepository().CountAsync(u => u.Status == Status.Created.ToString().ToLowerInvariant());
            var evaluationsPerformed = await _unitOfWork.GetEvaluationRepository().CountAsync(e => e.CreateDate >= from && e.CreateDate <= to);

            return new
            {
                TotalProjectsCreated = totalProjects,
                ProjectsCompleted = completedProjects,
                ProposalsSubmitted = proposalsSubmitted,
                ActiveUsers = activeUsers,
                EvaluationsPerformed = evaluationsPerformed
            };
        }

        public async Task<object> GetUserProjectStatsAsync(DateTime from, DateTime to)
        {
            var accountId = Guid.Parse(_userContextService.GetCurrentUserId());
            var participatedProjects = await _unitOfWork.GetUserRoleRepository().CountAsync(ur => ur.AccountId == accountId && ur.CreatedAt >= from && ur.CreatedAt <= to);
            var pendingEvals = await _unitOfWork.GetIndividualEvaluationRepository().CountAsync(e => e.ReviewerId == accountId && e.Status == Status.InProgress.ToString().ToLowerInvariant());

            return new
            {
                ProjectsParticipated = participatedProjects,
                PendingEvaluations = pendingEvals
            };
        }

        public async Task<object> GetPrincipalDashboardAsync(DateTime from, DateTime to)
        {
            var accountId = Guid.Parse(_userContextService.GetCurrentUserId());
            var activeProposals = await _unitOfWork.GetDocumentRepository().CountAsync(p => p.UploaderId == accountId && p.Status == Status.Created.ToString().ToLowerInvariant() && p.UploadAt >= from && p.UploadAt <= to);
            var completedProjects = await _unitOfWork.GetProjectRepository().CountAsync(p => p.CreatorId == accountId && p.Status == "Completed");
            var fields = await _unitOfWork.GetProjectRepository().GetListAsync(
                p => p.CreatorId == accountId,
            include: p => p = p.Include(p => p.ProjectMajors)
                    .ThenInclude(pm => pm.Major)
                        .ThenInclude(m => m.Field)
            );

            return new
            {
                ActiveProposals = activeProposals,
                CompletedResearchProjects = completedProjects,
                FieldsInvolved = fields
            };
        }

        public async Task<object> GetInstitutionDashboardAsync(DateTime from, DateTime to)
        {
            var institutionId = Guid.Parse(_userContextService.GetCurrentUserId());
            var baselines = await _unitOfWork.GetProjectRepository().CountAsync(p => p.CreatorId == institutionId && p.Genre == "normal");
            var completed = await _unitOfWork.GetProjectRepository().CountAsync(p => p.CreatorId == institutionId && p.Status == Status.Completed.ToString().ToLowerInvariant());           
            var fields = await _unitOfWork.GetProjectRepository().GetListAsync(
                p => p.CreatorId == institutionId,
                include: p => p = p.Include(p => p.ProjectMajors)
                    .ThenInclude(pm => pm.Major)
                        .ThenInclude(m => m.Field)
            );

            return new
            {
                ProposeProjectsCreated = baselines,
                CompletedProjectsFromInstitution = completed,
                FieldsOfStudyRepresented = fields
            };
        }


        public async Task<List<object>> GetKpiTilesAsync( DateTime from, DateTime to)
        {
            var accountId = Guid.Parse(_userContextService.GetCurrentUserId());
            var activeProjects = await _unitOfWork.GetProjectRepository().CountAsync(p => p.CreatorId == accountId && p.Status == "Active");
            var completedMilestones = await _unitOfWork.GetMilestoneRepository().CountAsync(m => m.CreatorId == accountId && m.Status == Status.Completed.ToString().ToLowerInvariant() );

            return new List<object>
        {
            new { Label = "Active Projects", Value = activeProjects, IconKey = "chart-line", Unit = "projects", Tooltip = "Ongoing research initiatives" },
            new { Label = "Completed Milestones", Value = completedMilestones, IconKey = "check-circle", Unit = "tasks", Tooltip = "Delivered project goals" }
        };
        }
    }

}
