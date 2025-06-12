using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;
public class ProjectTag
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(150)]
    public string Name { get; set; }

    [Required]
    public Guid ProjectId { get; set; }

    // Navigation properties
    public virtual Project Project { get; set; } = null!;
}