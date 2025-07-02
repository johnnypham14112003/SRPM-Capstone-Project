using System.ComponentModel.DataAnnotations;

namespace SRPM_Services.BusinessModels.ResponseModels;

public class RS_FieldContent
{
    public Guid? Id { get; set; }

    public int IndexInField { get; set; } = 1;
    public string? Title { get; set; }
    [MaxLength(30)] public string? TitleAlign { get; set; }
    [MaxLength(50)] public string? TitleStyle { get; set; }

    public string? Content { get; set; }
    [MaxLength(30)] public string? ContentAlign { get; set; }
    [MaxLength(50)] public string? ContentStyle { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Foreign keys
    public Guid DocumentFieldId { get; set; }
    public virtual IEnumerable<RS_ContentTable>? ContentTables { get; set; }
}