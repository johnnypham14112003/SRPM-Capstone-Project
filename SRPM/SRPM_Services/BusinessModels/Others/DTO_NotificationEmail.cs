namespace SRPM_Services.BusinessModels.Others;

public class DTO_NotificationEmail
{
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;//email description
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public string? RefTitle { get; set; }//name object
    public string? RefContent { get; set; }//content object
    public string? RefButton { get; set; }//name object button
    public string? RefUrl { get; set; }//link to object detail
}