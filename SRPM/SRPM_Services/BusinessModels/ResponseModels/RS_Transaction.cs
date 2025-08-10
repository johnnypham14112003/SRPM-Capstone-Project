using SRPM_Repositories.Models;
using System.ComponentModel.DataAnnotations;

namespace SRPM_Services.BusinessModels.ResponseModels;

public class RS_Transaction
{
    public Guid Id { get; set; }

    public string? EvidenceImage { get; set; }
    public string Code { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Type { get; set; } = null!;

    // Sender
    public string? SenderAccount { get; set; }
    public string? SenderName { get; set; }
    public string? SenderBankName { get; set; }

    // Receiver (Person who request this)
    public string ReceiverAccount { get; set; } = null!;
    public string ReceiverName { get; set; } = null!;
    public string ReceiverBankName { get; set; } = null!;

    public string? TransferContent { get; set; }
    public DateTime RequestDate { get; set; } = DateTime.Now;
    public DateTime? HandleDate { get; set; }
    public decimal FeeCost { get; set; } = 0m;// decimal suffix
    public decimal TotalMoney { get; set; } = 0m;// decimal suffix
    public string PayMethod { get; set; } = "transfer";
    public string Status { get; set; } = "created";

    // Foreign keys
    public Guid RequestPersonId { get; set; }
    public Guid? HandlePersonId { get; set; }

    public Guid? ProjectId { get; set; }
    public Guid? EvaluationStageId { get; set; }
    public ICollection<RS_Document>? Documents { get; set; }
}