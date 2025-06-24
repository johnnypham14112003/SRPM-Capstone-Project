using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.BusinessModels;
using SRPM_Services.Extensions.Enumerables;
using SRPM_Services.Extensions.Exceptions;
using SRPM_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapster;

namespace SRPM_Services.Implements
{
    public class MilestoneService : IMilestoneService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MilestoneService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<RS_Milestone?> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.GetMilestoneRepository().GetByIdAsync<Guid>(id);
            return entity?.Adapt<RS_Milestone>();
        }

        public async Task<List<RS_Milestone>> GetByProjectAsync(Guid projectId)
        {
            var list = await _unitOfWork.GetMilestoneRepository().GetListAsync(m => m.ProjectId == projectId, hasTrackings: false);
            return list.Adapt<List<RS_Milestone>>();
        }

        public async Task<PagingResult<RS_Milestone>> GetListAsync(RQ_MilestoneQuery query)
        {
            query.PageIndex = query.PageIndex < 1 ? 1 : query.PageIndex;
            query.PageSize = query.PageSize < 1 ? 10 : query.PageSize;

            var list = await _unitOfWork.GetMilestoneRepository().GetListAsync(m =>
                (string.IsNullOrWhiteSpace(query.Code) || m.Code == query.Code) &&
                (string.IsNullOrWhiteSpace(query.Title) || m.Title.Contains(query.Title)) &&
                (string.IsNullOrWhiteSpace(query.Type) || m.Type == query.Type) &&
                (string.IsNullOrWhiteSpace(query.Status) || m.Status == query.Status) &&
                (query.ProjectId == null || m.ProjectId == query.ProjectId) &&
                (query.CreatorId == null || m.CreatorId == query.CreatorId),
                hasTrackings: false
            );

            var total = list.Count;
            if (total == 0)
                throw new NotFoundException("No milestones found.");

            var page = list
                .Skip((query.PageIndex - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            return new PagingResult<RS_Milestone>
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TotalCount = total,
                DataList = page.Adapt<List<RS_Milestone>>()
            };
        }

        public async Task<RS_Milestone> CreateAsync(RQ_Milestone request)
        {
            var entity = request.Adapt<Milestone>();
            entity.Id = Guid.NewGuid();
            entity.CreatedAt = DateTime.UtcNow;
            entity.Status = request.Status.ToStatus().ToString().ToLowerInvariant();

            await _unitOfWork.GetMilestoneRepository().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.Adapt<RS_Milestone>();
        }

        public async Task<RS_Milestone?> UpdateAsync(Guid id, RQ_Milestone request)
        {
            var repo = _unitOfWork.GetMilestoneRepository();
            var entity = await repo.GetByIdAsync<Guid>(id);
            if (entity == null) return null;

            entity.Status = request.Status.ToStatus().ToString().ToLowerInvariant();
            request.Adapt(entity);

            await repo.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.Adapt<RS_Milestone>();
        }

        public async Task<RS_Milestone?> ToggleStatusAsync(Guid id)
        {
            var repo = _unitOfWork.GetMilestoneRepository();
            var entity = await repo.GetByIdAsync<Guid>(id);
            if (entity == null) return null;

            var current = entity.Status.ToStatus();
            entity.Status = current == Status.Created ? Status.Deleted.ToString().ToLower() : Status.Created.ToString().ToLower();

            await repo.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.Adapt<RS_Milestone>();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var repo = _unitOfWork.GetMilestoneRepository();
            var entity = await repo.GetByIdAsync<Guid>(id);
            if (entity == null) return false;

            await repo.DeleteAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
