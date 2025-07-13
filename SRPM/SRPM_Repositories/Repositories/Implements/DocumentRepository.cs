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
        (string? keyWord, string? docType, string? status,
        DateTime? fromDate, DateTime? toDate, bool searchByCreateDate,
        byte SortBy, int pageIndex, int pageSize)
    {
        var query = _context.Document
            .Include(d => d.Signatures)
            .AsNoTracking()
            .AsQueryable();

        // ===========================[ Apply Search ]===========================
        //keyword Filter
        if (!string.IsNullOrWhiteSpace(keyWord))
            query = query.Where(d => d.Name.ToLower().Contains(keyWord.ToLower()));

        //Type Filter
        if (!string.IsNullOrWhiteSpace(docType))
            query = query.Where(d => d.Type.ToLower().Equals(docType.ToLower()));

        //Status Filter
        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(d => d.Status.ToLower().Equals(status.ToLower()));

        //Date Filter
        if (searchByCreateDate)
        {
            if (fromDate.HasValue)
                query = query.Where(d => d.UploadAt >= fromDate.Value);
            if (toDate.HasValue)
                query = query.Where(d => d.UploadAt <= toDate.Value);
        }
        else
        {
            if (fromDate.HasValue)
                query = query.Where(d => d.UpdatedAt >= fromDate.Value);
            if (toDate.HasValue)
                query = query.Where(d => d.UpdatedAt <= toDate.Value);
        }

        // Sort By (Newer come first)(A-Z)
        switch (SortBy)
        {
            default://Name
                query = query.OrderBy(d => d.Name);
                break;
            case 1://UpdateTime
                query = query.OrderByDescending(d => d.UpdatedAt);
                break;
            case 2://CreateTime
                query = query.OrderByDescending(d => d.UploadAt);
                break;
            case 3://UploaderId
                query = query.OrderBy(d => d.UploaderId);
                break;
            case 4://ProjectId
                query = query.OrderBy(d => d.ProjectId);
                break;
            case 5://EvaluationId
                query = query.OrderBy(d => d.EvaluationId);
                break;
            case 6://IndividualEvaluationId
                query = query.OrderBy(d => d.IndividualEvaluationId);
                break;
            case 7://TransactionId
                query = query.OrderBy(d => d.TransactionId);
                break;
        }

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