using SRPM_Services.BusinessModels.RequestModels;

namespace SRPM_Services.Interfaces;

public interface IDocumentSectionService
{
    public Task<(bool result, Guid DocumentId)> AddSection(RQ_DocumentSection docSec);
    public Task<bool> UpdateDocumentSection(RQ_DocumentSection newDocumentSection);
    public Task<bool> DeleteDocumentSection(Guid id);
}