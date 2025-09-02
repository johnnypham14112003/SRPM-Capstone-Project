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
    public async Task<List<Project>?> FilterProposalByCouncil(Guid councilId)
    {
        return await _context.Project
            //Filter projects that have at least one matching council in Evaluations or EvaluationStages
            .Where(p => p.Evaluations
                .Any(ev => ev.AppraisalCouncilId == councilId ||
                    ev.EvaluationStages.Any(es => es.AppraisalCouncilId == councilId)
                )
            )

            //Include only those Evaluations that match
            .Include(p => p.Evaluations
                .Where(ev => ev.AppraisalCouncilId == councilId ||
                             ev.EvaluationStages.Any(est => est.AppraisalCouncilId == councilId)
                )
            )
                //For each included Evaluation, include only its matching EvaluationStages
                .ThenInclude(eva => eva.EvaluationStages
                    .Where(es => es.AppraisalCouncilId == councilId)
                )
            .AsSplitQuery()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<(List<Evaluation>? listEvaluation, int totalFound)> ListPaging
        (string? keyWord, string? status,
        DateTime? fromDate, DateTime? toDate, byte? rating,
        Guid? projectId, Guid? appraisalCouncilId,
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

        //Status Filter
        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(e => e.Status.ToLower().Equals(status.ToLower()));
            if (!status.ToLower().Equals("deleted"))
                query = query.Where(ie => !ie.Status.ToLower().Equals("deleted"));
        }

        // Date Range Filter
        if (fromDate.HasValue)
            query = query.Where(e => e.CreateDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(e => e.CreateDate <= toDate.Value);

        // Rating Filter
        if (rating.HasValue)
            query = query.Where(e => e.TotalRate.HasValue && e.TotalRate.Value == rating.Value);

        //By ProjectId
        if (projectId.HasValue)
            query = query.Where(e => e.ProjectId == projectId.Value);

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
            case 4: // ProjectId
                query = query.OrderBy(e => e.ProjectId);
                break;
            case 5: // AppraisalCouncilId
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