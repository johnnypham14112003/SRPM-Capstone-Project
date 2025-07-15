using Microsoft.EntityFrameworkCore;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;

namespace SRPM_Repositories.Repositories.Implements;

public class EvaluationRepository : GenericRepository<Evaluation>, IEvaluationRepository
{
    private readonly SRPMDbContext _context;
    public EvaluationRepository(SRPMDbContext context) : base(context)
    {
        _context = context;
    }
    //===================================================================================
    public async Task<Evaluation?> GetDetailWithInclude(Guid id, byte includeNo)
    {
        switch (includeNo)
        {
            case 1://EvaluationStages
                return await _context.Evaluation
                    .Include(e => e.EvaluationStages)
                    .FirstOrDefaultAsync(e => e.Id == id);

            case 2://Documents
                return await _context.Evaluation
                    .Include(e => e.Documents)
                    .FirstOrDefaultAsync(e => e.Id == id);

            default://Include All
                return await _context.Evaluation
                    .Include(e => e.Documents)
                    .Include(e => e.EvaluationStages)
                    .AsSplitQuery()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id == id);
        }
    }

    public async Task<(List<Evaluation>? listEvaluation, int totalFound)> ListPaging
        (string? keyWord, string? evaPhrase, string? evaType, string? status,
        DateTime? fromDate, DateTime? toDate, byte? rating,
        Guid? projectId,Guid? milestoneId, Guid? appraisalCouncilId,
        byte sortBy, int pageIndex, int pageSize)
    {
        var query = _context.Evaluation
            .Include(e => e.EvaluationStages)
            .AsNoTracking()
            .AsQueryable();

        // ===========================[ Apply Search ]===========================
        //keyword Filter
        if (!string.IsNullOrWhiteSpace(keyWord))
            query = query.Where(e =>
            e.Code.ToLower().Contains(keyWord.ToLower()) ||
            e.Title.ToLower().Contains(keyWord.ToLower()));

        //Phrase Filter
        if (!string.IsNullOrWhiteSpace(evaPhrase))
            query = query.Where(e => e.Phrase.Equals(evaPhrase, StringComparison.OrdinalIgnoreCase));

        //Type Filter
        if (!string.IsNullOrWhiteSpace(evaType))
            query = query.Where(e => e.Type.Equals(evaType, StringComparison.OrdinalIgnoreCase));

        //Status Filter
        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(e => e.Status.Equals(status, StringComparison.OrdinalIgnoreCase));

        // Date Range Filter
        if (fromDate.HasValue)
        {
            query = query.Where(e => e.CreateDate >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(e => e.CreateDate <= toDate.Value);
        }

        // Rating Filter
        if (rating.HasValue)
            query = query.Where(e => e.TotalRate.HasValue && e.TotalRate.Value == rating.Value);

        //By ProjectId
        if (projectId.HasValue)
            query = query.Where(e => e.ProjectId == projectId.Value);

        //By milestoneId
        if (milestoneId.HasValue)
            query = query.Where(es => es.MilestoneId == milestoneId.Value);

        //By appraisalId
        if (appraisalCouncilId.HasValue)
            query = query.Where(es => es.AppraisalCouncilId == appraisalCouncilId.Value);

        // Sort By (Newer come first)(A-Z)
        switch (sortBy)
        {
            default://Name
                query = query.OrderBy(e => e.Title);
                break;
            case 2: // Rating
                query = query.OrderByDescending(e => e.TotalRate);
                break;
            case 3: // CreateTime
                query = query.OrderByDescending(e => e.CreateDate);
                break;
            case 4: // Phrase
                query = query.OrderBy(e => e.Phrase);
                break;
            case 5: // Type
                query = query.OrderBy(e => e.Type);
                break;
            case 6: // ProjectId
                query = query.OrderBy(e => e.ProjectId);
                break;
            case 7: // MilestoneId
                query = query.OrderBy(e => e.MilestoneId);
                break;
            case 8: // AppraisalCouncilId
                query = query.OrderBy(e => e.AppraisalCouncilId);
                break;
        }

        // Sum notification of a user
        int sumEvaluation = await query.CountAsync();

        // ===========================[ Apply paging ]===========================
        var pagedList = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (pagedList, sumEvaluation);
    }
}