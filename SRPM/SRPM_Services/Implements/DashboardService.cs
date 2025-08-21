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
            // Total source projects created (genre: propose or normal)
            var totalProjects = await _unitOfWork.GetProjectRepository().CountAsync(
                p => (p.Genre.ToLower() == "propose" || p.Genre.ToLower() == "normal") &&
                     p.CreatedAt >= from && p.CreatedAt <= to
            );

            // Completed source projects (status: completed)
            var completedProjects = await _unitOfWork.GetProjectRepository().CountAsync(
                p => p.Status.ToLower() == Status.Completed.ToString().ToLowerInvariant() &&
                     p.UpdatedAt >= from && p.UpdatedAt <= to
            );

            // Active source projects (status: inprogress)
            var activeProjects = await _unitOfWork.GetProjectRepository().CountAsync(
                p => (p.Genre.ToLower() == "propose" || p.Genre.ToLower() == "normal") &&
                     p.Status.ToLower() == Status.InProgress.ToString().ToLowerInvariant()
            );


            // Active users (status: created)
            var activeUsers = await _unitOfWork.GetAccountRepository().CountAsync(
                u => u.Status == Status.Created.ToString().ToLowerInvariant()
            );

            // Total evaluations performed
            var totalEvaluations = await _unitOfWork.GetEvaluationRepository().CountAsync(
                e => e.CreateDate >= from && e.CreateDate <= to
            );

            // Total milestones created
            var totalMilestones = await _unitOfWork.GetMilestoneRepository().CountAsync(
                m => m.CreatedAt >= from && m.CreatedAt <= to
            );

            return new
            {
                TotalProjectsCreated = totalProjects,
                CompletedProjects = completedProjects,
                ActiveProjects = activeProjects,
                ActiveUsers = activeUsers,
                TotalEvaluations = totalEvaluations,
                TotalMilestones = totalMilestones
            };
        }
        public async Task<object> GetSourceProjectStatusStatsAsync(DateTime from, DateTime to)
        {
            var repository = _unitOfWork.GetProjectRepository();

            var inProgressCount = await repository.CountAsync(p =>
                p.Status == Status.InProgress.ToString().ToLowerInvariant() &&
                p.Genre != null &&
                (p.Genre.ToLower() == "propose" || p.Genre.ToLower() == "normal") &&
                p.CreatedAt >= from && p.CreatedAt <= to
            );

            var createdCount = await repository.CountAsync(p =>
                p.Status == Status.Created.ToString().ToLowerInvariant() &&
                p.Genre != null &&
                (p.Genre.ToLower() == "propose" || p.Genre.ToLower() == "normal") &&
                p.CreatedAt >= from && p.CreatedAt <= to
            );

            var cancelledCount = await repository.CountAsync(p =>
                p.Status == Status.Cancelled.ToString().ToLowerInvariant() &&
                p.Genre != null &&
                (p.Genre.ToLower() == "propose" || p.Genre.ToLower() == "normal") &&
                p.CreatedAt >= from && p.CreatedAt <= to
            );

            var completedCount = await repository.CountAsync(p =>
                p.Status == Status.Completed.ToString().ToLowerInvariant() &&
                p.Genre != null &&
                (p.Genre.ToLower() == "propose" || p.Genre.ToLower() == "normal") &&
                p.CreatedAt >= from && p.CreatedAt <= to
            );

            return new
            {
                InProgress = inProgressCount,
                Created = createdCount,
                Cancelled = cancelledCount,
                Completed = completedCount
            };
        }
        public async Task<List<object>> GetMajorDistributionStatsAsync(DateTime from, DateTime to)
        {
            var projects = await _unitOfWork.GetProjectRepository().GetListAsync(
                p => p.CreatedAt >= from && p.CreatedAt <= to &&
                     p.ProjectMajors != null &&
                     (p.Genre.ToLower() == "propose" || p.Genre.ToLower() == "normal"),
                include: p => p.Include(pj => pj.ProjectMajors).ThenInclude(pm => pm.Major)
            );

            var majorGroups = projects
                .SelectMany(p => p.ProjectMajors
                    .Where(pm => pm.Major != null)
                    .Select(pm => pm.Major.Name.Trim()))
                .GroupBy(majorName => majorName)
                .Select(g => new
                {
                    MajorName = g.Key,
                    ProjectCount = g.Count()
                })
                .OrderByDescending(g => g.ProjectCount)
                .ToList<object>();

            return majorGroups;
        }
        public async Task<List<object>> GetBaseUserRoleDistributionAsync()
        {
            var baseRoles = await _unitOfWork.GetUserRoleRepository().GetListAsync(
                ur => ur.ProjectId == null && ur.AppraisalCouncilId == null,
                include: q => q.Include(ur => ur.Role),
                hasTrackings: false
            );

            var roleGroups = baseRoles
                .Where(ur => ur.Role != null)
                .GroupBy(ur => ur.Role.Name.Trim())
                .Select(g => new
                {
                    RoleName = g.Key,
                    UserCount = g.Count()
                })
                .OrderByDescending(g => g.UserCount)
                .ToList<object>();

            return roleGroups;
        }
        public async Task<Dictionary<string, object>> GetTimeSeriesStatsAsync(DateTime? from, DateTime to, string granularity = "daily")
        {
            // Step 1: Determine the earliest data point across all entities
            var projectRepo = _unitOfWork.GetProjectRepository();
            var evalRepo = _unitOfWork.GetEvaluationRepository();
            var milestoneRepo = _unitOfWork.GetMilestoneRepository();
            var userRepo = _unitOfWork.GetAccountRepository();
            var transactionRepo = _unitOfWork.GetTransactionRepository();

            // Fetch minimal data to determine earliest dates
            var projectDates = await projectRepo.GetListAsync(p => true, hasTrackings: false);
            var evalDates = await evalRepo.GetListAsync(e => true, hasTrackings: false);
            var milestoneDates = await milestoneRepo.GetListAsync(m => true, hasTrackings: false);
            var userDates = await userRepo.GetListAsync(u => true, hasTrackings: false);
            var transactionDates = await transactionRepo.GetListAsync(t => true, hasTrackings: false);

            var earliestDates = new List<DateTime?> {
                projectDates!.MinBy(p => p.CreatedAt)?.CreatedAt,
                evalDates!.MinBy(e => e.CreateDate)?.CreateDate,
                milestoneDates!.MinBy(m => m.CreatedAt)?.CreatedAt,
                userDates!.MinBy(u => u.CreateTime)?.CreateTime,
                transactionDates!.MinBy(t => t.RequestDate)?.RequestDate
            };

            var startDate = from ?? earliestDates.Where(d => d.HasValue).Min() ?? DateTime.Now.AddMonths(-1);
            // Step 2: Generate time buckets
            var dateGroups = new List<DateTime>();

            if (granularity == "daily")
            {
                var totalDays = (to.Date - startDate.Date).Days;
                if (totalDays > 90) granularity = "weekly"; // auto-switch to weekly if too large
            }

            if (granularity == "daily")
            {
                dateGroups = Enumerable.Range(0, (to - startDate).Days + 1)
                    .Select(offset => startDate.Date.AddDays(offset))
                    .ToList();
            }
            else if (granularity == "weekly")
            {
                var start = startDate.Date;
                while (start <= to)
                {
                    dateGroups.Add(start);
                    start = start.AddDays(7);
                }
            }
            else if (granularity == "monthly")
            {
                var start = new DateTime(startDate.Year, startDate.Month, 1);
                while (start <= to)
                {
                    dateGroups.Add(start);
                    start = start.AddMonths(1);
                }
            }

            // Step 3: Fetch data within range
            var projects = await _unitOfWork.GetProjectRepository().GetListAsync(
                p => p.CreatedAt >= startDate && p.CreatedAt <= to &&
                     (p.Genre.ToLower() == "propose" || p.Genre.ToLower() == "normal"),
                hasTrackings: false
            );

            var evaluations = await _unitOfWork.GetEvaluationRepository().GetListAsync(
                e => e.CreateDate >= startDate && e.CreateDate <= to,
                hasTrackings: false
            );

            var milestones = await _unitOfWork.GetMilestoneRepository().GetListAsync(
                m => m.CreatedAt >= startDate && m.CreatedAt <= to,
                hasTrackings: false
            );

            var users = await _unitOfWork.GetAccountRepository().GetListAsync(
                u => u.CreateTime >= startDate && u.CreateTime <= to,
                hasTrackings: false
            );

            var transactions = await _unitOfWork.GetTransactionRepository().GetListAsync(
                t => t.RequestDate >= startDate && t.RequestDate <= to,
                hasTrackings: false
            );

            // Step 4: Aggregate stats
            var stats = new Dictionary<string, object>();

            foreach (var groupStart in dateGroups)
            {
                DateTime groupEnd = granularity switch
                {
                    "daily" => groupStart,
                    "weekly" => groupStart.AddDays(6),
                    "monthly" => groupStart.AddMonths(1).AddDays(-1),
                    _ => groupStart
                };

                stats[groupStart.ToString("yyyy-MM-dd")] = new
                {
                    Projects = projects.Count(p => p.CreatedAt.Date >= groupStart && p.CreatedAt.Date <= groupEnd),
                    Evaluations = evaluations.Count(e => e.CreateDate.Date >= groupStart && e.CreateDate.Date <= groupEnd),
                    Milestones = milestones.Count(m => m.CreatedAt.Date >= groupStart && m.CreatedAt.Date <= groupEnd),
                    Users = users.Count(u => u.CreateTime.Date >= groupStart && u.CreateTime.Date <= groupEnd),
                    Transactions = transactions.Count(t => t.RequestDate.Date >= groupStart && t.RequestDate.Date <= groupEnd)
                };
            }

            return stats;
        }
        public async Task<object> GetTransactionStatsAsync(DateTime from, DateTime to)
        {
            var repo = _unitOfWork.GetTransactionRepository();

            // Get all transactions in the time frame
            var transactions = await repo.GetListAsync(
                t => t.RequestDate >= from && t.RequestDate <= to,
                hasTrackings: false
            );
            var totalCount = transactions!.Count;
            var totalMoney = transactions.Sum(t => t.TotalMoney);
            var pendingCount = transactions.Count(t => t.Status.ToLower() == Status.Pending.ToString().ToLower());

            // Monthly average
            var totalMonths = Math.Max(1, ((to.Year - from.Year) * 12 + to.Month - from.Month + 1));
            var averageMonthly = totalCount / totalMonths;

            // Processing rate: completed / (completed + pending + cancelled)
            var completedCount = transactions.Count(t => t.Status.ToLower() == Status.Completed.ToString().ToLower());
            var processingBase = transactions.Count(t =>
                t.Status.ToLower() == Status.Completed.ToString().ToLower() ||
                t.Status.ToLower() == Status.Pending.ToString().ToLower() ||
                t.Status.ToLower() == Status.Cancelled.ToString().ToLower()
            );

            var processingRate = processingBase > 0
                ? Math.Round((double)completedCount / processingBase * 100, 2)
                : 0;

            return new
            {
                TotalTransactions = totalCount,
                TotalMoney = totalMoney,
                PendingTransactions = pendingCount,
                AverageMonthly = averageMonthly,
                ProcessingRate = $"{processingRate}%" // e.g. "76.92%"
            };
        }
        public async Task<object> GetMilestoneProgressStatsAsync()
        {
            var repo = _unitOfWork.GetMilestoneRepository();

            // Fetch all milestones
            var milestones = await repo.GetListAsync(p => true,hasTrackings: false);

            var totalCount = milestones.Count;

            // Status breakdown
            var completedCount = milestones.Count(m =>
                m.Status.ToLower() == Status.Completed.ToString().ToLower());

            var inProgressCount = milestones.Count(m =>
                m.Status.ToLower() == Status.InProgress.ToString().ToLower());

            var cancelledCount = milestones.Count(m =>
                m.Status.ToLower() == Status.Cancelled.ToString().ToLower());

            var relevantCount = totalCount - completedCount;

            // Completion rate
            var completionRate = relevantCount > 0
                ? Math.Round((double)completedCount / relevantCount * 100, 2)
                : 0;

            return new
            {
                TotalMilestones = totalCount,
                CompletedMilestones = completedCount,
                InProgressMilestones = inProgressCount,
                CancelledMilestones = cancelledCount,
                CompletionRate = $"{completionRate}%"
            };
        }
        public async Task<object> GetCouncilProjectStatsAsync()
        {
            var fieldRepo = _unitOfWork.GetFieldRepository();
            var majorRepo = _unitOfWork.GetMajorRepository();
            var userRoleRepo = _unitOfWork.GetUserRoleRepository();
            var projectRepo = _unitOfWork.GetProjectRepository();

            // Total Fields & Majors
            var totalFields = await fieldRepo.CountAsync(p => true);
            var totalMajors = await majorRepo.CountAsync(p => true);

            // Total Council Members: users with role "CouncilMember" and linked to a project + appraisal council
            var councilMembers = await userRoleRepo.GetListAsync(
                u => u.Role.Name.ToLower() == "Appraisal Council".ToLower() &&
                     u.ProjectId == null &&
                     u.AppraisalCouncilId == null,
                hasTrackings: false
            );
            var totalCouncilMembers = councilMembers.Count;

            // Average projects per council
            var projects = await projectRepo.GetListAsync(
                p => p.Genre == "propose" || p.Genre == "normal",
                hasTrackings: false
            );

            // Flatten all evaluations from all projects
            var allEvaluations = projects
                .SelectMany(p => p.Evaluations)
                .Where(e => e.AppraisalCouncilId != null)
                .ToList();

            // Group evaluations by AppraisalCouncilId
            var councilGroups = allEvaluations
                .GroupBy(e => e.AppraisalCouncilId);

            // Total distinct councils
            var totalCouncils = councilGroups.Count();

            // Count distinct projects per council
            var averageProjectsPerCouncil = totalCouncils > 0
                ? Math.Round(
                    councilGroups.Average(g => g
                        .Select(e => e.ProjectId)
                        .Distinct()
                        .Count()
                    ), 2)
                : 0;

            return new
            {
                TotalFields = totalFields,
                TotalMajors = totalMajors,
                TotalCouncilMembers = totalCouncilMembers,
                AverageProjectsPerCouncil = averageProjectsPerCouncil
            };
        }
    }

}
