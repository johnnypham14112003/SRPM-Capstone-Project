using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;

public class ProjectResult
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(250)] public string Name { get; set; } = null!;
    public string? Url { get; set; }
    [Required] public DateTime AddedDate { get; set; } = DateTime.Now;

    // Foreign keys
    [Required] public Guid ProjectId { get; set; }

    // Navigation properties
    public virtual Project Project { get; set; } = null!;
    public virtual ICollection<ResultPublish>? ResultPublishs { get; set; }
}