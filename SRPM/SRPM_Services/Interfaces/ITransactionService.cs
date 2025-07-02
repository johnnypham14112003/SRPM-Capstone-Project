using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;

namespace SRPM_Services.Interfaces;

public interface ITransactionService
{
    Task<(bool result, Guid transactionId)> NewTransaction(RQ_Transaction inputData);
    Task<RS_Transaction?> GetTransactionById(Guid id);
    Task<bool> UpdateTransaction(Guid id, RQ_Transaction inputData);
    Task<bool> DeleteTransaction(Guid id);
}