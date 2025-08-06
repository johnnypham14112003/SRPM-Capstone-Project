using Microsoft.EntityFrameworkCore;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;

namespace SRPM_Repositories.Repositories.Implements;

public class AppraisalCouncilRepository : GenericRepository<AppraisalCouncil>, IAppraisalCouncilRepository
{
    private readonly SRPMDbContext _context;
    public AppraisalCouncilRepository(SRPMDbContext context) : base(context)
    {
        _context = context;
    }
    //===================================================================================
    public async Task<AppraisalCouncil?> GetDetailWithInclude(Guid id, byte includeNo)
    {
        switch (includeNo)
        {
            case 1://Evaluations
                return await _context.AppraisalCouncil
                    .Include(ac => ac.Evaluations)
                    .FirstOrDefaultAsync(ac => ac.Id == id);

            case 2://EvaluationStages
                return await _context.AppraisalCouncil
                    .Include(ac => ac.EvaluationStages)
                    .FirstOrDefaultAsync(ac => ac.Id == id);

            case 3://Members
                return await _context.AppraisalCouncil
                    .Include(ac => ac.CouncilMembers)
                    .FirstOrDefaultAsync(ac => ac.Id == id);

            case 12://Evaluations && EvaluationStage
                return await _context.AppraisalCouncil
                    .Include(ac => ac.Evaluations)
                    .Include(ac => ac.EvaluationStages)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync(ac => ac.Id == id);

            case 13://Evaluations && Members
                return await _context.AppraisalCouncil
                    .Include(ac => ac.Evaluations)
                    .Include(ac => ac.CouncilMembers)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync(ac => ac.Id == id);

            case 23://EvaluationStages && Members
                return await _context.AppraisalCouncil
                    .Include(ac => ac.EvaluationStages)
                    .Include(ac => ac.CouncilMembers)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync(ac => ac.Id == id);

            case 123://Include all
                return await _context.AppraisalCouncil
                    .Include(ac => ac.Evaluations)
                    .Include(ac => ac.EvaluationStages)
                    .Include(ac => ac.CouncilMembers)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync(ac => ac.Id == id);

            default: return await _context.AppraisalCouncil.FirstOrDefaultAsync(ac => ac.Id == id);
        }
    }

    public async Task<(List<AppraisalCouncil>? listCouncil, int totalCount)> ListPaging
        (string? keyWord, DateTime? fromDate, DateTime? toDate,
        byte SortBy, string? status, int pageIndex, int pageSize)
    {
        var query = _context.AppraisalCouncil
            .AsNoTracking()
            .AsQueryable();

        // ===========================[ Apply Search ]===========================
        //keyword Filter
        if (!string.IsNullOrWhiteSpace(keyWord))
        {
            query = query.Where(ac => ac.Code.ToLower().Contains(keyWord.ToLower()));
            query = query.Where(ac => ac.Name.ToLower().Contains(keyWord.ToLower()));
        }

        //Status Filter
        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(ac => ac.Status.ToLower().Equals(status.ToLower()));
            if (!status.ToLower().Equals("deleted"))
                query = query.Where(ac => !ac.Status.ToLower().Equals("deleted"));
        }

        //Date Filter
        if (fromDate.HasValue)
            query = query.Where(ac => ac.CreatedAt >= fromDate.Value);
        if (toDate.HasValue)
            query = query.Where(ac => ac.CreatedAt <= toDate.Value);

        // Sort By (Newer come first)(A-Z)
        if (SortBy == 1)
        { query = query.OrderByDescending(ac => ac.Code); }
        else if (SortBy == 2)
        { query = query.OrderByDescending(ac => ac.Name); }
        else
        { query = query.OrderByDescending(ac => ac.CreatedAt); }

        // Sum notification of a user
        int sumCouncil = await query.CountAsync();

        // ===========================[ Apply paging ]===========================
        var pagedList = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (pagedList, sumCouncil);
    }
}