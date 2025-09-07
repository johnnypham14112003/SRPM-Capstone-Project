using SRPM_Repositories.Models;

namespace SRPM_Repositories.Repositories.Interfaces;

public interface IAppraisalCouncilRepository : IGenericRepository<AppraisalCouncil>
{
    Task<AppraisalCouncil?> GetDetailWithInclude(Guid id, byte includeNo);

    Task<(List<Project>? srcProject, List<Project>? proposals, string? error)> GetProjectOfCouncil(Guid councilId);
    Task<(AppraisalCouncil? council, string? error)> GetCouncilBelongToProject(Guid projectId, int? stageOrder = null);

    Task<(List<AppraisalCouncil>? listCouncil, int totalCount)> ListPaging
        (string? keyWord, DateTime? fromDate, DateTime? toDate,
        byte SortBy, string? status, int pageIndex, int pageSize);
}