namespace SRPM_Services.BusinessModels.ResponseModels;

public class RS_Notification
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;
    public string Type { get; set; } = null!;
    public bool IsGlobalSend { get; set; } = false;
    public DateTime CreateDate { get; set; } = DateTime.Now;
    public string Status { get; set; } = null!;

    // Foreign keys
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
