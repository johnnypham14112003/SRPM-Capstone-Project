using Mapster;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Services.BusinessModels;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Extensions.Exceptions;
using SRPM_Services.Interfaces;

namespace SRPM_Services.Implements;
public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    //Relate ModelRepo
    private readonly IAccountNotificationRepository _accountNotificationRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IIndividualEvaluationRepository _individualEvaluationRepository;
    private readonly IEvaluationStageRepository _evaluationStageRepository;
    private readonly IEvaluationRepository _evaluationRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IMemberTaskRepository _memberTaskRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly ISystemConfigurationRepository _systemConfigurationRepository;
    public NotificationService(INotificationRepository notificationRepository,
        IAccountNotificationRepository accountNotificationRepository,
        ITransactionRepository transactionRepository,
        IIndividualEvaluationRepository individualEvaluationRepository,
        IEvaluationStageRepository evaluationStageRepository,
        IEvaluationRepository evaluationRepository,
        IUserRoleRepository userRoleRepository,
        IDocumentRepository documentRepository,
        IMemberTaskRepository memberTaskRepository,
        ITaskRepository taskRepository,
        ISystemConfigurationRepository systemConfigurationRepository)
    {
        _notificationRepository = notificationRepository;
        _accountNotificationRepository = accountNotificationRepository;
        _transactionRepository = transactionRepository;
        _individualEvaluationRepository = individualEvaluationRepository;
        _evaluationStageRepository = evaluationStageRepository;
        _evaluationRepository = evaluationRepository;
        _userRoleRepository = userRoleRepository;
        _documentRepository = documentRepository;
        _memberTaskRepository = memberTaskRepository;
        _taskRepository = taskRepository;
        _systemConfigurationRepository = systemConfigurationRepository;
    }
    //=============================================================================
    public async Task<(bool, Guid)> CreateNew(RQ_Notification newNotification)
    {
        //Check Null Data
        bool hasInvalidFields = new[] { newNotification.Title, newNotification.Type }
        .Any(string.IsNullOrWhiteSpace);

        if (hasInvalidFields) throw new BadRequestException("Cannot create blank notification");

        //Transfer Data
        Notification notificationDTO = newNotification.Adapt<Notification>();//Assign handmade in switch below

        switch (notificationDTO.Type.ToLower())
        {
            case "transaction":
                var existTr = await _transactionRepository.GetOneAsync(tr => tr.Id == newNotification.ObjecNotificationId, false)
                    ?? throw new NotFoundException("Create Notification Faild: Not found any Transaction relate to this Id"); ;
                notificationDTO.TransactionId = newNotification.ObjecNotificationId;
                notificationDTO.Transaction = existTr;
                break;
            case "individualevaluation":
                var existIE = await _individualEvaluationRepository.GetOneAsync(ie => ie.Id == newNotification.ObjecNotificationId, false)
                    ?? throw new NotFoundException("Create Notification Faild: Not found any IndividualEvaluation relate to this Id"); ;
                notificationDTO.IndividualEvaluationId = newNotification.ObjecNotificationId;
                notificationDTO.IndividualEvaluation = existIE;
                break;
            case "evaluationstage":
                var existES = await _evaluationStageRepository.GetOneAsync(es => es.Id == newNotification.ObjecNotificationId, false)
                    ?? throw new NotFoundException("Create Notification Faild: Not found any EvaluationStage relate to this Id"); ;
                notificationDTO.EvaluationStageId = newNotification.ObjecNotificationId;
                notificationDTO.EvaluationStage = existES;
                break;
            case "evaluation":
                var existE = await _evaluationRepository.GetOneAsync(e => e.Id == newNotification.ObjecNotificationId, false)
                    ?? throw new NotFoundException("Create Notification Faild: Not found any Evaluation relate to this Id"); ;
                notificationDTO.EvaluationId = newNotification.ObjecNotificationId;
                notificationDTO.Evaluation = existE;
                break;
            case "userrole":
                var existUR = await _userRoleRepository.GetOneAsync(ur => ur.Id == newNotification.ObjecNotificationId, false)
                    ?? throw new NotFoundException("Create Notification Faild: Not found any GroupUser relate to this Id"); ;
                notificationDTO.UserRoleId = newNotification.ObjecNotificationId;
                notificationDTO.UserRole = existUR;
                break;
            case "document":
                var existD = await _documentRepository.GetOneAsync(d => d.Id == newNotification.ObjecNotificationId, false)
                    ?? throw new NotFoundException("Create Notification Faild: Not found any Document relate to this Id"); ;
                notificationDTO.DocumentId = newNotification.ObjecNotificationId;
                notificationDTO.Document = existD;
                break;
            case "membertask":
                var existMT = await _memberTaskRepository.GetOneAsync(mt => mt.Id == newNotification.ObjecNotificationId, false)
                    ?? throw new NotFoundException("Create Notification Faild: Not found any MemberTask relate to this Id"); ;
                notificationDTO.MemberTaskId = newNotification.ObjecNotificationId;
                notificationDTO.MemberTask = existMT;
                break;
            case "task":
                var existTa = await _taskRepository.GetOneAsync(ta => ta.Id == newNotification.ObjecNotificationId, false)
                    ?? throw new NotFoundException("Create Notification Faild: Not found any Task relate to this Id"); ;
                notificationDTO.TaskId = newNotification.ObjecNotificationId;
                notificationDTO.Task = existTa;
                break;
            default: //"systemconfiguration"
                var existSC = await _systemConfigurationRepository.GetOneAsync(sc => sc.Id == newNotification.ObjecNotificationId, false)
                    ?? throw new NotFoundException("Create Notification Faild: Not found any SystemConfiguration relate to this Id");
                notificationDTO.SystemConfigurationId = newNotification.ObjecNotificationId;
                notificationDTO.SystemConfiguration = existSC;
                break;
        }

        await _notificationRepository.AddAsync(notificationDTO);
        var saveSuccess = await _notificationRepository.SaveChangeAsync();

        //If is global send then do it instancely
        if (notificationDTO.IsGlobalSend) await NotificateToUser(null, notificationDTO.Id);
        return (saveSuccess, notificationDTO.Id);
    }

    public async Task<bool> NotificateToUser(List<Guid>? ListAccountId, Guid notificationId)
    {
        var existNoti = await _notificationRepository.GetOneAsync(n => n.Id == notificationId, false)
                    ?? throw new NotFoundException("Not found any Notification match the Id");

        //Handle if list null
        if (ListAccountId is null || !ListAccountId.Any())
            ListAccountId = await _accountNotificationRepository.ListIdAllAccount();

        //Transfer list AccountId into list AccountNotification
        var accountNotificationDTOs = ListAccountId!.Select(accountId =>
        new AccountNotification
        {
            AccountId = accountId,
            NotificationId = notificationId,
            CreateDate = existNoti.CreateDate
        });

        await _accountNotificationRepository.AddRangeAsync(accountNotificationDTOs);
        return await _accountNotificationRepository.SaveChangeAsync();
    }

    public async Task<PagingResult<RS_AccountNotification>?> ListNotificationOfUser(RQ_QueryAccountNotification queryInput)
    {
        //Re-assign value if it smaller than 1
        queryInput.PageIndex = queryInput.PageIndex < 1 ? 1 : queryInput.PageIndex;
        queryInput.PageSize = queryInput.PageIndex < 1 ? 1 : queryInput.PageIndex;

        var dataResult = await _accountNotificationRepository.ListAccountNotification
            (queryInput.AccountId, queryInput.KeyWord, queryInput.FromDate, queryInput.ToDate,
            queryInput.IsRead, queryInput.Type, queryInput.Status, queryInput.PageIndex, queryInput.PageSize);

        // Check Null Result
        if (dataResult.totalCount == 0) throw new NotFoundException("There are no record of any Notification in AccountNotification");
        if (dataResult.listNotificationWithStatus is null || dataResult.listNotificationWithStatus.Count == 0)
            throw new NotFoundException("Not Found Any Notification Of This User");

        return new PagingResult<RS_AccountNotification>
        {
            PageIndex = queryInput.PageIndex,
            PageSize = queryInput.PageSize,
            TotalCount = dataResult.totalCount,
            DataList = dataResult.listNotificationWithStatus.Adapt<List<RS_AccountNotification>>()
        };
    }

    public async Task<bool> UpdateNotification(RQ_Notification newNotification)
    {
        var existNoti = await _notificationRepository.GetOneAsync(n => n.Id == newNotification.Id)
                    ?? throw new NotFoundException("Not found any Notification match the Id");

        //Check Null Data
        bool hasInvalidFields = new[] { newNotification.Title, newNotification.Status }
        .Any(string.IsNullOrWhiteSpace);

        if (hasInvalidFields) throw new BadRequestException("Cannot set Title or Status empty!");

        //Update new Data to old Data
        existNoti.Title = newNotification.Title;
        existNoti.Status = newNotification.Status;

        return await _notificationRepository.SaveChangeAsync();
    }

    public async Task<bool> DeleteNotification(Guid id)
    {
        var existNoti = await _notificationRepository.GetOneAsync(n => n.Id == id, false)
                    ?? throw new NotFoundException("Not found any Notification match the Id");

        await _notificationRepository.DeleteAsync(existNoti);
        return await _notificationRepository.SaveChangeAsync();
    }
}