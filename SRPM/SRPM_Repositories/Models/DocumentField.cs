using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;

public class DocumentField
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string? Chapter { get; set; }

    [MaxLength(30)]
    public string? ChapterAlign { get; set; }

    [MaxLength(50)]
    public string? ChapterStyle { get; set; }

    // Title
    public string? Title { get; set; }

    [MaxLength(30)]
    public string? TitleAlign { get; set; }

    [MaxLength(50)]
    public string? TitleStyle { get; set; }

    public string? Subtitle { get; set; }

    [MaxLength(30)]
    public string? SubTitleAlign { get; set; }

    [MaxLength(50)]
    public string? SubTitleStyle { get; set; }

    [Required]
    public int IndexInDoc { get; set; } = 1;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public Guid DocumentId { get; set; }

    // Navigation properties
    public virtual Document Document { get; set; } = null!;
    public virtual ICollection<FieldContent> FieldContents { get; set; } = new List<FieldContent>();
}
