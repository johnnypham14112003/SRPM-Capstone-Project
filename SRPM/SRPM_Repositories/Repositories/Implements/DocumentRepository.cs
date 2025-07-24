using Microsoft.EntityFrameworkCore;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;

namespace SRPM_Repositories.Repositories.Implements;

public class DocumentRepository : GenericRepository<Document>, IDocumentRepository
{
    private readonly SRPMDbContext _context;
    public DocumentRepository(SRPMDbContext context) : base(context)
    {
        _context = context;
    }

    //===================================================================================
    public async Task<Document?> GetFullDetailDocument(Guid id)
    {
        return await _context.Document.AsSplitQuery()
            .Include(d => d.Signatures)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<(List<Document>? listDocument, int totalFound)> ListPaging
        (string? keyWord, string? docType, bool? isTemplate, string? status,
        DateTime? fromDate, DateTime? toDate,
        Guid? uploaderId, Guid? editorId, Guid? projectId, Guid? evaluationId, Guid? individualEvaluationId, Guid? transactionId,
        byte sortBy, int pageIndex, int pageSize)
    {
        var query = _context.Document
            .Include(d => d.Signatures)
            .AsNoTracking()
            .AsQueryable();

        // ===========================[ Apply Filters ]===========================
        //Filter By Name
        if (!string.IsNullOrWhiteSpace(keyWord))
            query = query.Where(d => d.Name.Contains(keyWord, StringComparison.OrdinalIgnoreCase));

        //Filter By Type
        if (!string.IsNullOrWhiteSpace(docType))
            query = query.Where(d => d.Type.Equals(docType, StringComparison.OrdinalIgnoreCase));
        
        //Filter By IsTemplate
        if (isTemplate.HasValue)
            query = query.Where(d => d.IsTemplate == isTemplate);

        //Filter By Status
        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(d => d.Status.Equals(status, StringComparison.OrdinalIgnoreCase));

        //Filter By Time
        if (fromDate.HasValue)
            query = query.Where(d => d.UploadAt >= fromDate.Value);
        if (toDate.HasValue)
            query = query.Where(d => d.UploadAt <= toDate.Value);

        // Filter by IDs
        if (uploaderId.HasValue)
            query = query.Where(d => d.UploaderId == uploaderId.Value);

        if (editorId.HasValue)
            query = query.Where(d => d.EditorId == editorId.Value);

        if (projectId.HasValue)
            query = query.Where(d => d.ProjectId == projectId.Value);

        if (evaluationId.HasValue)
            query = query.Where(d => d.EvaluationId == evaluationId.Value);

        if (individualEvaluationId.HasValue)
            query = query.Where(d => d.IndividualEvaluationId == individualEvaluationId.Value);

        if (transactionId.HasValue)
            query = query.Where(d => d.TransactionId == transactionId.Value);

        // ===========================[ Apply Sorting ]===========================

        query = sortBy switch
        {
            // UpdatedAt 
            1 => query.OrderByDescending(d => d.UpdatedAt),
            2 => query.OrderBy(d => d.UpdatedAt),

            // UploadAt
            3 => query.OrderByDescending(d => d.UploadAt),
            4 => query.OrderByDescending(d => d.UpdatedAt),

            // UploaderId
            5 => query.OrderBy(d => d.UploaderId),

            // ProjectId
            6 => query.OrderBy(d => d.ProjectId),

            // EvaluationId
            7 => query.OrderBy(d => d.EvaluationId),

            // IndividualEvaluationId
            8 => query.OrderBy(d => d.IndividualEvaluationId),

            // TransactionId
            9 => query.OrderBy(d => d.TransactionId),

            // EditorId
            10 => query.OrderBy(d => d.EditorId),

            // Name
            11 => query.OrderByDescending(d => d.Name),
            _ => query.OrderBy(d => d.Name),
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