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
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<RS_Role?> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.GetRoleRepository().GetByIdAsync<Guid>(id);
            return entity?.Adapt<RS_Role>();
        }

        public async Task<List<RS_Role>> GetAllAsync()
        {
            var list = await _unitOfWork.GetRoleRepository().GetListAsync(_ => true, hasTrackings: false);
            return list.Adapt<List<RS_Role>>();
        }

        public async Task<RS_Role> CreateAsync(RQ_Role request)
        {
            var entity = request.Adapt<Role>();
            entity.Id = Guid.NewGuid();
            entity.Status = Status.Created.ToString().ToLowerInvariant();

            await _unitOfWork.GetRoleRepository().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.Adapt<RS_Role>();
        }

        public async Task<RS_Role?> UpdateAsync(Guid id, RQ_Role request)
        {
            var repo = _unitOfWork.GetRoleRepository();
            var entity = await repo.GetByIdAsync<Guid>(id);
            if (entity == null) return null;

            var parsedStatus = request.Status.ToStatus();
            entity.Status = parsedStatus.ToString().ToLowerInvariant();

            request.Adapt(entity);
            await repo.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.Adapt<RS_Role>();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var repo = _unitOfWork.GetRoleRepository();
            var entity = await repo.GetByIdAsync<Guid>(id);
            if (entity == null) return false;

            await repo.DeleteAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        public async Task<RS_Role?> ToggleStatusAsync(Guid id)
        {
            var repo = _unitOfWork.GetRoleRepository();
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

            return entity.Adapt<RS_Role>();
        }

    }

}
