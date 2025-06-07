using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;
public class Document
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public bool? HaveHeader { get; set; }
    public string? Header { get; set; }

    [MaxLength(30)]
    public string? HeaderAlign { get; set; }

    [MaxLength(50)]
    public string? HeaderStyle { get; set; }

    public string? SubHeader { get; set; }

    [MaxLength(30)]
    public string? SubHeaderAlign { get; set; }

    [MaxLength(50)]
    public string? SubHeaderStyle { get; set; }

    // Main
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
    [MaxLength(30)]
    public string Type { get; set; } = "system";

    public bool? IsSigned { get; set; }
    public DateTime? DateInDoc { get; set; }

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime UploadAt { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(30)]
    public string Status { get; set; } = "draft";

    [Required]
    public Guid Uploader { get; set; }

    public Guid? ProjectId { get; set; }

    // Navigation properties
    public virtual UserRole UploaderUser { get; set; } = null!;
    public virtual Project? Project { get; set; }
    public virtual ICollection<DocumentField> DocumentFields { get; set; } = new List<DocumentField>();
    public virtual Evaluation? EvaluationsWithFinalDoc { get; set; }
    public virtual Transaction? FundRequestTransaction { get; set; }
    public virtual ICollection<Notification>? Notifications { get; set; }
}