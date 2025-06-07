using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;
public class ResearchPaper
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string RefLink { get; set; } = null!;

    [MaxLength(250)]
    public string? Title { get; set; }

    public string? Content { get; set; }

    [MaxLength(100)]
    public string? ProviderName { get; set; }

    [Required]
    public DateTime AddedDate { get; set; } = DateTime.UtcNow;

    [Required]
    public Guid ProjectId { get; set; }

    [Required]
    public Guid PrincipalInvestigatorId { get; set; }

    // Navigation properties
    public virtual Project Project { get; set; } = null!;
    public virtual UserRole PrincipalInvestigator { get; set; } = null!;
}