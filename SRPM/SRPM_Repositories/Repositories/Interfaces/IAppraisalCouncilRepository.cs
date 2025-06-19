using SRPM_Repositories.Models;

namespace SRPM_Repositories.Repositories.Interfaces;

public interface IAppraisalCouncilRepository : IGenericRepository<AppraisalCouncil>
{
    Task<AppraisalCouncil?> GetDetailWithInclude(Guid id, byte includeNo);
    Task<(List<AppraisalCouncil>? listCouncil, int totalCount)> ListPaging
        (string? keyWord, DateTime? fromDate, DateTime? toDate,
        byte SortBy, string? status, int pageIndex, int pageSize);
}