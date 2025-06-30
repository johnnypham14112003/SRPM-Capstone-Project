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
    public class MemberTaskService : IMemberTaskService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MemberTaskService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<RS_MemberTask?> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.GetMemberTaskRepository().GetByIdAsync<Guid>(id);
            return entity?.Adapt<RS_MemberTask>();
        }

        public async Task<PagingResult<RS_MemberTask>> GetListAsync(RQ_MemberTaskQuery query)
        {
            var list = await _unitOfWork.GetMemberTaskRepository().GetListAsync(x =>
                (query.MemberId == null || x.MemberId == query.MemberId) &&
                (query.TaskId == null || x.TaskId == query.TaskId) &&
                (string.IsNullOrWhiteSpace(query.Status) || x.Status == query.Status),
                hasTrackings: false);

            var total = list.Count;
            if (total == 0)
                throw new NotFoundException("No member task records found.");

            var page = list
                .Skip((query.PageIndex - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            return new PagingResult<RS_MemberTask>
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TotalCount = total,
                DataList = page.Adapt<List<RS_MemberTask>>()
            };
        }

        public async Task<RS_MemberTask> CreateAsync(RQ_MemberTask request)
        {
            var entity = request.Adapt<MemberTask>();
            entity.Id = Guid.NewGuid();
            entity.JoinedAt = DateTime.UtcNow;
            entity.Status = Status.Created.ToString().ToLowerInvariant();

            await _unitOfWork.GetMemberTaskRepository().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.Adapt<RS_MemberTask>();
        }

        public async Task<RS_MemberTask?> UpdateAsync(Guid id, RQ_MemberTask request)
        {
            var repo = _unitOfWork.GetMemberTaskRepository();
            var entity = await repo.GetByIdAsync<Guid>(id);
            if (entity == null) return null;

            entity.Status = request.Status.ToStatus().ToString().ToLowerInvariant();
            request.Adapt(entity);

            await repo.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.Adapt<RS_MemberTask>();
        }

        public async Task<RS_MemberTask?> ToggleStatusAsync(Guid id)
        {
            var repo = _unitOfWork.GetMemberTaskRepository();
            var entity = await repo.GetByIdAsync<Guid>(id);
            if (entity == null) return null;

            var parsed = entity.Status.ToStatus();
            entity.Status = parsed == Status.Created
                ? Status.Deleted.ToString().ToLower()
                : Status.Created.ToString().ToLower();

            await repo.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.Adapt<RS_MemberTask>();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var repo = _unitOfWork.GetMemberTaskRepository();
            var entity = await repo.GetByIdAsync<Guid>(id);
            if (entity == null) return false;

            await repo.DeleteAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }

}
