using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SRPM_Repositories.Models;
public class Transaction
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(30)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(30)]
    public string Type { get; set; } = string.Empty;

    // Sender
    [MaxLength(30)]
    public string? SenderAccount { get; set; }

    [MaxLength(50)]
    public string? SenderName { get; set; }

    [MaxLength(50)]
    public string? SenderBankName { get; set; }

    // Receiver (Person who request this)
    [MaxLength(30)]
    public string ReceiverAccount { get; set; } = null!;

    [MaxLength(50)]
    public string ReceiverName { get; set; } = null!;

    [MaxLength(50)]
    public string ReceiverBankName { get; set; } = null!;

    [MaxLength(50)]
    public string? TransferContent { get; set; }

    [Required]
    public DateTime RequestDate { get; set; } = DateTime.UtcNow;

    public DateTime? HandleDate { get; set; }

    [Required]
    [Column(TypeName = "money")]
    public decimal FeeCost { get; set; } = 0;

    [Required]
    [Column(TypeName = "money")]
    public decimal TotalMoney { get; set; } = 0;

    [Required]
    [MaxLength(30)]
    public string PayMethod { get; set; } = "transfer";

    [Required]
    [MaxLength(30)]
    public string Status { get; set; } = "complete";

    [Required]
    public Guid RequestPersonId { get; set; }

    [Required]
    public Guid HandlePersonId { get; set; }

    public Guid? ProjectId { get; set; }
    public Guid? EvaluationStageId { get; set; }
    public Guid? FundRequestDocId { get; set; }

    // Navigation properties
    [ForeignKey(nameof(RequestPersonId))]
    public virtual UserRole RequestPerson { get; set; } = null!;
    [ForeignKey(nameof(HandlePersonId))]
    public virtual UserRole HandlePerson { get; set; } = null!;
    public virtual Project? Project { get; set; }
    [ForeignKey(nameof(EvaluationStageId))]
    public virtual EvaluationStage? EvaluationStage { get; set; }
    [ForeignKey(nameof(FundRequestDocId))]
    public virtual Document? FundRequestDoc { get; set; }
    public virtual ICollection<Notification>? Notifications { get; set; }
}