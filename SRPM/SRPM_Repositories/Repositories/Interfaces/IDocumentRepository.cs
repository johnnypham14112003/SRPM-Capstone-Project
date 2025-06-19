using SRPM_Repositories.Models;

namespace SRPM_Repositories.Repositories.Interfaces;

public interface IDocumentRepository : IGenericRepository<Document>
{
    Task<Document?> GetFullDetailDocument(Guid id);
    Task<(List<Document>? listDocument, int totalFound)> ListPaging
        (string? keyWord, string? docType, string? status,
        DateTime? fromDate, DateTime? toDate, bool searchByCreateDate,
        byte SortBy, int pageIndex, int pageSize);
}
