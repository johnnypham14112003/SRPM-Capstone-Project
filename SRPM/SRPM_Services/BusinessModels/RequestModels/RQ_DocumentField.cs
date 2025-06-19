using SRPM_Repositories.Models;
using System.ComponentModel.DataAnnotations;

namespace SRPM_Services.BusinessModels.RequestModels;

public class RQ_DocumentField
{
    public Guid? Id { get; set; }

    public int IndexInDoc { get; set; } = 1;
    public string? Chapter { get; set; }
    [MaxLength(30)] public string? ChapterAlign { get; set; }
    [MaxLength(50)] public string? ChapterStyle { get; set; }

    public bool IsBlankLine { get; set; } = false;
    public bool HaveHeader { get; set; } = false;
    public string? Header { get; set; }
    [MaxLength(30)] public string? HeaderAlign { get; set; }
    [MaxLength(50)] public string? HeaderStyle { get; set; }

    public string? Title { get; set; }
    [MaxLength(30)] public string? TitleAlign { get; set; }
    [MaxLength(50)] public string? TitleStyle { get; set; }

    public string? Subtitle { get; set; }
    [MaxLength(30)] public string? SubTitleAlign { get; set; }
    [MaxLength(50)] public string? SubTitleStyle { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Foreign keys
    public Guid DocumentId { get; set; }
    public virtual IEnumerable<RQ_FieldContent>? FieldContents { get; set; }
}