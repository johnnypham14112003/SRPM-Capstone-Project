using SRPM_Services.BusinessModels;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.RequestModels.Query;
using SRPM_Services.BusinessModels.ResponseModels;

namespace SRPM_Services.Interfaces;

public interface ITransactionService
{
    Task<(bool result, Guid transactionId)> NewTransaction(RQ_Transaction inputData);
    Task<RS_Transaction?> GetTransactionById(Guid id);
    Task<PagingResult<RS_Transaction>?> ListTransaction(Q_Transaction queryInput);
    Task<bool> UpdateTransaction(RQ_Transaction inputData);
    Task<bool> DeleteTransaction(Guid id);
    Task<bool> UpdateStatusTransaction(Guid transactionId, string status);
}