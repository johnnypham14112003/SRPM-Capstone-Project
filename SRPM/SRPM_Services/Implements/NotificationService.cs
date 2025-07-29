using Mapster;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Implements;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Services.BusinessModels;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.RequestModels.Query;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Extensions.BackgroundService;
using SRPM_Services.Extensions.Exceptions;
using SRPM_Services.Extensions.FluentEmail;
using SRPM_Services.Interfaces;

namespace SRPM_Services.Implements;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public NotificationService(IUnitOfWork unitOfWork,
        IUserContextService userContextService)
    {
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
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

        //Get Exist Object Id then assign
        switch (notificationDTO.Type.ToLower())
        {
            case "transaction":
                var existTr = await _unitOfWork.GetTransactionRepository().GetOneAsync(tr => tr.Id == newNotification.ObjecNotificationId, null, false)
                    ?? throw new NotFoundException("Create Notification Faild: Not found any Transaction relate to this Id"); ;
                notificationDTO.TransactionId = newNotification.ObjecNotificationId;
                break;
            case "individualevaluation":
                var existIE = await _unitOfWork.GetIndividualEvaluationRepository().GetOneAsync(ie => ie.Id == newNotification.ObjecNotificationId, null, false)
                    ?? throw new NotFoundException("Create Notification Faild: Not found any IndividualEvaluation relate to this Id"); ;
                notificationDTO.IndividualEvaluationId = newNotification.ObjecNotificationId;
                break;
            case "evaluationstage":
                var existES = await _unitOfWork.GetEvaluationStageRepository().GetOneAsync(es => es.Id == newNotification.ObjecNotificationId, null, false)
                    ?? throw new NotFoundException("Create Notification Faild: Not found any EvaluationStage relate to this Id"); ;
                notificationDTO.EvaluationStageId = newNotification.ObjecNotificationId;
                break;
            case "evaluation":
                var existE = await _unitOfWork.GetEvaluationRepository().GetOneAsync(e => e.Id == newNotification.ObjecNotificationId, null, false)
                    ?? throw new NotFoundException("Create Notification Faild: Not found any Evaluation relate to this Id"); ;
                notificationDTO.EvaluationId = newNotification.ObjecNotificationId;
                break;
            case "userrole":
                var existUR = await _unitOfWork.GetUserRoleRepository().GetOneAsync(ur => ur.Id == newNotification.ObjecNotificationId, null, false)
                    ?? throw new NotFoundException("Create Notification Faild: Not found any GroupUser relate to this Id"); ;
                notificationDTO.UserRoleId = newNotification.ObjecNotificationId;
                break;
            case "document":
                var existD = await _unitOfWork.GetDocumentRepository().GetOneAsync(d => d.Id == newNotification.ObjecNotificationId, null, false)
                    ?? throw new NotFoundException("Create Notification Faild: Not found any Document relate to this Id"); ;
                notificationDTO.DocumentId = newNotification.ObjecNotificationId;
                break;
            case "membertask":
                var existMT = await _unitOfWork.GetMemberTaskRepository().GetOneAsync(mt => mt.Id == newNotification.ObjecNotificationId, null, false)
                    ?? throw new NotFoundException("Create Notification Faild: Not found any MemberTask relate to this Id"); ;
                notificationDTO.MemberTaskId = newNotification.ObjecNotificationId;
                break;
            case "task":
                var existTa = await _unitOfWork.GetTaskRepository().GetOneAsync(ta => ta.Id == newNotification.ObjecNotificationId, null, false)
                    ?? throw new NotFoundException("Create Notification Faild: Not found any Task relate to this Id"); ;
                notificationDTO.TaskId = newNotification.ObjecNotificationId;
                break;
            default: //"systemconfiguration"
                var existSC = await _unitOfWork.GetSystemConfigurationRepository().GetOneAsync(sc => sc.Id == newNotification.ObjecNotificationId, null, false)
                    ?? throw new NotFoundException("Create Notification Faild: Not found any SystemConfiguration relate to this Id");
                notificationDTO.SystemConfigurationId = newNotification.ObjecNotificationId;
                break;
        }

        await _unitOfWork.GetNotificationRepository().AddAsync(notificationDTO);
        var saveSuccess = await _unitOfWork.GetNotificationRepository().SaveChangeAsync();

        //If is global send then do it instancely
        if (notificationDTO.IsGlobalSend) await NotificateToUser(null, notificationDTO.Id);
        return (saveSuccess, notificationDTO.Id);
    }

    public async Task<bool> NotificateToUser(List<Guid>? ListAccountId, Guid notificationId)
    {
        var existNoti = await _unitOfWork.GetNotificationRepository().GetOneAsync(n => n.Id == notificationId, null, false)
                    ?? throw new NotFoundException("Not found any Notification match the Id");

        //Handle if list null -> get all user
        if (ListAccountId is null || !ListAccountId.Any())
            ListAccountId = await _unitOfWork.GetAccountNotificationRepository().ListIdAllAccount();

        //Transfer list AccountId into list AccountNotification
        var accountNotificationDTOs = ListAccountId!.Select(accountId =>
        new AccountNotification
        {
            AccountId = accountId,
            NotificationId = notificationId,
            CreateDate = existNoti.CreateDate
        });

        await _unitOfWork.GetAccountNotificationRepository().AddRangeAsync(accountNotificationDTOs);
        return await _unitOfWork.GetAccountNotificationRepository().SaveChangeAsync();
    }

    public async Task<PagingResult<RS_AccountNotification>?> ListNotificationOfUser(Q_AccountNotification queryInput)
    {
        List<NotificationWithReadStatus>? listNoti = null;
        int totalCount = 0;

        //Re-assign value if it smaller than 1
        queryInput.PageIndex = queryInput.PageIndex < 1 ? 1 : queryInput.PageIndex;
        queryInput.PageSize = queryInput.PageIndex < 1 ? 1 : queryInput.PageIndex;

        Guid accId = Guid.Empty;
        if (string.IsNullOrWhiteSpace(queryInput.Email))
        {
            _ = Guid.TryParse(_userContextService.GetCurrentUserId(), out accId);

            (listNoti, totalCount) = await _unitOfWork.GetAccountNotificationRepository().ListAccountNotification
                (accId, queryInput.KeyWord, queryInput.FromDate, queryInput.ToDate,
                queryInput.IsRead, queryInput.Type, queryInput.Status, queryInput.PageIndex, queryInput.PageSize);
        }

        // Checking Result
        if (listNoti is null || listNoti.Count == 0)
            throw new NotFoundException("Not Found Any Notification Of This User");

        return new PagingResult<RS_AccountNotification>
        {
            PageIndex = queryInput.PageIndex,
            PageSize = queryInput.PageSize,
            TotalCount = totalCount,
            DataList = listNoti.Adapt<List<RS_AccountNotification>>()
        };
    }

    public async Task<bool> ReadNotification(Guid? notificationId)
    {
        _ = Guid.TryParse(_userContextService.GetCurrentUserId(), out Guid accId);

        var repoAN = _unitOfWork.GetAccountNotificationRepository();
        if (notificationId.HasValue)
        {
            var existAccountNoti = await repoAN.GetOneAsync(an => an.NotificationId == notificationId && an.AccountId == accId)
                ?? throw new NotFoundException("Not found this Notification of current session user");

            existAccountNoti.IsRead = true;
        }
        else
        {
            var allNotis = await repoAN.GetListAdvanceAsync(an => an.AccountId == accId && an.IsRead == false, null)
                ?? throw new NotFoundException("Not found any Unread Notification of current session user");

            foreach (var noti in allNotis)
            {
                noti.IsRead = true;
            }
        }

        return await _unitOfWork.GetAccountNotificationRepository().SaveChangeAsync();
    }

    public async Task<bool> UpdateNotification(RQ_Notification newNotification)
    {
        var existNoti = await _unitOfWork.GetNotificationRepository().GetOneAsync(n => n.Id == newNotification.Id)
                    ?? throw new NotFoundException("Not found any Notification match the Id");

        //Check Null Data
        bool hasInvalidFields = new[] { newNotification.Title, newNotification.Status }
        .Any(string.IsNullOrWhiteSpace);

        if (hasInvalidFields) throw new BadRequestException("Cannot set Title or Status empty!");

        //Update new Data to old Data
        existNoti.Title = newNotification.Title!;
        existNoti.Status = newNotification.Status;

        //Change read status if update notification
        var accountNotification = await _unitOfWork.GetAccountNotificationRepository()
            .GetListAdvanceAsync(an => an.NotificationId == existNoti.Id);

        if (accountNotification is not null)
        {
            foreach (var noti in accountNotification)
            {
                noti.IsRead = false;
            }
        }

        return await _unitOfWork.GetNotificationRepository().SaveChangeAsync();
    }

    public async Task<bool> DeleteNotification(Guid id)
    {
        var existNoti = await _unitOfWork.GetNotificationRepository().GetOneAsync(n => n.Id == id, null, false)
                    ?? throw new NotFoundException("Not found any Notification match the Id");

        //delete reference
        var relateUserNoti = await _unitOfWork.GetAccountNotificationRepository()
            .GetListAdvanceAsync(an => an.NotificationId == id, null, false);
        if (relateUserNoti is not null)
            await _unitOfWork.GetAccountNotificationRepository().DeleteRangeAsync(relateUserNoti);

        await _unitOfWork.GetNotificationRepository().DeleteAsync(existNoti);
        return await _unitOfWork.GetNotificationRepository().SaveChangeAsync();
    }
}