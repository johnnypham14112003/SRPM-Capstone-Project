using Microsoft.EntityFrameworkCore;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;

namespace SRPM_Repositories.Repositories.Implements;

public class IndividualEvaluationRepository : GenericRepository<IndividualEvaluation>, IIndividualEvaluationRepository
{
    private readonly SRPMDbContext _context;
    public IndividualEvaluationRepository(SRPMDbContext context) : base(context)
    {
        _context = context;
    }

    //=========================================================================
    public async Task<(List<IndividualEvaluation>? listIndividualEvaluations, int totalFound)> GetListPagingAsync(
        string? keyword, byte? totalRate, DateTime? fromDate, DateTime? toDate,
        bool? isApproved, bool? reviewerResult, bool? isAIReport,
        string? status, Guid? evaluationStageId, Guid? reviewerId,
        byte sortBy, int pageIndex, int pageSize)
    {
        var query = _context.IndividualEvaluation
            .Include(ie => ie.Reviewer).ThenInclude(r => r.Account)
            .Include(ie => ie.EvaluationStage)
            .Include(ie => ie.Documents) // if applicable
            .AsNoTracking()
            .AsQueryable();

        // ===========================[ Apply Search ]===========================
        // Keyword Filter
        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(ie => ie.Name.ToLower().Contains(keyword.ToLower()));

        // TotalRate Filter
        if (totalRate.HasValue)
            query = query.Where(ie => ie.TotalRate == totalRate.Value);

        // IsApproved Filter
        if (isApproved.HasValue)
            query = query.Where(ie => ie.IsApproved == isApproved);

        // ReviewerResult Filter
        if (reviewerResult.HasValue)
            query = query.Where(ie => ie.ReviewerResult == reviewerResult.Value);

        // IsAIReport Filter
        if (isAIReport.HasValue)
            query = query.Where(ie => ie.IsAIReport == isAIReport);

        // Status Filter
        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(ie => ie.Status.ToLower().Equals(status.ToLower()));
            if (!status.ToLower().Equals("deleted"))
                query = query.Where(ie => !ie.Status.ToLower().Equals("deleted"));
        }

        // EvaluationStageId Filter
        if (evaluationStageId.HasValue)
            query = query.Where(ie => ie.EvaluationStageId == evaluationStageId);

        // ReviewerId Filter
        if (reviewerId.HasValue)
            query = query.Where(ie => ie.ReviewerId == reviewerId);

        //Filter By Time
        if (fromDate.HasValue)
            query = query.Where(ie => ie.SubmittedAt >= fromDate.Value);
        if (toDate.HasValue)
            query = query.Where(ie => ie.SubmittedAt <= toDate.Value);

        // ===========================[ Apply Sorting ]===========================
        query = sortBy switch
        {
            // TotalRate
            1 => query.OrderByDescending(ie => ie.TotalRate),
            // Time
            2 => query.OrderByDescending(ie => ie.SubmittedAt),
            // Status
            3 => query.OrderByDescending(ie => ie.Status),
            // Name
            _ => query.OrderBy(ie => ie.Name),
        };

        // ===========================[ Apply Paging ]===========================
        int totalFound = await query.CountAsync();

        var pagedList = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (pagedList, totalFound);
    }
}