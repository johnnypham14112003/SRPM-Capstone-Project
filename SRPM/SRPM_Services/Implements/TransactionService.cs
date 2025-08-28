using Mapster;
using Microsoft.AspNetCore.Components.Forms;
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
        //Get origin Cost for request Cost check
        decimal originCost = 0m;
        if (trans.EvaluationStageId.HasValue)
        {
            var existEvaluationStage = await _unitOfWork.GetEvaluationStageRepository().GetOneAsync(
                es => es.Id == trans.EvaluationStageId,
                es => es.Include(stage => stage.Milestone), false)
            ?? throw new NotFoundException($"Not found this EvaluationStage Id! :'{trans.EvaluationStageId}'");

            if (existEvaluationStage.Milestone is not null)
                originCost = existEvaluationStage.Milestone.Cost;
        }
        else if (trans.ProjectId.HasValue)
        {
            var existProject = await _unitOfWork.GetProjectRepository().GetOneAsync(
                es => es.Id == trans.ProjectId, null, false)
            ?? throw new NotFoundException($"Not found this project Id! :'{trans.ProjectId}'");

            originCost = existProject.Budget;
        }

        if (trans.TotalMoney != originCost) throw new BadRequestException($"The request money is not equal to origin declared! {originCost}");

        //Check Null Data
        bool hasInvalidFields = new[] { trans.Title, trans.Type,
            trans.ReceiverAccount, trans.ReceiverBankName, trans.ReceiverName }
        .Any(string.IsNullOrWhiteSpace);
        if (hasInvalidFields)
            throw new BadRequestException("Transaction Title, Type, ReceiverAccount, ReceiverBankName, ReceiverName cannot be empty!");

        //Money range in system
        var moneyRange = await _unitOfWork.GetSystemConfigurationRepository().GetListAsync(
            sys => sys.ConfigType.ToLower().Equals("finance"), null, false);
        if (moneyRange is null || moneyRange.Count > 2)
            throw new BadRequestException("Not found any condition of transaction range or it has more than 2 range point");

        var minimumConfig = moneyRange.FirstOrDefault(t => t.ConfigKey.ToLower().Equals("fund request from"));
        decimal.TryParse(minimumConfig.ConfigValue, out decimal minimumValue);

        var maxConfig = moneyRange.FirstOrDefault(t => t.ConfigKey.ToLower().Equals("fund request to"));
        decimal.TryParse(maxConfig.ConfigValue, out decimal maxValue);


        if (trans.TotalMoney < minimumValue || trans.TotalMoney > maxValue)
            throw new BadRequestException("The total money requested is not in range allowed of system!");

        Guid userRoleId = Guid.Empty;
        //default get by current session || use id on parameter
        if (trans.RequestPersonId is null)
        {
            //Get All Available Role Of This Account
            var userRoles = await GetCurrentUserRoles();
            foreach (var ur in userRoles)
            {
                //Only PI can create transaction
                var currentRole = await _unitOfWork.GetRoleRepository().GetOneAsync(r => r.Id == ur.RoleId);
                if (currentRole.Name.ToLower().Equals("principal investigator"))
                    userRoleId = ur.Id;
            }
        }
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

        //If staff is updating
        if (!string.IsNullOrWhiteSpace(inputData.SenderAccount) && transaction.HandlePersonId is null)
        {
            Guid staffURid = Guid.Empty;
            var userRoles = await GetCurrentUserRoles();
            foreach (var ur in userRoles)
            {
                var currentRole = await _unitOfWork.GetRoleRepository().GetOneAsync(r => r.Id == ur.RoleId);
                if (currentRole.Name.ToLower().Equals("staff"))
                    staffURid = ur.Id;
            }

            if (staffURid == Guid.Empty)
                throw new BadRequestException("Unknown Who Is Handle This Transaction!");

            inputData.HandlePersonId = staffURid;
        }

        await _unitOfWork.GetTransactionRepository().UpdateAsync(inputData.Adapt(transaction));
        return await _unitOfWork.GetTransactionRepository().SaveChangeAsync();
    }

    public async Task<bool> DeleteTransaction(Guid id)
    {
        var transaction = await _unitOfWork.GetTransactionRepository().GetByIdAsync(id);
        if (transaction is null) return false;

        var relateDocs = await _unitOfWork.GetDocumentRepository().GetListAsync(d => d.TransactionId == id);
        if (relateDocs is not null) await _unitOfWork.GetDocumentRepository().DeleteRangeAsync(relateDocs);
        var relateNoti = await _unitOfWork.GetNotificationRepository().GetListAsync(n => n.TransactionId == id);
        if (relateNoti is not null) await _unitOfWork.GetNotificationRepository().DeleteRangeAsync(relateNoti);

        await _unitOfWork.GetTransactionRepository().DeleteAsync(transaction);
        return await _unitOfWork.GetTransactionRepository().SaveChangeAsync();
    }

    public async Task<bool> UpdateStatusTransaction(Guid transactionId, string status)
    {
        //Query exist transaction
        var transaction = await _unitOfWork.GetTransactionRepository().GetByIdAsync(transactionId)
            ?? throw new NotFoundException("Not Found This Transaction id To Update!");

        //Validate status
        if (string.IsNullOrWhiteSpace(status))
            throw new BadRequestException("Cannot update null status!");

        //Assign new status
        transaction.Status = status;

        //If staff is updating for first approve
        if (transaction.HandlePersonId is null && transaction.Status.Equals("pending"))
        {
            Guid staffURid = Guid.Empty;
            var userRoles = await GetCurrentUserRoles();
            foreach (var ur in userRoles)
            {
                var currentRole = await _unitOfWork.GetRoleRepository().GetOneAsync(r => r.Id == ur.RoleId);
                if (currentRole.Name.ToLower().Equals("staff"))
                    staffURid = ur.Id;
            }

            if (staffURid == Guid.Empty)
                throw new BadRequestException("Unknown Who Is Handle This Transaction!");

            transaction.HandlePersonId = staffURid;
            transaction.Status = "awaiting";
        }

        return await _unitOfWork.GetTransactionRepository().SaveChangeAsync();
    }

    //=================================================================================
    private async Task<List<UserRole>?> GetCurrentUserRoles()
    {
        _ = Guid.TryParse(_userContextService.GetCurrentUserId(), out Guid accId);

        var existAccount = await _unitOfWork.GetAccountRepository().GetOneAsync(a => a.Id == accId, null, false) ??
            throw new NotFoundException("Your current session account Id is not exist in database! Can't find your cv");

        var listUserRole = await _unitOfWork.GetUserRoleRepository().GetListAdvanceAsync(ur =>
            ur.AccountId == accId &&
            ur.ProjectId == null &&
            ur.AppraisalCouncilId == null, null, false) ??
        throw new NotFoundException("Not found your base roles or it is expired in system!");

        return listUserRole;
    }
}