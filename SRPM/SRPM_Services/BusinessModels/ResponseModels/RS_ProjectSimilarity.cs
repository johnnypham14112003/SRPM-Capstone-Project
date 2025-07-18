namespace SRPM_Services.BusinessModels.ResponseModels;

public class RS_ProjectSimilarity
{
    public Guid ProjectId { get; set; }
    public Guid IndividualEvaluationId { get; set; }

    public double Similarity { get; set; }
}