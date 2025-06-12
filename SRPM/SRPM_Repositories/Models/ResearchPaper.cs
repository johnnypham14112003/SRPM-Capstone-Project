using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;

public class ResearchPaper
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    [Required] public string RefLink { get; set; } = null!;
    [Required, MaxLength(250)] public string Title { get; set; } = null!;
    [Required] public string Content { get; set; } = null!;
    [MaxLength(100)] public string? ProviderName { get; set; }
    [Required] public DateTime AddedDate { get; set; } = DateTime.Now;

    // Foreign keys
    [Required] public Guid ProjectId { get; set; }
    [Required] public Guid PrincipalInvestigatorId { get; set; }

    // Navigation properties
    public virtual Project Project { get; set; } = null!;
    public virtual UserRole PrincipalInvestigator { get; set; } = null!;
}