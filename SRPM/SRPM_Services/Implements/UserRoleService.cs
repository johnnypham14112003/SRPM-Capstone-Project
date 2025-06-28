using Mapster;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Extensions.Enumerables;
using SRPM_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.Implements
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserRoleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<RS_UserRole?> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.GetUserRoleRepository().GetByIdAsync<Guid>(id);
            return entity?.Adapt<RS_UserRole>();
        }
        public async Task<bool> UserHasRoleAsync(Guid accountId, string roleName)
        {
            var userRoles = await _unitOfWork.GetUserRoleRepository().GetListByFilterAsync(
                accountId: accountId,
                roleId: null,
                projectId: null,
                appraisalCouncilId: null,
                status: Status.Created.ToString().ToLower(),
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
                status: Status.Created.ToString().ToLower(),
                isOfficial: null
            );

            return roles
                .Where(r => r.Role != null)
                .Select(r => r.Role.Name)
                .Distinct()
                .ToList();
        }



        public async Task<List<RS_UserRole>> GetAllAsync()
        {
            var list = await _unitOfWork.GetUserRoleRepository()
                .GetListAsync(_ => true, hasTrackings: false);
            return list.Adapt<List<RS_UserRole>>();
        }

        public async Task<RS_UserRole> CreateAsync(RQ_UserRole request)
        {
            var entity = request.Adapt<UserRole>();
            entity.Id = Guid.NewGuid();
            entity.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.GetUserRoleRepository().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.Adapt<RS_UserRole>();
        }

        public async Task<RS_UserRole?> UpdateAsync(Guid id, RQ_UserRole request)
        {
            var repo = _unitOfWork.GetUserRoleRepository();
            var entity = await repo.GetByIdAsync<Guid>(id);
            if (entity == null) return null;

            var parsedStatus = request.Status.ToStatus();
            entity.Status = parsedStatus.ToString().ToLowerInvariant();
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
            var entity = await repo.GetByIdAsync<Guid>(id);
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

}
