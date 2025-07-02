using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;

public class DocumentField
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    [Required] public int IndexInDoc { get; set; } = 1;
    public string? Chapter { get; set; }
    [MaxLength(30)] public string? ChapterAlign { get; set; }
    [MaxLength(50)] public string? ChapterStyle { get; set; }

    [Required] public bool IsBlankLine { get; set; } = false;
    [Required] public bool HaveHeader { get; set; } = false;
    public string? Header { get; set; }
    [MaxLength(30)] public string? HeaderAlign { get; set; }
    [MaxLength(50)] public string? HeaderStyle { get; set; }

    public string? Title { get; set; }
    [MaxLength(30)] public string? TitleAlign { get; set; }
    [MaxLength(50)] public string? TitleStyle { get; set; }

    public string? Subtitle { get; set; }
    [MaxLength(30)] public string? SubTitleAlign { get; set; }
    [MaxLength(50)] public string? SubTitleStyle { get; set; }

    [Required] public DateTime UpdatedAt { get; set; } = DateTime.Now;
    [Required] public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Foreign keys
    [Required] public Guid DocumentId { get; set; }

    // Navigation properties
    public virtual Document Document { get; set; } = null!;
    public virtual ICollection<FieldContent>? FieldContents { get; set; }
}
