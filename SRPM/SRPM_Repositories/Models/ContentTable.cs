using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;

public class ContentTable
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    [Required] public int ColumnIndex { get; set; } = 1;
    [Required] public int RowIndex { get; set; } = 1;

    [MaxLength(255)] public string? ColumnTitle { get; set; }
    [MaxLength(30)] public string? ColumnTitleAlign { get; set; }
    [MaxLength(50)] public string? ColumnTitleStyle { get; set; }

    [MaxLength(255)] public string? SubColumnTitle { get; set; }
    [MaxLength(30)] public string? SubColumnTitleAlign { get; set; }
    [MaxLength(50)] public string? SubColumnTitleStyle { get; set; }

    public string? CellContent { get; set; }
    [MaxLength(30)] public string? CellContentAlign { get; set; }
    [MaxLength(50)] public string? CellContentStyle { get; set; }

    [Required] public DateTime UpdatedAt { get; set; } = DateTime.Now;
    [Required] public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Foreign keys
    [Required] public Guid FieldContentId { get; set; }

    // Navigation properties
    public virtual FieldContent FieldContent { get; set; } = null!;
}