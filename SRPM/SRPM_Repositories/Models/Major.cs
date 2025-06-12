using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;

public class Major
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(100)] public string Name { get; set; } = null!;

    // Foreign keys
    [Required] public Guid FieldId { get; set; }

    // Navigation properties
    public virtual Field Field { get; set; } = null!;
    public virtual ICollection<Account>? Accounts { get; set; }
    public virtual ICollection<ProjectMajor>? ProjectMajors { get; set; }
}