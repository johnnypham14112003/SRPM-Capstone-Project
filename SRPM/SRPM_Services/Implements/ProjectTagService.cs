using Mapster;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Interfaces;

namespace SRPM_Services.Implements;

public class ProjectTagService : IProjectTagService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProjectTagService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<RS_ProjectTag?> GetByIdAsync(Guid id)
    {
        var tag = await _unitOfWork.GetProjectTagRepository().GetByIdAsync<Guid>(id);
        return tag?.Adapt<RS_ProjectTag>();
    }

    public async Task<List<RS_ProjectTag>> GetByProjectIdAsync(Guid projectId)
    {
        var list = await _unitOfWork.GetProjectTagRepository()
            .GetListAsync(t => t.ProjectId == projectId, hasTrackings: false);
        return list.Adapt<List<RS_ProjectTag>>();
    }

    public async Task<List<RS_ProjectTag>> CreateAsync(RQ_ProjectTag request)
    {
        var entities = request.Names
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => new ProjectTag
            {
                Id = Guid.NewGuid(),
                Name = name,
                ProjectId = request.ProjectId
            }).ToList();

        await _unitOfWork.GetProjectTagRepository().AddRangeAsync(entities);
        await _unitOfWork.SaveChangesAsync();

        return entities.Adapt<List<RS_ProjectTag>>();
    }
    public async Task<RS_ProjectTag?> UpdateAsync(Guid id, RQ_ProjectTag request)
    {
        var repo = _unitOfWork.GetProjectTagRepository();
        var entity = await repo.GetByIdAsync<Guid>(id);
        if (entity == null) return null;

        request.Adapt(entity);
        await repo.UpdateAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return entity.Adapt<RS_ProjectTag>();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var repo = _unitOfWork.GetProjectTagRepository();
        var entity = await repo.GetByIdAsync<Guid>(id);
        if (entity == null) return false;

        await repo.DeleteAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}