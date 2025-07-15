using Mapster;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Services.BusinessModels;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Interfaces;

namespace SRPM_Services.Implements;

public class ProjectMajorService : IProjectMajorService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProjectMajorService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagingResult<RS_ProjectMajor>> GetListAsync(RQ_ProjectMajorQuery query)
    {
        var list = await _unitOfWork.GetProjectMajorRepository()
            .GetListWithIncludesAsync(query.ProjectId, query.MajorId);

        var total = list.Count;
        var paged = list
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        return new PagingResult<RS_ProjectMajor>
        {
            PageIndex = query.PageIndex,
            PageSize = query.PageSize,
            TotalCount = total,
            DataList = paged.Adapt<List<RS_ProjectMajor>>()
        };
    }

    public async Task<RS_ProjectMajor> CreateAsync(RQ_ProjectMajor request)
    {
        var entity = request.Adapt<ProjectMajor>();

        await _unitOfWork.GetProjectMajorRepository().AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return entity.Adapt<RS_ProjectMajor>();
    }

    public async Task<RS_ProjectMajor?> UpdateAsync(Guid projectId, Guid majorId, RQ_ProjectMajor request)
    {
        var repo = _unitOfWork.GetProjectMajorRepository();
        var entity = await repo.GetOneAsync(pm =>
            pm.ProjectId == projectId && pm.MajorId == majorId);

        if (entity == null) return null;

        request.Adapt(entity);
        await repo.UpdateAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return entity.Adapt<RS_ProjectMajor>();
    }

    public async Task<bool> DeleteAsync(Guid projectId, Guid majorId)
    {
        var repo = _unitOfWork.GetProjectMajorRepository();
        var entity = await repo.GetOneAsync(pm =>
            pm.ProjectId == projectId && pm.MajorId == majorId);

        if (entity == null) return false;

        await repo.DeleteAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}