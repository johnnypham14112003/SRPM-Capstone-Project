using Mapster;
using Microsoft.EntityFrameworkCore;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Services.BusinessModels;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.RequestModels.Query;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.Implements
{
    public class ProjectResultService : IProjectResultService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContextService;
        public ProjectResultService(IUnitOfWork unitOfWork, IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _userContextService = userContextService;
        }
        public async Task<RS_ProjectResult> CreateAsync(RQ_ProjectResult request)
        {
            var repo = _unitOfWork.GetProjectResultRepository();
            var entity = request.Adapt<ProjectResult>();
            entity.Id = Guid.NewGuid(); 
            await repo.AddAsync(entity);

            if (request.ResultPublishs != null)
            {
                var publishRepo = _unitOfWork.GetResultPublishRepository();
                foreach (var pub in request.ResultPublishs)
                {
                    var newPub = pub.Adapt<ResultPublish>();
                    newPub.Id = Guid.NewGuid(); 
                    newPub.ProjectResultId = entity.Id;
                    newPub.PublicationDate ??= DateTime.UtcNow;
                    await publishRepo.AddAsync(newPub);
                }
            }

            await _unitOfWork.SaveChangesAsync();

            var result = entity.Adapt<RS_ProjectResult>();
            result.ResultPublishs = entity.ResultPublishs?.Adapt<List<RS_ResultPublish>>();
            return result;
        }
        public async Task<RS_ProjectResult> UpdateAsync(RQ_ProjectResult request)
        {
            if (!request.Id.HasValue)
                throw new ArgumentException("Id is required for update.");

            var repo = _unitOfWork.GetProjectResultRepository();
            var entity = await repo.GetOneAsync(
                p => p.Id == request.Id.Value,
                include: q => q.Include(p => p.ResultPublishs),
                hasTrackings: true
            ) ?? throw new Exception("ProjectResult not found");

            // Update main fields
            entity.Name = request.Name;
            entity.Url = request.Url;

            // Handle ResultPublishs
            if (request.ResultPublishs != null)
            {
                var publishRepo = _unitOfWork.GetResultPublishRepository();

                foreach (var pub in request.ResultPublishs)
                {
                    if (pub.Id.HasValue)
                    {
                        var existing = entity.ResultPublishs?.FirstOrDefault(p => p.Id == pub.Id.Value);
                        if (existing != null)
                        {
                            existing.Url = pub.Url;
                            existing.Title = pub.Title;
                            existing.Description = pub.Description;
                            existing.Publisher = pub.Publisher;
                            existing.PublicationDate = pub.PublicationDate ?? existing.PublicationDate;
                            existing.AccessType = pub.AccessType;
                            existing.Tags = pub.Tags;
                        }
                    }
                    else
                    {
                        var newPub = pub.Adapt<ResultPublish>();
                        newPub.ProjectResultId = entity.Id;
                        newPub.PublicationDate ??= DateTime.UtcNow;
                        await publishRepo.AddAsync(newPub);
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();

            // Reload updated entity if needed
            var updatedEntity = await repo.GetOneAsync(
                p => p.Id == entity.Id,
                include: q => q.Include(p => p.ResultPublishs),
                hasTrackings: false
            );

            var result = updatedEntity.Adapt<RS_ProjectResult>();
            result.ResultPublishs = updatedEntity.ResultPublishs?.Adapt<List<RS_ResultPublish>>();
            return result;
        }
        public async Task<PagingResult<RS_ProjectResult>> GetListAsync(Q_ProjectResult query)
        {
            query.PageIndex = query.PageIndex < 1 ? 1 : query.PageIndex;
            query.PageSize = query.PageSize < 1 ? 10 : query.PageSize;

            var repo = _unitOfWork.GetProjectResultRepository();

            var results = await repo.GetListAsync(
                p =>
                    (string.IsNullOrWhiteSpace(query.Name) || p.Name.Contains(query.Name)) &&
                    (!query.ProjectId.HasValue || p.ProjectId == query.ProjectId),
                include: q => q.Include(p => p.ResultPublishs),
                hasTrackings: false
            );

            var total = results.Count;
            var page = results
                .Skip((query.PageIndex - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            var data = page.Adapt<List<RS_ProjectResult>>();

            return new PagingResult<RS_ProjectResult>
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TotalCount = total,
                DataList = data
            };
        }
        public async Task<bool> DeleteAsync(Guid id)
        {
            var repo = _unitOfWork.GetProjectResultRepository();
            var entity = await repo.GetByIdAsync(id);
            if (entity == null) return false;

            await repo.DeleteAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteResultPublishAsync(Guid id)
        {
            var repo = _unitOfWork.GetResultPublishRepository();
            var entity = await repo.GetByIdAsync(id);
            if (entity == null) return false;

            await repo.DeleteAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        public async Task<RS_ProjectResult?> GetByIdAsync(Guid id)
        {
            var repo = _unitOfWork.GetProjectResultRepository();

            var entity = await repo.GetOneAsync(
                p => p.Id == id,
                include: q => q.Include(p => p.ResultPublishs),
                hasTrackings: false
            );

            if (entity == null) return null;

            var result = entity.Adapt<RS_ProjectResult>();
            result.ResultPublishs = entity.ResultPublishs?.Adapt<List<RS_ResultPublish>>();

            return result;
        }
    }
}
