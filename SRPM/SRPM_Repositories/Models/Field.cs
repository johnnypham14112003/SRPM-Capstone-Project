using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;

public class Field
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(100)] public string Name { get; set; } = null!;

    // Navigation properties
    public virtual ICollection<Major>? Majors { get; set; }
}