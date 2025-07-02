namespace SRPM_Repositories.Repositories.Interfaces;

public interface IUnitOfWork
{
    Task<bool> SaveChangesAsync();

    IAccountNotificationRepository GetAccountNotificationRepository();
    IAccountRepository GetAccountRepository();
    IAppraisalCouncilRepository GetAppraisalCouncilRepository();
    IContentTableRepository GetContentTableRepository();
    IDocumentFieldRepository GetDocumentFieldRepository();
    IDocumentRepository GetDocumentRepository();
    IEvaluationRepository GetEvaluationRepository();
    IEvaluationStageRepository GetEvaluationStageRepository();
    IFieldContentRepository GetFieldContentRepository();
    IIndividualEvaluationRepository GetIndividualEvaluationRepository();
    IMemberTaskRepository GetMemberTaskRepository();
    INotificationRepository GetNotificationRepository();
    ISystemConfigurationRepository GetSystemConfigurationRepository();
    ITaskRepository GetTaskRepository();
    ITransactionRepository GetTransactionRepository();
    IUserRoleRepository GetUserRoleRepository();
    IMajorRepository GetMajorRepository();
    IMilestoneRepository GetMilestoneRepository();
    IOTPCodeRepository GetOTPCodeRepository();
    IProjectRepository GetProjectRepository();
    IProjectMajorRepository GetProjectMajorRepository();
    IProjectTagRepository GetProjectTagRepository();
    IResearchPaperRepository GetResearchPaperRepository();
    IRoleRepository GetRoleRepository();
    ISignatureRepository GetSignatureRepository();


    // Add other repositories...
}