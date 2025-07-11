using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.Implements
{
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContextService;

        public ProjectService(IUnitOfWork unitOfWork, IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _userContextService = userContextService;
        }
        public async Task<RS_Project?> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.GetProjectRepository().GetByIdAsync(id, hasTrackings: false);
            return entity?.Adapt<RS_Project>();
        }
        public async Task<PagingResult<RS_Project>> GetListAsync(RQ_ProjectQuery query)
        {
            query.PageIndex = query.PageIndex < 1 ? 1 : query.PageIndex;
            query.PageSize = query.PageSize < 1 ? 10 : query.PageSize;

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
                    (string.IsNullOrWhiteSpace(query.MajorName) ||
                        p.ProjectMajors.Any(pm => pm.Major.Name.Contains(query.MajorName))) &&
                    (string.IsNullOrWhiteSpace(query.FieldName) ||
                        p.ProjectMajors.Any(pm => pm.Major.Field.Name.Contains(query.FieldName))) &&
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

                    if (query.IncludeIndividualEvaluations)
                        q = q.Include(p => p.IndividualEvaluations);

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
                "createdat" => query.Desc ? projects.OrderByDescending(p => p.CreatedAt).ToList() : projects.OrderBy(p => p.CreatedAt).ToList(),
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
            // Ensure abbreviation is safe and trimmed
            var sanitizedAbbr = (entity.Abbreviations ?? "XXX").Trim().ToUpperInvariant();

            // Format: RP2025_06_ABC
            entity.Code = $"RP-{DateTime.Now:yyyy_MM}{sanitizedAbbr}";

            var accountId = Guid.Parse(_userContextService.GetCurrentUserId());

            var userRoles = await _unitOfWork.GetUserRoleRepository().GetListByFilterAsync(
                accountId: accountId,
                roleId: null,
                projectId: null,
                appraisalCouncilId: null,
                status: Status.Created.ToString().ToLower(),
                isOfficial: null
            );

            // Filter for the correct role name (Role is already included)
            var hostInstitutionUserRole = userRoles
                .FirstOrDefault(ur => ur.Role != null && ur.Role.Name == "Host Institution");

            if (hostInstitutionUserRole == null)
                throw new UnauthorizedException("Not authorized");

            entity.CreatorId = hostInstitutionUserRole.Id;


            entity.Status = Status.Created.ToString().ToLowerInvariant();
            await _unitOfWork.GetProjectRepository().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.Adapt<RS_Project>();
        }

        public async Task<RS_Project?> UpdateAsync(Guid id, RQ_Project request)
        {
            var repo = _unitOfWork.GetProjectRepository();
            var entity = await repo.GetByIdAsync<Guid>(id);
            if (entity == null) return null;
            entity.UpdatedAt = DateTime.Now;
            request.Adapt(entity);

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
    }


}
