using SRPM_Services.BusinessModels;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.RequestModels.Query;
using SRPM_Services.BusinessModels.ResponseModels;

namespace SRPM_Services.Interfaces;

public interface IDocumentService
{
    Task<(bool result, Guid DocumentId)> NewDocument(RQ_Document doc);
    Task<PagingResult<RS_Document>?> ListDocument(Q_Document queryInput);
    Task<RS_Document> ViewDetailDocument(Guid id);
    Task<bool> UpdateDocumentInfo(RQ_Document newDocument);
    Task<bool> DeleteDocument(Guid id);
}