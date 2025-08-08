namespace SRPM_Services.BusinessModels.Others;

public class DTO_NotificationEmail
{
    public string Content { get; set; } = null!;//email description
    public DateTime CreatedDate { get; set; }
    public string? RefTitle { get; set; }//name object
    public string? RefURL { get; set; }//link to object detail
}