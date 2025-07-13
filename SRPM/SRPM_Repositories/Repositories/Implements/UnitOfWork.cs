using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Repositories.Repositories.Implements;

namespace SRPM_Repositories.Repositories.Implements;

public class UnitOfWork : IUnitOfWork
{
    //Declare DI
    private readonly SRPMDbContext _context;
    //Lazy (initial when needed)
    private readonly Lazy<IAccountNotificationRepository> _accountNotificationRepository;
    private readonly Lazy<IAccountRepository> _accountRepository;
    private readonly Lazy<IAppraisalCouncilRepository> _appraisalCouncilRepository;
    private readonly Lazy<IDocumentRepository> _documentRepository;
    private readonly Lazy<IEvaluationRepository> _evaluationRepository;
    private readonly Lazy<IEvaluationStageRepository> _evaluationStageRepository;
    private readonly Lazy<IIndividualEvaluationRepository> _individualEvaluationRepository;
    private readonly Lazy<IMajorRepository> _majorRepository;
    private readonly Lazy<IMemberTaskRepository> _memberTaskRepository;
    private readonly Lazy<IMilestoneRepository> _milestoneRepository;
    private readonly Lazy<INotificationRepository> _notificationRepository;
    private readonly Lazy<IOTPCodeRepository> _otpCodeRepository;
    private readonly Lazy<IProjectMajorRepository> _projectMajorRepository;
    private readonly Lazy<IProjectRepository> _projectRepository;
    private readonly Lazy<IProjectTagRepository> _projectTagRepository;
    private readonly Lazy<IResearchPaperRepository> _researchPaperRepository;
    private readonly Lazy<IRoleRepository> _roleRepository;
    private readonly Lazy<ISignatureRepository> _signatureRepository;
    private readonly Lazy<ISystemConfigurationRepository> _systemConfigurationRepository;
    private readonly Lazy<ITaskRepository> _taskRepository;
    private readonly Lazy<ITransactionRepository> _transactionRepository;
    private readonly Lazy<IUserRoleRepository> _userRoleRepository;
    private readonly Lazy<IFieldRepository> _fieldRepository;


    //======================================================================================
    //Constructor
    public UnitOfWork(SRPMDbContext context)
    {
        _context = context;
        _accountNotificationRepository = new Lazy<IAccountNotificationRepository>
            (() => new AccountNotificationRepository(context));

        _accountRepository = new Lazy<IAccountRepository>
            (() => new AccountRepository(context));

        _appraisalCouncilRepository = new Lazy<IAppraisalCouncilRepository>
            (() => new AppraisalCouncilRepository(context));

        _documentRepository = new Lazy<IDocumentRepository>
            (() => new DocumentRepository(context));

        _evaluationRepository = new Lazy<IEvaluationRepository>
            (() => new EvaluationRepository(context));

        _evaluationStageRepository = new Lazy<IEvaluationStageRepository>
            (() => new EvaluationStageRepository(context));

        _individualEvaluationRepository = new Lazy<IIndividualEvaluationRepository>
            (() => new IndividualEvaluationRepository(context));

        _majorRepository = new Lazy<IMajorRepository>
            (() => new MajorRepository(context));

        _memberTaskRepository = new Lazy<IMemberTaskRepository>
            (() => new MemberTaskRepository(context));

        _milestoneRepository = new Lazy<IMilestoneRepository>
            (() => new MilestoneRepository(context));

        _notificationRepository = new Lazy<INotificationRepository>
            (() => new NotificationRepository(context));

        _otpCodeRepository = new Lazy<IOTPCodeRepository>
            (() => new OTPCodeRepository(context));

        _projectMajorRepository = new Lazy<IProjectMajorRepository>
            (() => new ProjectMajorRepository(context));

        _projectRepository = new Lazy<IProjectRepository>
            (() => new ProjectRepository(context));

        _projectTagRepository = new Lazy<IProjectTagRepository>
            (() => new ProjectTagRepository(context));

        _researchPaperRepository = new Lazy<IResearchPaperRepository>
            (() => new ResearchPaperRepository(context));

        _roleRepository = new Lazy<IRoleRepository>
            (() => new RoleRepository(context));

        _signatureRepository = new Lazy<ISignatureRepository>
            (() => new SignatureRepository(context));

        _systemConfigurationRepository = new Lazy<ISystemConfigurationRepository>
            (() => new SystemConfigurationRepository(context));

        _taskRepository = new Lazy<ITaskRepository>
            (() => new TaskRepository(context));

        _transactionRepository = new Lazy<ITransactionRepository>
            (() => new TransactionRepository(context));

        _userRoleRepository = new Lazy<IUserRoleRepository>
            (() => new UserRoleRepository(context));
        _fieldRepository = new Lazy<IFieldRepository>
            (() => new FieldRepository(context));

    }

    //======================================================================================
    //Methods Expose Repository
    public async Task<bool> SaveChangesAsync()
        => await _context.SaveChangesAsync() > 0;

    public IAccountNotificationRepository GetAccountNotificationRepository()
        => _accountNotificationRepository.Value;
    public IAccountRepository GetAccountRepository()
        => _accountRepository.Value;
    public IAppraisalCouncilRepository GetAppraisalCouncilRepository()
        => _appraisalCouncilRepository.Value;
    public IDocumentRepository GetDocumentRepository()
        => _documentRepository.Value;
    public IEvaluationRepository GetEvaluationRepository()
        => _evaluationRepository.Value;
    public IEvaluationStageRepository GetEvaluationStageRepository()
        => _evaluationStageRepository.Value;
    public IIndividualEvaluationRepository GetIndividualEvaluationRepository()
        => _individualEvaluationRepository.Value;
    public IMajorRepository GetMajorRepository()
    => _majorRepository.Value;
    public IMemberTaskRepository GetMemberTaskRepository()
        => _memberTaskRepository.Value;
    public IMilestoneRepository GetMilestoneRepository()
        => _milestoneRepository.Value;
    public INotificationRepository GetNotificationRepository()
        => _notificationRepository.Value;
    public IOTPCodeRepository GetOTPCodeRepository()
        => _otpCodeRepository.Value;
    public IProjectMajorRepository GetProjectMajorRepository()
        => _projectMajorRepository.Value;
    public IProjectRepository GetProjectRepository()
        => _projectRepository.Value;
    public IProjectTagRepository GetProjectTagRepository()
        => _projectTagRepository.Value;
    public IResearchPaperRepository GetResearchPaperRepository()
        => _researchPaperRepository.Value;
    public IRoleRepository GetRoleRepository()
        => _roleRepository.Value;
    public ISignatureRepository GetSignatureRepository()
        => _signatureRepository.Value;
    public ISystemConfigurationRepository GetSystemConfigurationRepository()
        => _systemConfigurationRepository.Value;
    public ITaskRepository GetTaskRepository()
        => _taskRepository.Value;
    public ITransactionRepository GetTransactionRepository()
        => _transactionRepository.Value;
    public IUserRoleRepository GetUserRoleRepository()
        => _userRoleRepository.Value;
    public IFieldRepository GetFieldRepository()
        => _fieldRepository.Value;

}