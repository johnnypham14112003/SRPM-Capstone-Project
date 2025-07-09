using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;

public class DocumentSection
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(255)] public string? Title { get; set; }
    [Required] public int SectionOrder { get; set; } = 1;
    [Required] public bool IsTable { get; set; } = false;//true -< SectionContent   | false -< TableStructure
    [Required] public bool IsSpacing { get; set; } = false;//space between section

    // Foreign keys
    [Required] public Guid DocumentId { get; set; }

    // Navigation properties
    public virtual Document Document { get; set; } = null!;
    public virtual ICollection<SectionContent>? SectionContents { get; set; }
    public virtual ICollection<TableStructure>? TableStructures { get; set; }
}
