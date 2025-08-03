using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;

public class ResultPublish
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    [Required] public string Url { get; set; } = null!;
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Publisher { get; set; }
    public DateTime? PublicationDate { get; set; }
    public string? AccessType {get; set; }
    public string? Tags {get; set; }

    // Foreign keys
    [Required] public Guid ProjectResultId { get; set; }

    // Navigation properties
    public virtual ProjectResult ProjectResult { get; set; } = null!;
}