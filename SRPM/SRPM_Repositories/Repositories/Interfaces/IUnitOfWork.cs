namespace SRPM_Repositories.Repositories.Interfaces;

public interface IUnitOfWork
{
    Task<bool> SaveChangesAsync();

    IAccountRepository GetAccountRepository();
    IAccountNotificationRepository GetAccountNotificationRepository();
    IDocumentRepository GetDocumentRepository();
    IEvaluationRepository GetEvaluationRepository();
    IEvaluationStageRepository GetEvaluationStageRepository();
    IIndividualEvaluationRepository GetIndividualEvaluationRepository();
    IMemberTaskRepository GetMemberTaskRepository();
    INotificationRepository GetNotificationRepository();
    ISystemConfigurationRepository GetSystemConfigurationRepository();
    ITaskRepository GetTaskRepository();
    ITransactionRepository GetTransactionRepository();
    IUserRoleRepository GetUserRoleRepository();

    // Add other repositories...
}