using SRPM_Repositories.Models;

namespace SRPM_Repositories.Repositories.Interfaces;

public interface IDocumentRepository : IGenericRepository<Document>
{
    Task<Document?> GetFullDetailDocument(Guid id);
    Task<(List<Document>? listDocument, int totalFound)> ListPaging
        (string? keyWord, string? docType, bool? isTemplate, string? status,
        DateTime? fromDate, DateTime? toDate,
        Guid? uploaderId, Guid? editorId, Guid? projectId, Guid? evaluationId, Guid? individualEvaluationId, Guid? transactionId,
        byte sortBy, int pageIndex, int pageSize);
}
