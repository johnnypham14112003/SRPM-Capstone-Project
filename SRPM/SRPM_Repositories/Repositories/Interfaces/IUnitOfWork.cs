using Microsoft.EntityFrameworkCore;
using System.Data;

namespace SRPM_Repositories.Repositories.Interfaces;

public interface IUnitOfWork
{
    Task<bool> SaveChangesAsync();
    Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
    Task CommitAsync();
    Task RollbackAsync();


    IAccountNotificationRepository GetAccountNotificationRepository();
    IAccountRepository GetAccountRepository();
    IAppraisalCouncilRepository GetAppraisalCouncilRepository();
    IDocumentRepository GetDocumentRepository();
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
    IProjectSimilarityRepository GetProjectSimilarityRepository();
    IProjectTagRepository GetProjectTagRepository();
    IProjectResultRepository GetProjectResultRepository();
    IResultPublishRepository GetResultPublishRepository();
    IRoleRepository GetRoleRepository();
    ISignatureRepository GetSignatureRepository();
    ISystemConfigurationRepository GetSystemConfigurationRepository();
    ITaskRepository GetTaskRepository();
    ITransactionRepository GetTransactionRepository();
    IUserRoleRepository GetUserRoleRepository();
    IFieldRepository GetFieldRepository();


    // Add other repositories...
}