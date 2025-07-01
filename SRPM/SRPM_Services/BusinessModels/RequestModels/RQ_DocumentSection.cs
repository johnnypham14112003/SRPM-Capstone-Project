using System.ComponentModel.DataAnnotations;

namespace SRPM_Services.BusinessModels.RequestModels;

public class RQ_DocumentSection
{
    public Guid? Id { get; set; }

    [MaxLength(255)] public string? Title { get; set; }
    public int SectionOrder { get; set; } = 1;
    public bool IsTable { get; set; } = false;//true -< SectionContent   | false -< TableStructure
    public bool IsSpacing { get; set; } = false;//space between section

    // Foreign keys
    public Guid DocumentId { get; set; }

    // Navigation properties
    public virtual ICollection<RQ_SectionContent>? SectionContents { get; set; }
    public virtual ICollection<RQ_TableStructure>? TableStructures { get; set; }
}