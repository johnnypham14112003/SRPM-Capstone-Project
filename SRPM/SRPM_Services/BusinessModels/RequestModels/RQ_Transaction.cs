using System.ComponentModel.DataAnnotations;

namespace SRPM_Services.BusinessModels.RequestModels;

public class RQ_Transaction
{
    public Guid? Id { get; set; }

    public string? EvidenceImage { get; set; }
    public string Code { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Type { get; set; } = null!;

    public string? SenderAccount { get; set; }
    public string? SenderName { get; set; }
    public string? SenderBankName { get; set; }

    public string ReceiverAccount { get; set; } = null!;
    public string ReceiverName { get; set; } = null!;
    public string ReceiverBankName { get; set; } = null!;

    [MaxLength(50)] public string? TransferContent { get; set; }
    [Required] public DateTime RequestDate { get; set; } = DateTime.Now;
    public DateTime? HandleDate { get; set; }

    public decimal FeeCost { get; set; }
    public decimal TotalMoney { get; set; }
    public string PayMethod { get; set; } = "transfer";
    public string Status { get; set; } = "created";

    public Guid RequestPersonId { get; set; }
    public Guid? HandlePersonId { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid? EvaluationStageId { get; set; }
}