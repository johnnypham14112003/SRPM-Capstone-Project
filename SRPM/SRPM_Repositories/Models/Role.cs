using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;

public class Role
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(50)] public string Name { get; set; } = "ResearchMember";
    [Required] public bool IsGroupRole { get; set; } = false;
    [Required, MaxLength(30)] public string Status { get; set; } = "created";

    // Navigation properties
    public virtual ICollection<UserRole>? UserRoles { get; set; }
}