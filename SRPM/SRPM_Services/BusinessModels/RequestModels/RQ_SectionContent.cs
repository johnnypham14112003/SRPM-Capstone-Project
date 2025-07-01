namespace SRPM_Services.BusinessModels.RequestModels;

public class RQ_SectionContent
{
    public Guid? Id { get; set; }

    public int ContentOrder { get; set; } = 1;
    public bool IsEditable { get; set; } = false;
    public string? ContentHTML { get; set; }

    // Foreign keys
    public Guid DocumentSectionId { get; set; }
}