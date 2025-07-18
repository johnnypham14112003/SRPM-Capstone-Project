using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;

public class ProjectSimilarity
{
    // Composite Primary Key
    [Required] public Guid ProjectId { get; set; }
    [Required] public Guid IndividualEvaluationId { get; set; }

    public double Similarity { get; set; }

    // Navigation properties
    public Project Project { get; set; } = null!;
    public IndividualEvaluation IndividualEvaluation { get; set; } = null!;
}