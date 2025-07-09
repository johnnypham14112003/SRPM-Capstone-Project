using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;

public class SectionContent
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    [Required] public int ContentOrder { get; set; } = 1;
    public bool IsEditable { get; set; } = false;
    public string? ContentHTML { get; set; }

    // Foreign keys
    [Required] public Guid DocumentSectionId { get; set; }

    // Navigation properties
    [Required] public virtual DocumentSection DocumentSection { get; set; } = null!;
}