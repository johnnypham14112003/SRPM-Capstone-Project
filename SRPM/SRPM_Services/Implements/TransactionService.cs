using Mapster;
using Microsoft.EntityFrameworkCore;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Services.BusinessModels;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.RequestModels.Query;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Extensions.Exceptions;
using SRPM_Services.Interfaces;

namespace SRPM_Services.Implements;

public class TransactionService : ITransactionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;
    public TransactionService(IUnitOfWork unitOfWork, IUserContextService userContextService)
    {
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    //===================================================================================
    public async Task<(bool result, Guid transactionId)> NewTransaction(RQ_Transaction trans)
    {
        //Check Null Data
        bool hasInvalidFields = new[] { trans.Title, trans.Type,
            trans.ReceiverAccount, trans.ReceiverBankName, trans.ReceiverName }
        .Any(string.IsNullOrWhiteSpace);
        if (hasInvalidFields)
            throw new BadRequestException("Transaction Title, Type, ReceiverAccount, ReceiverBankName, ReceiverName cannot be empty!");
        
        //default get by current session || use id on parameter
        Guid userRoleId = trans.RequestPersonId is null ? userRoleId = await GetCurrentMainUserRoleId() : Guid.Empty;
        if (userRoleId == Guid.Empty)
            throw new BadRequestException("Unknown Who Is Creating This Transaction!");

        var datePart = DateTime.Now.ToString("ddMMyyyy");
        trans.Code = $"T{datePart}";
        trans.RequestPersonId = userRoleId;

        var transactionDTO = trans.Adapt<Transaction>();
        await _unitOfWork.GetTransactionRepository().AddAsync(transactionDTO);
        var result = await _unitOfWork.GetTransactionRepository().SaveChangeAsync();
        return (result, transactionDTO.Id);
    }

    public async Task<RS_Transaction?> GetTransactionById(Guid id)
    {
        var existTransaction = await _unitOfWork.GetTransactionRepository().GetOneAsync(
            t => t.Id == id,
            t => t.Include(ti => ti.Documents), false);
        return existTransaction.Adapt<RS_Transaction>();
    }

    public async Task<PagingResult<RS_Transaction>?> ListTransaction(Q_Transaction queryInput)
    {
        //Re-assign value if it smaller than 1
        queryInput.PageIndex = queryInput.PageIndex < 1 ? 1 : queryInput.PageIndex;
        queryInput.PageSize = queryInput.PageSize < 1 ? 1 : queryInput.PageSize;

        var dataResult = await _unitOfWork.GetTransactionRepository().ListPaging
            (queryInput.KeyWord, queryInput.FromDate, queryInput.ToDate,
            queryInput.SortBy, queryInput.Status, queryInput.PageIndex, queryInput.PageSize,
            queryInput.ProjectId, queryInput.EvaluationStageId, queryInput.RequestPersonId, queryInput.HandlePersonId);

        // Checking Result
        if (dataResult.listTransaction is null || dataResult.listTransaction.Count == 0)
            throw new NotFoundException("Not Found Any Council!");

        return new PagingResult<RS_Transaction>
        {
            PageIndex = queryInput.PageIndex,
            PageSize = queryInput.PageSize,
            TotalCount = dataResult.totalCount,
            DataList = dataResult.listTransaction.Adapt<List<RS_Transaction>>()
        };
    }

    public async Task<bool> UpdateTransaction(RQ_Transaction inputData)
    {
        var transaction = await _unitOfWork.GetTransactionRepository().GetByIdAsync(inputData.Id)
            ?? throw new NotFoundException("Not Found This Transaction Object To Update!");

        inputData.Code = transaction.Code;
        inputData.RequestPersonId = transaction.RequestPersonId;
        if (!string.IsNullOrWhiteSpace(inputData.SenderAccount))
        {
            //default get by current session || use id on parameter
            Guid userRoleId = inputData.RequestPersonId is null ? userRoleId = await GetCurrentMainUserRoleId() : Guid.Empty;
            if (userRoleId == Guid.Empty)
                throw new BadRequestException("Unknown Who Is Handle This Transaction!");

            inputData.HandlePersonId = userRoleId;
        }

        await _unitOfWork.GetTransactionRepository().UpdateAsync(inputData.Adapt(transaction));
        return await _unitOfWork.GetTransactionRepository().SaveChangeAsync();
    }

    public async Task<bool> DeleteTransaction(Guid id)
    {
        var transaction = await _unitOfWork.GetTransactionRepository().GetByIdAsync(id);
        if (transaction == null) return false;

        await _unitOfWork.GetTransactionRepository().DeleteAsync(transaction);
        return await _unitOfWork.GetTransactionRepository().SaveChangeAsync();
    }

    //=================================================================================
    private async Task<Guid> GetCurrentMainUserRoleId()
    {
        _ = Guid.TryParse(_userContextService.GetCurrentUserId(), out Guid accId);

        var existAccount = await _unitOfWork.GetAccountRepository().GetOneAsync(a => a.Id == accId, null, false) ??
            throw new NotFoundException("Your current session account Id is not exist in database! Can't find your cv");

        var defaultUserRole = await _unitOfWork.GetUserRoleRepository().GetOneAsync(ur =>
            ur.AccountId == accId &&
            ur.ProjectId == null &&
            ur.AppraisalCouncilId == null &&
            ur.ExpireDate.HasValue &&
            ur.ExpireDate > DateTime.Now, null, false) ??
        throw new NotFoundException("Not found your base role Id or it is expired in system!");

        return defaultUserRole.Id;
    }
}