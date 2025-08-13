using System.ComponentModel.DataAnnotations;

namespace SRPM_Services.BusinessModels.ResponseModels;

public class RS_Signature
{
    public Guid Id { get; set; }
    public string? SignerName { get; set; }
    public string? SignatureHashPreview { get; set; }
    public DateTime SignedDate { get; set; } = DateTime.Now;

    // Foreign keys
    public Guid SignerId { get; set; }
    public Guid DocumentId { get; set; }
}