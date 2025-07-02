using Mapster;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Interfaces;

namespace SRPM_Services.Implements;

public class TransactionService : ITransactionService
{
    private readonly IUnitOfWork _unitOfWork;
    public TransactionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    //=============================================================================
    public async Task<(bool result, Guid transactionId)> NewTransaction(RQ_Transaction inputData)
    {
        var transactionDTO = inputData.Adapt<Transaction>();
        await _unitOfWork.GetTransactionRepository().AddAsync(transactionDTO);
        var result = await _unitOfWork.GetTransactionRepository().SaveChangeAsync();
        return (result, transactionDTO.Id);
    }

    public async Task<RS_Transaction?> GetTransactionById(Guid id)
    {
        var existTransaction = await _unitOfWork.GetTransactionRepository().GetByIdAsync(id);
        return existTransaction.Adapt<RS_Transaction>();
    }

    public async Task<bool> UpdateTransaction(Guid id, RQ_Transaction inputData)
    {
        var transaction = await _unitOfWork.GetTransactionRepository().GetByIdAsync(id);
        if (transaction == null) return false;

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
}