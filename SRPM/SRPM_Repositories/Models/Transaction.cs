using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;

public class Transaction
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    public string? EvidenceImage { get; set; }
    [Required, MaxLength(30)] public string Code { get; set; } = null!;
    [Required, MaxLength(255)] public string Title { get; set; } = null!;
    [Required, MaxLength(30)] public string Type { get; set; } = null!;

    // Sender
    [MaxLength(30)] public string? SenderAccount { get; set; }
    [MaxLength(50)] public string? SenderName { get; set; }
    [MaxLength(50)] public string? SenderBankName { get; set; }

    // Receiver (Person who request this)
    [MaxLength(30)] public string ReceiverAccount { get; set; } = null!;
    [MaxLength(50)] public string ReceiverName { get; set; } = null!;
    [MaxLength(50)] public string ReceiverBankName { get; set; } = null!;

    [MaxLength(50)] public string? TransferContent { get; set; }
    [Required] public DateTime RequestDate { get; set; } = DateTime.Now;
    public DateTime? HandleDate { get; set; }
    [Required] public decimal FeeCost { get; set; } = 0m;// decimal suffix
    [Required] public decimal TotalMoney { get; set; } = 0m;// decimal suffix
    [Required, MaxLength(30)] public string PayMethod { get; set; } = "transfer";
    [Required, MaxLength(30)] public string Status { get; set; } = "created";

    // Foreign keys
    [Required] public Guid RequestPersonId { get; set; }
    [Required] public Guid? HandlePersonId { get; set; }

    public Guid? ProjectId { get; set; }
    public Guid? EvaluationStageId { get; set; }

    // Navigation properties
    public virtual UserRole RequestPerson { get; set; } = null!;
    public virtual UserRole? HandlePerson { get; set; }
    public virtual Project? Project { get; set; }
    public virtual EvaluationStage? EvaluationStage { get; set; }
    public virtual ICollection<Document>? Documents { get; set; }
    public virtual ICollection<Notification>? Notifications { get; set; }
}