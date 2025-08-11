using SRPM_Repositories.Models;

namespace SRPM_Services.BusinessModels.RequestModels.Query;

public class Q_RequestNoti
{
    public string? KeyWord { get; set; }
    public string? Type { get; set; }
    public string? Status { get; set; }
    public bool IsRequest { get; set; } = true;
    public bool IsGlobalSend { get; set; } = false;
    public byte SortBy { get; set; } = 0;
    public bool Descending { get; set; } = true;
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 5;

    public virtual Guid? ProjectId { get; set; }
    public virtual Guid? AppraisalCouncilId { get; set; }
    public virtual Guid? EvaluationId { get; set; }
    public virtual Guid? EvaluationStageId { get; set; }
    public virtual Guid? IndividualEvaluationId { get; set; }
    public virtual Guid? DocumentId { get; set; }
    public virtual Guid? SignatureId { get; set; }
    public virtual Guid? TaskId { get; set; }
    public virtual Guid? MemberTaskId { get; set; }
    public virtual Guid? TransactionId { get; set; }
    public virtual Guid? SystemConfigurationId { get; set; }
    public virtual Guid? UserRoleId { get; set; }
}