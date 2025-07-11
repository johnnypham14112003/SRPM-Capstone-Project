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
using SRPM_Services.BusinessModels.Others;
using Microsoft.EntityFrameworkCore;

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
            var list = await _unitOfWork.GetMilestoneRepository().GetListAsync(
                m =>
                    (string.IsNullOrWhiteSpace(query.Code) || m.Code.Contains(query.Code)) &&
                    (string.IsNullOrWhiteSpace(query.Title) || m.Title.Contains(query.Title)) &&
                    (string.IsNullOrWhiteSpace(query.Description) || (m.Description ?? "").Contains(query.Description)) &&
                    (string.IsNullOrWhiteSpace(query.Objective) || (m.Objective ?? "").Contains(query.Objective)) &&
                    (!query.Cost.HasValue || m.Cost == query.Cost.Value) &&
                    (string.IsNullOrWhiteSpace(query.Type) || m.Type == query.Type) &&
                    (!query.StartDate.HasValue || m.StartDate >= query.StartDate.Value) &&
                    (!query.EndDate.HasValue || m.EndDate <= query.EndDate.Value) &&
                    (string.IsNullOrWhiteSpace(query.Status) || m.Status == query.Status) &&
                    (!query.ProjectId.HasValue || m.ProjectId == query.ProjectId.Value) &&
                    (!query.CreatorId.HasValue || m.CreatorId == query.CreatorId.Value),
                include: q => q
                    .Include(m => m.Project)
                    .Include(m => m.Creator).ThenInclude(c => c.Role)
                    .Include(m => m.Evaluations)
                    .Include(m => m.IndividualEvaluations)
                    .Include(m => m.Tasks),
                hasTrackings: false
            );


            // 🔄 Apply Sorting
            list = query.SortBy?.ToLower() switch
            {
                "title" => query.Desc ? list.OrderByDescending(x => x.Title).ToList() : list.OrderBy(x => x.Title).ToList(),
                "objective" => query.Desc ? list.OrderByDescending(x => x.Objective).ToList() : list.OrderBy(x => x.Objective).ToList(),
                "cost" => query.Desc ? list.OrderByDescending(x => x.Cost).ToList() : list.OrderBy(x => x.Cost).ToList(),
                "type" => query.Desc ? list.OrderByDescending(x => x.Type).ToList() : list.OrderBy(x => x.Type).ToList(),
                "startdate" => query.Desc ? list.OrderByDescending(x => x.StartDate).ToList() : list.OrderBy(x => x.StartDate).ToList(),
                "enddate" => query.Desc ? list.OrderByDescending(x => x.EndDate).ToList() : list.OrderBy(x => x.EndDate).ToList(),
                _ => list.OrderBy(x => x.Title).ToList() // Default
            };

            var total = list.Count;
            var paged = list
                .Skip((query.PageIndex - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            return new PagingResult<RS_Milestone>
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TotalCount = total,
                DataList = paged.Adapt<List<RS_Milestone>>()
            };
        }



        public async Task<RS_Milestone> CreateAsync(RQ_Milestone request)
        {
            var entity = request.Adapt<Milestone>();
            entity.Id = Guid.NewGuid();
            entity.CreatedAt = DateTime.Now;
            entity.Status = Status.Created.ToString().ToLowerInvariant();
            // e.g. MS-202507-001
            var yyyymm = DateTime.Now.ToString("yyyyMM");
            var sequence = new Random().Next(1, 999).ToString("D3");
            entity.Code = $"MS-{yyyymm}-{sequence}";


            await _unitOfWork.GetMilestoneRepository().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.Adapt<RS_Milestone>();
        }

        public async Task<RS_Milestone?> UpdateAsync(Guid id, RQ_Milestone request)
        {
            var repo = _unitOfWork.GetMilestoneRepository();
            var entity = await repo.GetByIdAsync<Guid>(id);
            if (entity == null) return null;
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
