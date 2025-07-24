using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
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
    private readonly IUserContextService _userContextService;
    public DocumentService(IUnitOfWork unitOfWork, IUserContextService userContextService)
    {
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    //=============================================================================
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
            (queryInput.KeyWord, queryInput.Type, queryInput.IsTemplate, queryInput.Status,
            queryInput.FromDate, queryInput.ToDate,
            queryInput.UploaderId, queryInput.EditorId, queryInput.ProjectId,
            queryInput.EvaluationId, queryInput.IndividualEvaluationId, queryInput.TransactionId,
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

    public async Task<RS_Document?> ViewProfileCV(string? email)
    {//default get by current session
        UserRole? defaultUserRole = null;
        Guid accId = Guid.Empty;
        Guid.TryParse(_userContextService.GetCurrentUserId(), out accId);

        if (accId == Guid.Empty && string.IsNullOrWhiteSpace(email))
            throw new BadRequestException("Unknown User Parameter To Find Document!");

        if (string.IsNullOrWhiteSpace(email))
        {
            var existAccount = await _unitOfWork.GetAccountRepository().GetOneAsync(a => a.Id == accId, null, false) ??
            throw new NotFoundException("Your current session account Id is not exist in database! Can't find your cv");

            defaultUserRole = await _unitOfWork.GetUserRoleRepository().GetOneAsync(ur =>
                ur.AccountId == accId &&
                ur.ProjectId == null &&
                ur.AppraisalCouncilId == null &&
                ur.ExpireDate.HasValue &&
                ur.ExpireDate > DateTime.Now, null, false) ??
            throw new NotFoundException("Not found your base role Id or it is expired in system!");
        }
        else
        {//search by email
            var existAccount = await _unitOfWork.GetAccountRepository()
                .GetOneAsync(a => a.Email.Equals(email, StringComparison.OrdinalIgnoreCase), null, false) ??
                    throw new NotFoundException("Not found this user account Id base on email!");

            defaultUserRole = await _unitOfWork.GetUserRoleRepository().GetOneAsync(ur =>
                ur.AccountId == existAccount.Id &&
                ur.ProjectId == null &&
                ur.AppraisalCouncilId == null &&
                ur.ExpireDate.HasValue &&
                ur.ExpireDate > DateTime.Now, null, false) ??
            throw new NotFoundException("Not found this email user base role Id or it is expired in system!");
        }

        var existDocument = await _unitOfWork.GetDocumentRepository().GetOneAsync(d =>
            (d.UploaderId == defaultUserRole!.Id || d.EditorId == defaultUserRole!.Id) &&
            d.Type.Equals("ScienceCV", StringComparison.OrdinalIgnoreCase) &&
            d.IsTemplate == false &&
            !d.Status.Equals("deleted", StringComparison.OrdinalIgnoreCase), null, false)
            ?? throw new NotFoundException("Not found the Cv Document of that UserRole!");

        return existDocument.Adapt<RS_Document>();
    }

    public async Task<RS_Document?> ViewDetailDocument(Guid id)
    {
        if (id == Guid.Empty) throw new BadRequestException("Cannot view a null Document Id!");
        var existDocument = await _unitOfWork.GetDocumentRepository().GetFullDetailDocument(id)
            ?? throw new NotFoundException("Not found this Document Id!");

        return existDocument.Adapt<RS_Document>();
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