using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;

public class FieldContent
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    [Required] public int IndexInField { get; set; } = 1;
    public string? Title { get; set; }
    [MaxLength(30)] public string? TitleAlign { get; set; }
    [MaxLength(50)] public string? TitleStyle { get; set; }

    public string? Content { get; set; }
    [MaxLength(30)] public string? ContentAlign { get; set; }
    [MaxLength(50)] public string? ContentStyle { get; set; }

    [Required] public DateTime UpdatedAt { get; set; } = DateTime.Now;
    [Required] public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Foreign keys
    [Required] public Guid DocumentFieldId { get; set; }

    // Navigation properties
    [Required] public virtual DocumentField DocumentField { get; set; } = null!;
    public virtual ICollection<ContentTable>? ContentTables { get; set; }
}