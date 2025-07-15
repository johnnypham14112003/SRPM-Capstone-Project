using Microsoft.EntityFrameworkCore;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;

namespace SRPM_Repositories.Repositories.Implements;

public class EvaluationStageRepository : GenericRepository<EvaluationStage>, IEvaluationStageRepository
{
    private readonly SRPMDbContext _context;
    public EvaluationStageRepository(SRPMDbContext context) : base(context)
    {
        _context = context;
    }
    //===================================================================================
    public async Task<EvaluationStage?> GetStageDetailWithInclude(Guid id, byte includeNo)
    {
        switch (includeNo)
        {
            case 1: // Transactions
                return await _context.EvaluationStage
                    .Include(es => es.Transactions)
                    .FirstOrDefaultAsync(es => es.Id == id);

            case 2: // IndividualEvaluations
                return await _context.EvaluationStage
                    .Include(es => es.IndividualEvaluations)
                    .FirstOrDefaultAsync(es => es.Id == id);

            case 3: // Notifications
                return await _context.EvaluationStage
                    .Include(es => es.Notifications)
                    .FirstOrDefaultAsync(es => es.Id == id);

            default: // Include All
                return await _context.EvaluationStage
                    .Include(es => es.Transactions)
                    .Include(es => es.IndividualEvaluations)
                    .Include(es => es.Notifications)
                    .AsSplitQuery()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(es => es.Id == id);
        }
    }

    public async Task<(List<EvaluationStage>? listStage, int totalFound)> ListStagePaging(
    string? keyWord, string? status,
    Guid? evaluationId, Guid? appraisalCouncilId,
    byte sortBy, int pageIndex, int pageSize)
    {
        var query = _context.EvaluationStage
            .Include(es => es.IndividualEvaluations)
            .AsNoTracking()
            .AsQueryable();

        // ===========================[ Apply Search ]===========================
        // Keyword Filter (Name)
        if (!string.IsNullOrWhiteSpace(keyWord))
            query = query.Where(es => es.Name!.ToLower().Contains(keyWord.ToLower()));

        // Status Filter
        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(es => es.Status.Equals(status, StringComparison.OrdinalIgnoreCase));

        // EvaluationId Filter
        if (evaluationId.HasValue)
            query = query.Where(es => es.EvaluationId == evaluationId.Value);

        // AppraisalCouncilId Filter
        if (appraisalCouncilId.HasValue)
            query = query.Where(es => es.AppraisalCouncilId == appraisalCouncilId.Value);

        // ===========================[ Apply Sorting ]===========================
        switch (sortBy)
        {
            default: // Name
                query = query.OrderBy(es => es.Name);
                break;
            case 2: // StageOrder
                query = query.OrderBy(es => es.StageOrder);
                break;
            case 3: // Status
                query = query.OrderBy(es => es.Status);
                break;
            case 4: // EvaluationId
                query = query.OrderBy(es => es.EvaluationId);
                break;
            case 5: // AppraisalCouncilId
                query = query.OrderBy(es => es.AppraisalCouncilId);
                break;
        }

        // ===========================[ Apply Paging ]===========================
        int totalFound = await query.CountAsync();

        var pagedList = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (pagedList, totalFound);
    }

}