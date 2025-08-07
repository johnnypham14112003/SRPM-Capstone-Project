using SRPM_Repositories.Models;

namespace SRPM_Repositories.Repositories.Interfaces;

public interface ITransactionRepository : IGenericRepository<Transaction>
{
    Task<(List<Transaction>? listTransaction, int totalCount)> ListPaging(
        string? keyWord, DateTime? fromDate, DateTime? toDate,
        byte sortBy, string? status, int pageIndex, int pageSize,
        Guid? projectId, Guid? evaluationStageId,
        Guid? requestPersonId, Guid? handlePersonId);
}
