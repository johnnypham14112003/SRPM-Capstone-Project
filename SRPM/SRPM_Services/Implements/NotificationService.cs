using Mapster;
using Microsoft.AspNetCore.SignalR;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Implements;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Services.BusinessModels;
using SRPM_Services.BusinessModels.Others;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.RequestModels.Query;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Extensions.Exceptions;
using SRPM_Services.Extensions.FluentEmail;
using SRPM_Services.Extensions.Hubs;
using SRPM_Services.Interfaces;

namespace SRPM_Services.Implements;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;
    private readonly IEmailService _emailService;
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(IUnitOfWork unitOfWork,
        IUserContextService userContextService,
        IEmailService emailService, IHubContext<NotificationHub> hubContext)
    {
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
        _emailService = emailService;
        _hubContext = hubContext;
    }

    //=============================================================================
    public async Task<(bool, Guid)> CreateNew(RQ_Notification newNotification)
    {
        // Your existing validation code
        bool hasInvalidFields = new[] { newNotification.Title, newNotification.Type }
            .Any(string.IsNullOrWhiteSpace);

        if (hasInvalidFields) throw new BadRequestException("Cannot create blank notification");

        // Your existing data transfer
        Notification notificationDTO = newNotification.Adapt<Notification>();

        // Your existing switch statement (unchanged)
        switch (notificationDTO.Type.ToLower())
        {
            case "project":
                _ = await _unitOfWork.GetProjectRepository().GetOneAsync(p => p.Id == newNotification.ObjecNotificationId, null, false)
                    ?? throw new NotFoundException("Create Notification Failed: Not found any Project relate to this Id");
                notificationDTO.ProjectId = newNotification.ObjecNotificationId;
                break;
            case "appraisalcouncil":
                _ = await _unitOfWork.GetAppraisalCouncilRepository().GetOneAsync(ac => ac.Id == newNotification.ObjecNotificationId, null, false)
                    ?? throw new NotFoundException("Create Notification Failed: Not found any AppraisalCouncil relate to this Id");
                notificationDTO.AppraisalCouncilId = newNotification.ObjecNotificationId;
                break;
            case "transaction":
                _ = await _unitOfWork.GetTransactionRepository().GetOneAsync(tr => tr.Id == newNotification.ObjecNotificationId, null, false)
                    ?? throw new NotFoundException("Create Notification Failed: Not found any Transaction relate to this Id");
                notificationDTO.TransactionId = newNotification.ObjecNotificationId;
                break;
            case "individualevaluation":
                _ = await _unitOfWork.GetIndividualEvaluationRepository().GetOneAsync(ie => ie.Id == newNotification.ObjecNotificationId, null, false)
                    ?? throw new NotFoundException("Create Notification Failed: Not found any IndividualEvaluation relate to this Id");
                notificationDTO.IndividualEvaluationId = newNotification.ObjecNotificationId;
                break;
            case "evaluationstage":
                _ = await _unitOfWork.GetEvaluationStageRepository().GetOneAsync(es => es.Id == newNotification.ObjecNotificationId, null, false)
                    ?? throw new NotFoundException("Create Notification Failed: Not found any EvaluationStage relate to this Id");
                notificationDTO.EvaluationStageId = newNotification.ObjecNotificationId;
                break;
            case "evaluation":
                _ = await _unitOfWork.GetEvaluationRepository().GetOneAsync(e => e.Id == newNotification.ObjecNotificationId, null, false)
                    ?? throw new NotFoundException("Create Notification Failed: Not found any Evaluation relate to this Id");
                notificationDTO.EvaluationId = newNotification.ObjecNotificationId;
                break;
            case "userrole":
                _ = await _unitOfWork.GetUserRoleRepository().GetOneAsync(ur => ur.Id == newNotification.ObjecNotificationId, null, false)
                    ?? throw new NotFoundException("Create Notification Failed: Not found any User relate to this Id");
                notificationDTO.UserRoleId = newNotification.ObjecNotificationId;
                break;
            case "document":
                _ = await _unitOfWork.GetDocumentRepository().GetOneAsync(d => d.Id == newNotification.ObjecNotificationId, null, false)
                    ?? throw new NotFoundException("Create Notification Failed: Not found any Document relate to this Id");
                notificationDTO.DocumentId = newNotification.ObjecNotificationId;
                break;
            case "membertask":
                _ = await _unitOfWork.GetMemberTaskRepository().GetOneAsync(mt => mt.Id == newNotification.ObjecNotificationId, null, false)
                    ?? throw new NotFoundException("Create Notification Failed: Not found any MemberTask relate to this Id");
                notificationDTO.MemberTaskId = newNotification.ObjecNotificationId;
                break;
            case "task":
                _ = await _unitOfWork.GetTaskRepository().GetOneAsync(ta => ta.Id == newNotification.ObjecNotificationId, null, false)
                    ?? throw new NotFoundException("Create Notification Failed: Not found any Task relate to this Id");
                notificationDTO.TaskId = newNotification.ObjecNotificationId;
                break;
            default: //"systemconfiguration"
                _ = await _unitOfWork.GetSystemConfigurationRepository().GetOneAsync(sc => sc.Id == newNotification.ObjecNotificationId, null, false)
                    ?? throw new NotFoundException("Create Notification Failed: Not found any SystemConfiguration relate to this Id");
                notificationDTO.SystemConfigurationId = newNotification.ObjecNotificationId;
                break;
        }

        // Save to database (existing code)
        await _unitOfWork.GetNotificationRepository().AddAsync(notificationDTO);
        var saveSuccess = await _unitOfWork.GetNotificationRepository().SaveChangeAsync();

        // Create real-time notification data
        var realtimeNotificationData = new
        {
            Id = notificationDTO.Id,
            Title = notificationDTO.Title,
            Type = notificationDTO.Type,
            CreateDate = notificationDTO.CreateDate,
            ObjectId = newNotification.ObjecNotificationId,
            IsGlobal = notificationDTO.IsGlobalSend
        };

        // Send database notifications and real-time notifications
        if (notificationDTO.IsGlobalSend)
        {
            // Save to database for all users
            await NotificateToUser(null, notificationDTO.Id);

            // Send real-time global notification
            await SendRealTimeGlobalNotification(realtimeNotificationData);
        }
        else
        {
            // Save to database for specific users
            await NotificateToUser(newNotification.ListAccountId, notificationDTO.Id);

            // Send real-time notification to specific users
            if (newNotification.ListAccountId?.Any() == true)
            {
                await SendRealTimeNotificationToUsers(newNotification.ListAccountId, realtimeNotificationData);
            }
        }

        return (saveSuccess, notificationDTO.Id);
    }

    public async Task<bool> NotificateToUser(List<Guid>? ListAccountId, Guid notificationId)
    {
        var existNoti = await _unitOfWork.GetNotificationRepository().GetOneAsync(n => n.Id == notificationId, null, false)
                    ?? throw new NotFoundException("Not found any Notification match the Id");

        // Handle if list null -> get all user
        if (ListAccountId is null || !ListAccountId.Any())
            ListAccountId = await _unitOfWork.GetAccountNotificationRepository().ListIdAllAccount();

        // Transfer list AccountId into list AccountNotification
        var accountNotificationDTOs = ListAccountId!.Select(accountId =>
        new AccountNotification
        {
            AccountId = accountId,
            NotificationId = notificationId,
            CreateDate = existNoti.CreateDate
        });

        await _unitOfWork.GetAccountNotificationRepository().AddRangeAsync(accountNotificationDTOs);
        var result = await _unitOfWork.GetAccountNotificationRepository().SaveChangeAsync();

        // NEW: Send real-time notifications after saving to database
        if (result && ListAccountId.Any())
        {
            var realtimeNotificationData = new
            {
                Id = existNoti.Id,
                Title = existNoti.Title,
                Type = existNoti.Type,
                CreateDate = existNoti.CreateDate,
                IsGlobal = false
            };

            await SendRealTimeNotificationToUsers(ListAccountId, realtimeNotificationData);
        }

        return result;
    }

    // NEW: Real-time notification methods
    public async System.Threading.Tasks.Task SendRealTimeNotificationToUsers(List<Guid> userIds, object notification)
    {
        var groups = userIds.Select(id => $"user_{id}").ToList();

        foreach (var group in groups)
        {
            await _hubContext.Clients.Group(group).SendAsync("ReceiveNotification", notification);
        }
    }

    public async System.Threading.Tasks.Task SendRealTimeGlobalNotification(object notification)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", notification);
    }

    public async Task<PagingResult<RS_Notification>?> ListRequestNotification(Q_RequestNoti queryInput)
    {
        //Re-assign value if it smaller than 1
        queryInput.PageIndex = queryInput.PageIndex < 1 ? 1 : queryInput.PageIndex;
        queryInput.PageSize = queryInput.PageSize < 1 ? 1 : queryInput.PageSize;

        var dataResult = await _unitOfWork.GetNotificationRepository().ListPaging
            (queryInput.KeyWord, queryInput.Type, queryInput.Status, queryInput.SortBy,
            queryInput.IsRequest, queryInput.IsGlobalSend,
            queryInput.FromDate, queryInput.ToDate, queryInput.PageIndex, queryInput.PageSize,
            queryInput.ProjectId, queryInput.AppraisalCouncilId, queryInput.EvaluationId,
            queryInput.EvaluationStageId, queryInput.IndividualEvaluationId, queryInput.DocumentId,
            queryInput.SignatureId, queryInput.TaskId, queryInput.MemberTaskId, queryInput.TransactionId,
            queryInput.SystemConfigurationId, queryInput.UserRoleId);

        // Checking Result
        if (dataResult.listNotification is null || dataResult.listNotification.Count == 0)
            throw new NotFoundException("Not Found Any Notification!");

        return new PagingResult<RS_Notification>
        {
            PageIndex = queryInput.PageIndex,
            PageSize = queryInput.PageSize,
            TotalCount = dataResult.totalFound,
            DataList = dataResult.listNotification.Adapt<List<RS_Notification>>()
        };
    }

    public async Task<PagingResult<RS_AccountNotification>?> ListNotificationOfUser(Q_AccountNotification queryInput)
    {
        List<NotificationWithReadStatus>? listNoti = null;
        int totalCount = 0;

        //Re-assign value if it smaller than 1
        queryInput.PageIndex = queryInput.PageIndex < 1 ? 1 : queryInput.PageIndex;
        queryInput.PageSize = queryInput.PageSize < 1 ? 1 : queryInput.PageSize;

        Guid accId = Guid.Empty;
        if (string.IsNullOrWhiteSpace(queryInput.Email))
        {
            _ = Guid.TryParse(_userContextService.GetCurrentUserId(), out accId);

            (listNoti, totalCount) = await _unitOfWork.GetAccountNotificationRepository().ListAccountNotification
                (accId, null, queryInput.KeyWord, queryInput.FromDate, queryInput.ToDate,
                queryInput.IsRead, queryInput.Type, queryInput.Status, queryInput.PageIndex, queryInput.PageSize);
        }
        else
        {
            (listNoti, totalCount) = await _unitOfWork.GetAccountNotificationRepository().ListAccountNotification
                (null, queryInput.Email, queryInput.KeyWord, queryInput.FromDate, queryInput.ToDate,
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

    public async Task<bool> SendNotificationMail(RQ_NotificationEmail notiEmail)
    {
        var notification = notiEmail.Adapt<DTO_NotificationEmail>();
        var body = await _emailService.RenderNotificationEmail(notification);
        if (string.IsNullOrWhiteSpace(body)) throw new BadRequestException("Failed to render email body!");

        var email = notiEmail.Adapt<EmailDTO>();
        email.Body = body;
        return await _emailService.SendEmailAsync(email);
    }
}