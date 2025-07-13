namespace SRPM_Services.BusinessModels.ResponseModels;

public class RS_Signature
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string? URL { get; set; }
    public DateTime SignedDate { get; set; } = DateTime.Now;

    // Foreign keys
    public Guid SignerId { get; set; }
    public Guid DocumentId { get; set; }
}