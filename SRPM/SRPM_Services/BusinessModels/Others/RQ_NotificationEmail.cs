using System.ComponentModel.DataAnnotations;

namespace SRPM_Services.BusinessModels.Others;

public class RQ_NotificationEmail
{
    [Required] public string Subject { get; set; } = null!;
    public string? Body { get; set; }
    [Required] public string ReceiverEmailAddress { get; set; } = null!;
    public IEnumerable<string>? ListAddressToCc { get; set; }
    public IEnumerable<string>? ListAddressToBcc { get; set; }

    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;//email description
    public DateTime CreatedDate { get; set; }
    public string? RefTitle { get; set; }//name object
    public string? RefContent { get; set; }//content object
    public string? RefButton { get; set; }//name object button
    public string? RefUrl { get; set; }//link to object detail
}