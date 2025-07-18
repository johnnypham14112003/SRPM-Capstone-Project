using Mapster;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Services.BusinessModels;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.RequestModels.Query;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Extensions.Exceptions;
using SRPM_Services.Interfaces;

namespace SRPM_Services.Implements;

public class DocumentService : IDocumentService
{
    private readonly IUnitOfWork _unitOfWork;
    public DocumentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    //=============================================================================
    public async Task<Dictionary<string, double>?> CheckPlagiarism(string inputText, IEnumerable<string> inputSource)
    {
        return null;
    }

    public async Task<(bool success, Guid DocumentId)> NewDocument(RQ_Document doc)
    {
        //Check Null Data
        bool hasInvalidFields = new[] { doc.Name, doc.Type, doc.ContentHtml }
        .Any(string.IsNullOrWhiteSpace);
        if (hasInvalidFields) throw new BadRequestException("Document Name or Type or Content cannot be empty!");

        Document documentDTO = doc.Adapt<Document>();
        await _unitOfWork.GetDocumentRepository().AddAsync(documentDTO);
        var resultSave = await _unitOfWork.GetDocumentRepository().SaveChangeAsync();
        return (resultSave, documentDTO.Id);
    }

    public async Task<PagingResult<RS_Document>?> ListDocument(Q_Document queryInput)
    {
        //Re-assign value if it smaller than 1
        queryInput.PageIndex = queryInput.PageIndex < 1 ? 1 : queryInput.PageIndex;
        queryInput.PageSize = queryInput.PageSize < 1 ? 1 : queryInput.PageSize;

        var dataResult = await _unitOfWork.GetDocumentRepository().ListPaging
            (queryInput.KeyWord, queryInput.Type, queryInput.Status,
            queryInput.FromDate, queryInput.ToDate, queryInput.SearchByUploadDate,
            queryInput.SortBy, queryInput.PageIndex, queryInput.PageSize);

        // Checking Result
        if (dataResult.listDocument is null || dataResult.listDocument.Count == 0)
            throw new NotFoundException("Not Found Any Document!");

        return new PagingResult<RS_Document>
        {
            PageIndex = queryInput.PageIndex,
            PageSize = queryInput.PageSize,
            TotalCount = dataResult.totalFound,
            DataList = dataResult.listDocument.Adapt<List<RS_Document>>()
        };
    }

    public async Task<RS_Document?> ViewDetailDocument(Guid id)
    {
        if (id == Guid.Empty) throw new BadRequestException("Cannot view a null Document Id!");
        var existCouncil = await _unitOfWork.GetDocumentRepository().GetFullDetailDocument(id)
            ?? throw new NotFoundException("Not found this Document Id!");

        return existCouncil.Adapt<RS_Document>();
    }

    public async Task<bool> UpdateDocumentInfo(RQ_Document newDocument)
    {
        var existDocument = await _unitOfWork.GetDocumentRepository().GetOneAsync(d => d.Id == newDocument.Id)
            ?? throw new NotFoundException("Not found any Document match this Id!");

        //Transfer new Data to old Data
        newDocument.Adapt(existDocument);
        return await _unitOfWork.GetDocumentRepository().SaveChangeAsync();
    }

    public async Task<bool> DeleteDocument(Guid id)
    {
        var existDocument = await _unitOfWork.GetDocumentRepository().GetOneAsync(d => d.Id == id)
            ?? throw new NotFoundException("Not found any Document match this Id!");

        //Remove reference Key
        //...
        //await _unitOfWork.GetDocumentCouncilRepository().DeleteAsync(existCouncil);

        existDocument.Status = "deleted";
        return await _unitOfWork.GetDocumentRepository().SaveChangeAsync();
    }
}