using Microsoft.EntityFrameworkCore;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;

namespace SRPM_Repositories.Repositories.Implements;

public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
{
    private readonly SRPMDbContext _context;
    public TransactionRepository(SRPMDbContext context) : base(context)
    {
        _context = context;
    }
    //===================================================================================
    public async Task<(List<Transaction>? listTransaction, int totalCount)> ListPaging(
        string? keyWord, DateTime? fromDate, DateTime? toDate,
        byte sortBy, string? status, int pageIndex, int pageSize,
        Guid? projectId, Guid? evaluationStageId,
        Guid? requestPersonId, Guid? handlePersonId)
    {
        var query = _context.Transaction
            .AsNoTracking()
            .AsQueryable();

        // ===========================[ Apply Search ]===========================
        if (!string.IsNullOrWhiteSpace(keyWord))
        {
            keyWord = keyWord.ToLower();
            query = query.Where(t =>
                t.Code.ToLower().Contains(keyWord) ||
                t.Title.ToLower().Contains(keyWord) ||
                t.Type.ToLower().Contains(keyWord) ||
                (t.SenderName != null && t.SenderName.ToLower().Contains(keyWord)) ||
                (t.ReceiverName != null && t.ReceiverName.ToLower().Contains(keyWord)) ||
                (t.TransferContent != null && t.TransferContent.ToLower().Contains(keyWord))
            );
        }

        // ===========================[ Status Filter ]===========================
        if (!string.IsNullOrWhiteSpace(status))
        {
            status = status.ToLower();
            query = query.Where(t => t.Status.ToLower() == status);
            if (status != "deleted")
                query = query.Where(t => t.Status.ToLower() != "deleted");
        }

        // ===========================[ Date Filter ]===========================
        if (fromDate.HasValue)
            query = query.Where(t => t.RequestDate >= fromDate.Value);
        if (toDate.HasValue)
            query = query.Where(t => t.RequestDate <= toDate.Value);

        // ===========================[ Foreign Key Filters ]===========================
        if (projectId.HasValue)
            query = query.Where(t => t.ProjectId == projectId.Value);
        if (evaluationStageId.HasValue)
            query = query.Where(t => t.EvaluationStageId == evaluationStageId.Value);
        if (requestPersonId.HasValue)
            query = query.Where(t => t.RequestPersonId == requestPersonId.Value);
        if (handlePersonId.HasValue)
            query = query.Where(t => t.HandlePersonId == handlePersonId.Value);

        // ===========================[ Sorting ]===========================
        switch (sortBy)
        {
            case 1:
                query = query.OrderByDescending(t => t.Code);
                break;
            case 2:
                query = query.OrderByDescending(t => t.Title);
                break;
            case 3:
                query = query.OrderByDescending(t => t.Type);
                break;
            case 4:
                query = query.OrderByDescending(t => t.ProjectId);
                break;
            default:
                query = query.OrderByDescending(t => t.RequestDate);
                break;
        }

        // ===========================[ Paging ]===========================
        int totalCount = await query.CountAsync();

        var pagedList = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (pagedList, totalCount);
    }

}