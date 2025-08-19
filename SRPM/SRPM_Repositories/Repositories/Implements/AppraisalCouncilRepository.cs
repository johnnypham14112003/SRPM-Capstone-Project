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
    public async Task<(List<Project>? srcProject, List<Project>? proposals, string? error)> GetProjectOfCouncil(Guid councilId)
    {
        var council = await _context.AppraisalCouncil
            .Include(c => c.Evaluations)
                .ThenInclude(e => e.Project)
                .ThenInclude(pt => pt.ProjectTags)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == councilId);
        if (council is null) return (null, null, "Not found any council with that councilId");

        //get proposal from evaluation
        var proposals = council.Evaluations
            .Where(e => e.Project != null && e.Project.Genre.ToLower().Equals("proposal"))
            .GroupBy(e => e.Project.Id)
            .Select(g =>
            {
                var project = g.First().Project!;
                project.Evaluations = g.ToList(); // Gán evaluations vào project
                return project;
            })
            .ToList();

        // Get list code of proposal
        var proposalCodes = proposals
            .Select(p => p.Code)
            .Distinct()
            .ToList();

        var projectSources = await _context.Project
        .Where(p => proposalCodes.Contains(p.Code) && !p.Genre.ToLower().Equals("proposal"))
        .Include(p => p.ProjectTags)
        .Distinct()
        .ToListAsync();

        return (projectSources, proposals, null);
    }

    public async Task<(AppraisalCouncil? council, string? error)> GetCouncilBelongToProject(Guid projectId)
    {
        var project = await _context.Project.FirstOrDefaultAsync(p => p.Id == projectId);
        if (project is null) return (null, "Not found this project Id");

        var proposal = await _context.Project
            .Where(p =>
            p.Code.Equals(project.Code) &&
            p.Genre.ToLower().Equals("proposal") &&
            (p.Status.ToLower().Equals("approved") || p.Status.ToLower().Equals("submitted") || p.Status.ToLower().Equals("inprogress")))
            .FirstOrDefaultAsync();
        if (proposal is null) return (null, "Not found any proposal of this project");

        var council = await _context.Evaluation
        .Where(e => e.ProjectId == proposal.Id && e.AppraisalCouncilId != null)
        .Select(e => e.AppraisalCouncil)
        .FirstOrDefaultAsync();

        if (council is null) return (null, "No council found for this project");
        return (council, null);
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
            query = query.Where(ac =>
                ac.Code.ToLower().Contains(keyWord.ToLower()) ||
                ac.Name.ToLower().Contains(keyWord.ToLower()));
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