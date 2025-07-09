namespace SRPM_Repositories.Repositories.Interfaces;

public interface IUnitOfWork
{
    Task<bool> SaveChangesAsync();

    IAccountNotificationRepository GetAccountNotificationRepository();
    IAccountRepository GetAccountRepository();
    IAppraisalCouncilRepository GetAppraisalCouncilRepository();
    IDocumentRepository GetDocumentRepository();
    IDocumentSectionRepository GetDocumentSectionRepository();
    IEvaluationRepository GetEvaluationRepository();
    IEvaluationStageRepository GetEvaluationStageRepository();
    IIndividualEvaluationRepository GetIndividualEvaluationRepository();
    IMajorRepository GetMajorRepository();
    IMemberTaskRepository GetMemberTaskRepository();
    IMilestoneRepository GetMilestoneRepository();
    INotificationRepository GetNotificationRepository();
    IOTPCodeRepository GetOTPCodeRepository();
    IProjectRepository GetProjectRepository();
    IProjectMajorRepository GetProjectMajorRepository();
    IProjectTagRepository GetProjectTagRepository();
    IResearchPaperRepository GetResearchPaperRepository();
    IRoleRepository GetRoleRepository();
    ISectionContentRepository GetSectionContentRepository();
    ISignatureRepository GetSignatureRepository();
    ISystemConfigurationRepository GetSystemConfigurationRepository();
    ITableRowRepository GetTableRowRepository();
    ITableStructureRepository GetTableStructureRepository();
    ITaskRepository GetTaskRepository();
    ITransactionRepository GetTransactionRepository();
    IUserRoleRepository GetUserRoleRepository();


    // Add other repositories...
}