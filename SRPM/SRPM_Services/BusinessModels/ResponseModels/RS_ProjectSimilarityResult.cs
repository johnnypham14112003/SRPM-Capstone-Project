namespace SRPM_Services.BusinessModels.ResponseModels;

public class RS_ProjectSimilarityResult
{
    public Guid Id { get; set; }//projectId
    public string EnglishTitle { get; set; } = null!;
    public string? Description { get; set; }
    public string? EncodedDescription { get; set; }
    public double Similarity { get; set; }
}