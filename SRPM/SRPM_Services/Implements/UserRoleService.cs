using Mapster;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Services.BusinessModels;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Extensions.Enumerables;
using SRPM_Services.Extensions.Exceptions;
using SRPM_Services.Interfaces;

namespace SRPM_Services.Implements;

public class UserRoleService : IUserRoleService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserRoleService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<RS_UserRole?> GetByIdAsync(Guid id)
    {
        var entity = await _unitOfWork.GetUserRoleRepository().GetOneAsync(
            include: p => p
                .Include(ur => ur.Role)
                .Include(ur => ur.Account)
                .Include(ur => ur.Project)
                .Include(ur => ur.AppraisalCouncil),
            expression: ur => ur.Id == id // Assuming you're filtering by ID
        );

        return entity?.Adapt<RS_UserRole>();
    }
    public async Task<bool> UserHasRoleAsync(Guid accountId, string roleName)
    {
        var userRoles = await _unitOfWork.GetUserRoleRepository().GetListByFilterAsync(
            accountId: accountId,
            roleId: null,
            projectId: null,
            appraisalCouncilId: null,
            status: Status.Approved.ToString().ToLower(),
            isOfficial: null
        );

        return userRoles
            .Where(ur => ur.Role != null && ur.Role.Name == roleName)
            .Any();
    }

    public async Task<IEnumerable<string>> GetAllUserRole(Guid userId)
    {
        var roles = await _unitOfWork.GetUserRoleRepository().GetListByFilterAsync(
            accountId: userId,
            roleId: null,
            projectId: null,
            appraisalCouncilId: null,
            status: Status.Approved.ToString().ToLower(),
            isOfficial: null
        );

        return roles
            .Where(r => r.Role != null)
            .Select(r => r.Role.Name)
            .Distinct()
            .ToList();
    }

    public async Task<PagingResult<RS_UserRole>> GetListAsync(RQ_UserRoleQuery query)
    {
        var list = await _unitOfWork.GetUserRoleRepository().GetListByFilterAsync(
            query.AccountId,
            query.RoleId,
            query.ProjectId,
            query.AppraisalCouncilId,
            query.Status,
            query.IsOfficial
        );

        var total = list.Count;
        var paged = list
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        return new PagingResult<RS_UserRole>
        {
            PageIndex = query.PageIndex,
            PageSize = query.PageSize,
            TotalCount = total,
            DataList = paged.Adapt<List<RS_UserRole>>()
        };
    }

    public async Task<List<RS_UserRole>> GetAllAsync()
    {
        var list = await _unitOfWork.GetUserRoleRepository()
            .GetListAsync(_ => true, hasTrackings: false);
        return list.Adapt<List<RS_UserRole>>();
    }

    public async Task<RS_UserRole> CreateAsync(RQ_UserRole request)
    {
        var account = await _unitOfWork.GetAccountRepository().GetByIdAsync(request.AccountId);
        if (account == null)
            throw new NotFoundException($"Account with ID {request.AccountId} was not found.");

        var role = await _unitOfWork.GetRoleRepository().GetByIdAsync(request.RoleId);
        if (role == null)
            throw new NotFoundException($"Role with ID {request.RoleId} was not found.");

        string? groupName = null;
        bool isOfficial = false;
        DateTime? expireDate = null;

        // 🆕 Direct creation if both IDs are null
        if (request.AppraisalCouncilId == null && request.ProjectId == null)
        {
            // Proceed without council/project validation
        }
        else if (request.AppraisalCouncilId != null)
        {
            var council = await _unitOfWork.GetAppraisalCouncilRepository().GetByIdAsync(request.AppraisalCouncilId.Value);
            if (council == null)
                throw new NotFoundException($"Appraisal Council with ID {request.AppraisalCouncilId} was not found.");

            //if (council.CreatedAt <= DateTime.MinValue)
                //throw new InvalidOperationException("Appraisal Council created date is invalid.");

            isOfficial = true;
            expireDate = council.CreatedAt.AddYears(1);
            groupName = council.Name;
        }
        else
        {
            var project = await _unitOfWork.GetProjectRepository().GetByIdAsync(request.ProjectId!.Value);
            if (project == null)
                throw new NotFoundException($"Project with ID {request.ProjectId} was not found.");

            if (project.EndDate <= DateTime.Now)
                throw new InvalidOperationException("Cannot assign role to a project that has already ended.");

            isOfficial = false;
            expireDate = project.EndDate;
            groupName = project.EnglishTitle;

            // 🔐 Role constraints for project
            var projectRoles = await _unitOfWork.GetUserRoleRepository().GetListAsync(
                ur =>
                    ur.ProjectId == request.ProjectId &&
                    ur.Status != Status.Deleted.ToString().ToLowerInvariant(),
                hasTrackings: false
            );

            if (role.Name == "Principal Investigator")
            {
                bool hasPI = projectRoles!.Any(ur =>
                    ur.RoleId == request.RoleId &&
                    ur.AccountId != request.AccountId);

                if (hasPI)
                    throw new InvalidOperationException("This project already has a Principal Investigator.");
            }

            if (role.IsGroupRole)
            {
                var existingGroupRole = await _unitOfWork.GetUserRoleRepository().GetListAsync(
                    ur =>
                        ur.ProjectId == request.ProjectId &&
                        ur.RoleId == request.RoleId &&
                        ur.Status != Status.Deleted.ToString().ToLowerInvariant(),
                    hasTrackings: false
                );

                if (existingGroupRole != null && existingGroupRole.Any())
                    throw new InvalidOperationException($"Project already has a user assigned to the group role '{role.Name}'.");
            }
        }

        // 🔍 Check for existing role
        var existing = await _unitOfWork.GetUserRoleRepository().GetListAsync(
            ur =>
                ur.AccountId == request.AccountId &&
                ur.RoleId == request.RoleId &&
                ur.ProjectId == request.ProjectId &&
                ur.AppraisalCouncilId == request.AppraisalCouncilId &&
                ur.IsOfficial == isOfficial,
            hasTrackings: false
        );

        var match = existing?.FirstOrDefault();
        if (match != null)
            return match.Adapt<RS_UserRole>();

        var entity = request.Adapt<UserRole>();
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.Now;
        if (string.IsNullOrWhiteSpace(request.Code))
        {
            var initials = string.Join("", role.Name
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(word => word[0]))
                .ToUpperInvariant();
            var idFragment = entity.Id.ToString().Replace("-", "").Substring(0, 6).ToUpperInvariant();
            entity.Code = $"{initials}-{idFragment}";
        }
        else
        {
            entity.Code = request.Code;
        }
        entity.Status = request.Status != null ? request.Status.ToStatus().ToString().ToLower() : Status.Pending.ToString().ToLower();
        entity.IsOfficial = isOfficial;
        entity.ExpireDate = expireDate;
        entity.GroupName = groupName;

        await _unitOfWork.GetUserRoleRepository().AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return entity.Adapt<RS_UserRole>();
    }


    public async Task<RS_UserRole?> UpdateAsync(Guid id, RQ_UserRole request, string? status)
    {
        var repo = _unitOfWork.GetUserRoleRepository();
        var entity = await repo.GetOneAsync(
            include: p => p
                .Include(ur => ur.Role)
                .Include(ur => ur.Account)
                .Include(ur => ur.Project)
                .Include(ur => ur.AppraisalCouncil),
            expression: ur => ur.Id == id 
        );
        if (entity == null) return null;
        if (status.ToLowerInvariant() != Status.Rejected.ToString().ToLowerInvariant() &&
            status.ToLowerInvariant() != Status.Approved.ToString().ToLowerInvariant() &&
            status.ToLowerInvariant() != Status.Pending.ToString().ToLowerInvariant())
        {
            throw new BadRequestException("Invalid Status.");
        }
        entity.Status = status.ToLowerInvariant();
        request.Adapt(entity);

        await repo.UpdateAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return entity.Adapt<RS_UserRole>();
    }


    public async Task<bool> DeleteAsync(Guid id)
    {
        var repo = _unitOfWork.GetUserRoleRepository();
        var entity = await repo.GetByIdAsync<Guid>(id);
        if (entity == null) return false;

        await repo.DeleteAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
    public async Task<RS_UserRole?> ToggleStatusAsync(Guid id)
    {
        var repo = _unitOfWork.GetUserRoleRepository();
        var entity = await repo.GetOneAsync(
            include: p => p
                .Include(ur => ur.Role)
                .Include(ur => ur.Account)
                .Include(ur => ur.Project)
                .Include(ur => ur.AppraisalCouncil),
            expression: ur => ur.Id == id
        );
        if (entity == null) return null;

        entity.Status = entity.Status.ToStatus() switch
        {
            Status.Created => Status.Deleted.ToString().ToLower(),
            Status.Deleted => Status.Created.ToString().ToLower(),
            _ => throw new InvalidOperationException("Unexpected status value.")
        };


        await repo.UpdateAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return entity.Adapt<RS_UserRole>();
    }

}