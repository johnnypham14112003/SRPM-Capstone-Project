using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;

public class Notification
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(255)] public string Title { get; set; } = null!;
    [Required, MaxLength(30)] public string Type { get; set; } = null!;
    [Required] public bool IsGlobalSend { get; set; } = false;
    [Required] public DateTime CreateDate { get; set; } = DateTime.Now;
    [Required, MaxLength(30)] public string Status { get; set; } = "created";

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

    // Navigation properties
    public virtual ICollection<AccountNotification>? AccountNotifications { get; set; }
    public virtual Project? Project { get; set; }
    public virtual AppraisalCouncil? AppraisalCouncil { get; set; }
    public virtual Evaluation? Evaluation { get; set; }
    public virtual EvaluationStage? EvaluationStage { get; set; }
    public virtual IndividualEvaluation? IndividualEvaluation { get; set; }
    public virtual Document? Document { get; set; }
    public virtual Signature? Signature { get; set; }
    public virtual Task? Task { get; set; }
    public virtual MemberTask? MemberTask { get; set; }
    public virtual Transaction? Transaction { get; set; }
    public virtual SystemConfiguration? SystemConfiguration { get; set; }
    public virtual UserRole? UserRole { get; set; }//For group notification
}