using Microsoft.EntityFrameworkCore;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;

namespace SRPM_Repositories.Repositories.Implements;

public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
{
    private readonly SRPMDbContext _context;
    public NotificationRepository(SRPMDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<(List<Notification>? listNotification, int totalFound)> ListPaging
        (string? keyWord, string? type, string? status, byte sortBy, bool isRequest, bool isGlobalSend,
        DateTime? fromDate, DateTime? toDate, int pageIndex, int pageSize,
        Guid? projectId, Guid? appraisalCouncilId, Guid? evaluationId, Guid? EvaluationStageId,
        Guid? individualEvaluationId, Guid? documentId, Guid? signatureId, Guid? taskId,
        Guid? memberTaskId, Guid? transactionId, Guid? systemConfigurationId, Guid? userRoleId)
    {
        var query = _context.Notification
            .AsNoTracking()
            .AsQueryable();

        // ===========================[ Apply Search ]===========================
        //keyword Filter
        if (!string.IsNullOrWhiteSpace(keyWord))
            query = query.Where(n => n.Title.ToLower().Contains(keyWord.ToLower()));

        //type Filter
        if (!string.IsNullOrWhiteSpace(type))
            query = query.Where(n => n.Type.ToLower().Contains(type.ToLower()));

        //isRequest Filter
        if (isRequest)
        {//get contain these status
            query = query.Where(n =>
            n.Status.ToLower().Equals("pending") ||
            n.Status.ToLower().Equals("rejected") ||
            n.Status.ToLower().Equals("approved"));
        }
        else
        {//get not contain these status
            query = query.Where(n =>
            !n.Status.ToLower().Equals("pending") ||
            !n.Status.ToLower().Equals("rejected") ||
            !n.Status.ToLower().Equals("approved"));
        }

        //Status Filter
        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(n => n.Status.ToLower().Equals(status.ToLower()));
            if (!status.ToLower().Equals("deleted"))
                query = query.Where(n => !n.Status.ToLower().Equals("deleted"));
        }

        //isGlobal Filter
        query = query.Where(n => n.IsGlobalSend == isGlobalSend);

        // Date Range Filter
        if (fromDate.HasValue) query = query.Where(n => n.CreateDate >= fromDate.Value);

        if (toDate.HasValue) query = query.Where(n => n.CreateDate <= toDate.Value);

        //======================================================
        //By ProjectId
        if (projectId.HasValue)
            query = query.Where(n => n.ProjectId == projectId.Value);

        //By appraisalId
        if (appraisalCouncilId.HasValue)
            query = query.Where(n => n.AppraisalCouncilId == appraisalCouncilId.Value);

        //By evaluationId
        if (evaluationId.HasValue)
            query = query.Where(n => n.EvaluationId == evaluationId.Value);

        //By EvaluationStageId
        if (EvaluationStageId.HasValue)
            query = query.Where(n => n.EvaluationStageId == EvaluationStageId.Value);

        //By individualEvaluationId
        if (individualEvaluationId.HasValue)
            query = query.Where(n => n.IndividualEvaluationId == individualEvaluationId.Value);

        //By documentId
        if (documentId.HasValue)
            query = query.Where(n => n.DocumentId == documentId.Value);

        //By signatureId
        if (signatureId.HasValue)
            query = query.Where(n => n.SignatureId == signatureId.Value);

        //By taskId
        if (taskId.HasValue) query = query.Where(n => n.TaskId == taskId.Value);

        //By memberTaskId
        if (memberTaskId.HasValue) query = query.Where(n => n.MemberTaskId == memberTaskId.Value);

        //By transactionId
        if (transactionId.HasValue) query = query.Where(n => n.TransactionId == transactionId.Value);

        //By systemConfigurationId
        if (systemConfigurationId.HasValue) query = query.Where(n => n.SystemConfigurationId == systemConfigurationId.Value);

        //By userRoleId
        if (userRoleId.HasValue) query = query.Where(n => n.UserRoleId == userRoleId.Value);

        // Sort By
        query = sortBy switch
        {
            //Title (Z-A)
            1 => query.OrderByDescending(n => n.Title),
            // Type (A-Z)
            2 => query.OrderBy(n => n.Type),
            // Type (Z-A)
            3 => query.OrderByDescending(n => n.Type),
            // Status (A-Z)
            4 => query.OrderBy(n => n.Status),
            // Status (Z-A)
            5 => query.OrderByDescending(n => n.Status),
            // CreateTime (old-new)
            6 => query.OrderBy(n => n.CreateDate),
            // CreateTime (new-old)
            7 => query.OrderByDescending(n => n.CreateDate),
            // ProjectId
            8 => query.OrderBy(n => n.ProjectId),
            // AppraisalCouncilId
            9 => query.OrderBy(n => n.AppraisalCouncilId),
            // AppraisalCouncilId
            10 => query.OrderBy(n => n.EvaluationId),
            // AppraisalCouncilId
            11 => query.OrderBy(n => n.EvaluationStageId),
            // AppraisalCouncilId
            12 => query.OrderBy(n => n.IndividualEvaluationId),
            // AppraisalCouncilId
            13 => query.OrderBy(n => n.DocumentId),
            // AppraisalCouncilId
            14 => query.OrderBy(n => n.SignatureId),
            // AppraisalCouncilId
            15 => query.OrderBy(n => n.TaskId),
            // AppraisalCouncilId
            16 => query.OrderBy(n => n.MemberTaskId),
            // AppraisalCouncilId
            17 => query.OrderBy(n => n.TransactionId),
            // AppraisalCouncilId
            18 => query.OrderBy(n => n.SystemConfigurationId),
            // AppraisalCouncilId
            19 => query.OrderBy(n => n.UserRoleId),
            //Title (A-Z)
            _ => query.OrderBy(n => n.Title)
        };

        // Sum notification of a user
        int sumEvaluation = await query.CountAsync();

        // ===========================[ Apply paging ]===========================
        var pagedList = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (pagedList, sumEvaluation);
    }
}
