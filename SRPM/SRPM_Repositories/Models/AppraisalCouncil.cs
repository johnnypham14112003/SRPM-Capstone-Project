using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;

public class AppraisalCouncil
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(30)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    [Required, MaxLength(30)]
    public string Status { get; set; } = "draft";

    // Navigation properties
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public virtual ICollection<Evaluation> Evaluations { get; set; } = new List<Evaluation>();
}
