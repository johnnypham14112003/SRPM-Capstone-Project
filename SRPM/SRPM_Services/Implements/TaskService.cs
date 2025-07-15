using Mapster;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Services.BusinessModels;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Extensions.Enumerables;
using SRPM_Services.Extensions.Exceptions;
using SRPM_Services.Interfaces;

namespace SRPM_Services.Implements;

public class TaskService : ITaskService
{
    private readonly IUnitOfWork _unitOfWork;

    public TaskService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<RS_Task?> GetByIdAsync(Guid id)
    {
        var entity = await _unitOfWork.GetTaskRepository().GetByIdAsync<Guid>(id);
        return entity?.Adapt<RS_Task>();
    }

    public async Task<PagingResult<RS_Task>> GetListAsync(RQ_TaskQuery query)
    {
        query.PageIndex = query.PageIndex < 1 ? 1 : query.PageIndex;
        query.PageSize = query.PageSize < 1 ? 10 : query.PageSize;

        var allTasks = await _unitOfWork.GetTaskRepository().GetListAsync(t =>
            (string.IsNullOrWhiteSpace(query.Code) || t.Code == query.Code) &&
            (string.IsNullOrWhiteSpace(query.Name) || t.Name.Contains(query.Name)) &&
            (string.IsNullOrWhiteSpace(query.Priority) || t.Priority == query.Priority) &&
            (query.MilestoneId == null || t.MilestoneId == query.MilestoneId) &&
            (query.CreatorId == null || t.CreatorId == query.CreatorId) &&
            (string.IsNullOrWhiteSpace(query.Status) || t.Status == query.Status),
            hasTrackings: false
        );

        var total = allTasks.Count;

        if (total == 0)
            throw new NotFoundException("No Task records found matching the query.");

        var pageData = allTasks
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        return new PagingResult<RS_Task>
        {
            PageIndex = query.PageIndex,
            PageSize = query.PageSize,
            TotalCount = total,
            DataList = pageData.Adapt<List<RS_Task>>()
        };
    }


    public async Task<RS_Task> CreateAsync(RQ_Task request)
    {
        var entity = request.Adapt<SRPM_Repositories.Models.Task>();
        entity.Id = Guid.NewGuid();
        entity.Status = Status.Created.ToString().ToLowerInvariant();

        await _unitOfWork.GetTaskRepository().AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return entity.Adapt<RS_Task>();
    }

    public async Task<RS_Task?> UpdateAsync(Guid id, RQ_Task request)
    {
        var repo = _unitOfWork.GetTaskRepository();
        var entity = await repo.GetByIdAsync<Guid>(id);
        if (entity == null) return null;
        entity.Status = Status.Created.ToString().ToLowerInvariant();

        request.Adapt(entity);
        await repo.UpdateAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return entity.Adapt<RS_Task>();
    }

    public async Task<RS_Task?> ToggleStatusAsync(Guid id)
    {
        var repo = _unitOfWork.GetTaskRepository();
        var entity = await repo.GetByIdAsync<Guid>(id);
        if (entity == null) return null;

        var current = entity.Status.ToStatus();
        entity.Status = current == Status.Created ? Status.Deleted.ToString().ToLower() : Status.Created.ToString().ToLower();

        await repo.UpdateAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return entity.Adapt<RS_Task>();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var repo = _unitOfWork.GetTaskRepository();
        var entity = await repo.GetByIdAsync<Guid>(id);
        if (entity == null) return false;

        await repo.DeleteAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}