using SRPM_Repositories.Repositories.Interfaces;

namespace SRPM_Repositories.Repositories.Repositories;

public class UnitOfWork : IUnitOfWork
{
    //Declare DI
    private readonly SRPMDbContext _context;
    private readonly Lazy<IAccountRepository> _accountRepository;//Lazy (initial when needed)
    private readonly Lazy<IAccountNotificationRepository> _accountNotificationRepository;
    private readonly Lazy<IDocumentRepository> _documentRepository;
    private readonly Lazy<IEvaluationRepository> _evaluationRepository;
    private readonly Lazy<IEvaluationStageRepository> _evaluationStageRepository;
    private readonly Lazy<IIndividualEvaluationRepository> _individualEvaluationRepository;
    private readonly Lazy<IMemberTaskRepository> _memberTaskRepository;
    private readonly Lazy<INotificationRepository> _notificationRepository;
    private readonly Lazy<ISystemConfigurationRepository> _systemConfigurationRepository;
    private readonly Lazy<ITaskRepository> _taskRepository;
    private readonly Lazy<ITransactionRepository> _transactionRepository;
    private readonly Lazy<IUserRoleRepository> _userRoleRepository;

    //======================================================================================
    //Constructor
    public UnitOfWork(SRPMDbContext context)
    {
        _context = context;
        _accountNotificationRepository = new Lazy<IAccountNotificationRepository>
            (() => new AccountNotificationRepository(context));

        _accountRepository = new Lazy<IAccountRepository>
            (() => new AccountRepository(context));

        _documentRepository = new Lazy<IDocumentRepository>
            (() => new DocumentRepository(context));

        _evaluationRepository = new Lazy<IEvaluationRepository>
            (() => new EvaluationRepository(context));

        _evaluationStageRepository = new Lazy<IEvaluationStageRepository>
            (() => new EvaluationStageRepository(context));

        _individualEvaluationRepository = new Lazy<IIndividualEvaluationRepository>
            (() => new IndividualEvaluationRepository(context));

        _memberTaskRepository = new Lazy<IMemberTaskRepository>
            (() => new MemberTaskRepository(context));

        _notificationRepository = new Lazy<INotificationRepository>
            (() => new NotificationRepository(context));

        _systemConfigurationRepository = new Lazy<ISystemConfigurationRepository>
            (() => new SystemConfigurationRepository(context));

        _taskRepository = new Lazy<ITaskRepository>
            (() => new TaskRepository(context));

        _transactionRepository = new Lazy<ITransactionRepository>
            (() => new TransactionRepository(context));

        _userRoleRepository = new Lazy<IUserRoleRepository>
            (() => new UserRoleRepository(context));
    }

    //======================================================================================
    //Methods Expose Repository
    public async Task<bool> SaveChangesAsync()
        => await _context.SaveChangesAsync() > 0;

    public IAccountNotificationRepository GetAccountNotificationRepository()
        => _accountNotificationRepository.Value;
    public IAccountRepository GetAccountRepository()
        => _accountRepository.Value;
    public IDocumentRepository GetDocumentRepository()
        => _documentRepository.Value;
    public IEvaluationRepository GetEvaluationRepository()
        => _evaluationRepository.Value;
    public IEvaluationStageRepository GetEvaluationStageRepository()
        => _evaluationStageRepository.Value;
    public IIndividualEvaluationRepository GetIndividualEvaluationRepository()
        => _individualEvaluationRepository.Value;
    public IMemberTaskRepository GetMemberTaskRepository()
        => _memberTaskRepository.Value;
    public INotificationRepository GetNotificationRepository()
        => _notificationRepository.Value;
    public ISystemConfigurationRepository GetSystemConfigurationRepository()
        => _systemConfigurationRepository.Value;
    public ITaskRepository GetTaskRepository()
        => _taskRepository.Value;
    public ITransactionRepository GetTransactionRepository()
        => _transactionRepository.Value;
    public IUserRoleRepository GetUserRoleRepository()
        => _userRoleRepository.Value;
}