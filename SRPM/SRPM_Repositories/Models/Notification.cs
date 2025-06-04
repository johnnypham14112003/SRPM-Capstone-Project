using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SRPM_Repositories.Models;
public class Notification
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(255)]
    public string Title { get; set; }= null!;

    [Required, MaxLength(30)]
    public string Type { get; set; } = null!;

    [Required]
    public bool IsGlobalSend { get; set; } = false;

    [Required]
    public DateTime CreateDate { get; set; } = DateTime.Now;

    [Required, MaxLength(30)]
    public string Status { get; set; } = "created";

    // ========================[ Navigation properties ]========================
    public virtual ICollection<AccountNotification>? AccountNotifications { get; set; }
    public Guid? TransactionId { get; set; }
    [ForeignKey(nameof(TransactionId))]
    public virtual Transaction? Transaction { get; set; }

    public Guid? IndividualEvaluationId { get; set; }
    [ForeignKey(nameof(IndividualEvaluationId))]
    public virtual IndividualEvaluation? IndividualEvaluation { get; set; }

    public Guid? EvaluationStageId { get; set; }

    [ForeignKey(nameof(EvaluationStageId))]
    public virtual EvaluationStage? EvaluationStage { get; set; }

    public Guid? EvaluationId { get; set; }
    [ForeignKey(nameof(EvaluationId))]
    public virtual Evaluation? Evaluation { get; set; }

    public Guid? GroupUserId { get; set; }
    [ForeignKey(nameof(GroupUserId))]
    public virtual UserRole? GroupUser { get; set; }

    public Guid? DocumentId { get; set; }
    [ForeignKey(nameof(DocumentId))]
    public virtual Document? Document { get; set; }

    public Guid? MemberTaskId { get; set; }
    [ForeignKey(nameof(MemberTaskId))]
    public virtual MemberTask? MemberTask { get; set; }

    public Guid? TaskId { get; set; }
    [ForeignKey(nameof(TaskId))]
    public virtual Task? Task { get; set; }

    public Guid? SystemConfigurationId { get; set; }
    [ForeignKey(nameof(SystemConfigurationId))]
    public virtual SystemConfiguration? SystemConfiguration { get; set; }
}