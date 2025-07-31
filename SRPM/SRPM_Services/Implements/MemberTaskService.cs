using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.BusinessModels;
using SRPM_Services.Extensions.Enumerables;
using SRPM_Services.Extensions.Exceptions;
using SRPM_Services.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace SRPM_Services.Implements;

public class MemberTaskService : IMemberTaskService
{
    private readonly IUnitOfWork _unitOfWork;

    public MemberTaskService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<RS_MemberTask?> GetByIdAsync(Guid id)
    {
        var entity = await _unitOfWork.GetMemberTaskRepository().GetOneAsync(p => p.Id == id, include: m => m
                .Include(m => m.Member).ThenInclude(m => m.Account)
                .Include(m => m.Member).ThenInclude(m => m.Role)
                .Include(m => m.Task));
        return entity?.Adapt<RS_MemberTask>();
    }

    public async Task<PagingResult<RS_MemberTask>> GetListAsync(RQ_MemberTaskQuery query)
    {
        var list = await _unitOfWork.GetMemberTaskRepository().GetListAsync(x =>
            (query.MemberId == null || x.MemberId == query.MemberId) &&
            (query.TaskId == null || x.TaskId == query.TaskId) &&
            (string.IsNullOrWhiteSpace(query.Status) || x.Status == query.Status),
            hasTrackings: false,
            include: m => m
                .Include(m => m.Member).ThenInclude(m => m.Account)
                .Include(m => m.Member).ThenInclude(m => m.Role)
                .Include(m => m.Task)
            );

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
            try
            {

                // Step 2: Get the Task to retrieve its MilestoneId
                var task = await _unitOfWork.GetTaskRepository().GetByIdAsync(request.TaskId);
                if (task == null)
                    throw new BadRequestException("Task not found");

                // Step 3: Get the Milestone to retrieve ProjectId
                var milestone = await _unitOfWork.GetMilestoneRepository().GetByIdAsync(task.MilestoneId);
                if (milestone == null)
                    throw new BadRequestException("Milestone not found");

                var projectId = milestone.ProjectId;

                // Step 4: Find user role for this project and user, excluding group roles
                var userRole = await _unitOfWork.GetUserRoleRepository()
                    .GetOneAsync(ur =>
                        ur.AccountId == request.MemberId &&
                        ur.ProjectId == projectId &&
                        ur.Role.IsGroupRole == false,
                        include: ur => 
                        ur = ur.Include(ur => ur.Role));

                if (userRole == null)
                    throw new BadRequestException("UserRole not found for user in this project");

                // Step 5: Create the MemberTask with the correct MemberId
                var entity = request.Adapt<MemberTask>();
                entity.Id = Guid.NewGuid();
                entity.JoinedAt = DateTime.Now;
                entity.Status = Status.Created.ToString().ToLowerInvariant();
                entity.MemberId = userRole.Id;

                await _unitOfWork.GetMemberTaskRepository().AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();

                return entity.Adapt<RS_MemberTask>();
            }
            catch (Exception e)
            {
                throw new BadRequestException("Failed to create MemberTask");
            }
        }

    public async Task<RS_MemberTask?> UpdateAsync(Guid id, RQ_MemberTask request)
    {
        var repo = _unitOfWork.GetMemberTaskRepository();
        var entity = await repo.GetByIdAsync<Guid>(id);
        if (entity == null) return null;
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