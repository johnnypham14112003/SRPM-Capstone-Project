using SRPM_Repositories.Models;

namespace SRPM_Repositories.Repositories.Interfaces;

public interface INotificationRepository : IGenericRepository<Notification>
{
    Task<(List<Notification>? listNotification, int totalFound)> ListPaging
        (string? keyWord, string? type, string? status, byte sortBy, bool isRequest, bool isGlobalSend,
        DateTime? fromDate, DateTime? toDate, int pageIndex, int pageSize,
        Guid? projectId, Guid? appraisalCouncilId, Guid? evaluationId, Guid? EvaluationStageId,
        Guid? individualEvaluationId, Guid? documentId, Guid? signatureId, Guid? taskId,
        Guid? memberTaskId, Guid? transactionId, Guid? systemConfigurationId, Guid? userRoleId);
}
