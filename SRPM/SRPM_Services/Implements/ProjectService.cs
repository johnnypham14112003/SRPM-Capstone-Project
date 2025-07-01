using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Services.BusinessModels;
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
            var entity = await _unitOfWork.GetProjectRepository().GetByIdAsync<Guid>(id);
            return entity?.Adapt<RS_Project>();
        }

        public async Task<PagingResult<RS_Project>> GetListAsync(RQ_ProjectQuery query)
        {
            query.PageIndex = query.PageIndex < 1 ? 1 : query.PageIndex;
            query.PageSize = query.PageSize < 1 ? 10 : query.PageSize;

            var list = await _unitOfWork.GetProjectRepository().GetListAsync(p =>
                (string.IsNullOrWhiteSpace(query.Code) || p.Code == query.Code) &&
                (string.IsNullOrWhiteSpace(query.EnglishTitle) || p.EnglishTitle.Contains(query.EnglishTitle)) &&
                (string.IsNullOrWhiteSpace(query.VietnameseTitle) || p.VietnameseTitle.Contains(query.VietnameseTitle)) &&
                (string.IsNullOrWhiteSpace(query.Category) || p.Category == query.Category) &&
                (string.IsNullOrWhiteSpace(query.Type) || p.Type == query.Type) &&
                (string.IsNullOrWhiteSpace(query.Genre) || p.Genre == query.Genre) &&
                (string.IsNullOrWhiteSpace(query.Status) || p.Status == query.Status) &&
                (query.CreatorId == null || p.CreatorId == query.CreatorId),
                hasTrackings: false);

            var total = list.Count;
            if (total == 0)
                throw new NotFoundException("No matching projects found.");

            var page = list
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
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            // Ensure abbreviation is safe and trimmed
            var sanitizedAbbr = (entity.Abbreviations ?? "XXX").Trim().ToUpperInvariant();

            // Format: RP2025_06_ABC
            entity.Code = $"RP-{DateTime.UtcNow:yyyy_MM}{sanitizedAbbr}";

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
            entity.UpdatedAt = DateTime.UtcNow;
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
