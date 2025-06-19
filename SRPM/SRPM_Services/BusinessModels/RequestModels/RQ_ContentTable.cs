using System.ComponentModel.DataAnnotations;

namespace SRPM_Services.BusinessModels.RequestModels;

public class RQ_ContentTable
{
    public Guid? Id { get; set; }

    public int ColumnIndex { get; set; } = 1;
    public int RowIndex { get; set; } = 1;

    [MaxLength(255)] public string? ColumnTitle { get; set; }
    [MaxLength(30)] public string? ColumnTitleAlign { get; set; }
    [MaxLength(50)] public string? ColumnTitleStyle { get; set; }

    [MaxLength(255)] public string? SubColumnTitle { get; set; }
    [MaxLength(30)] public string? SubColumnTitleAlign { get; set; }
    [MaxLength(50)] public string? SubColumnTitleStyle { get; set; }

    public string? CellContent { get; set; }
    [MaxLength(30)] public string? CellContentAlign { get; set; }
    [MaxLength(50)] public string? CellContentStyle { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Foreign keys
    public Guid FieldContentId { get; set; }
}