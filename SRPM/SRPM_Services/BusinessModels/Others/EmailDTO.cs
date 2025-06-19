using System.ComponentModel.DataAnnotations;

namespace SRPM_Services.BusinessModels.Others;

public class EmailDTO
{
    [Required] public string Subject { get; set; } = null!;
    [Required] public string Body { get; set; } = null!;
    [Required] public string ReceiverEmailAddress { get; set; } = null!;
    public IEnumerable<string>? ListAddressToCC { get; set; }
    public IEnumerable<string>? ListAddressToBCC { get; set; }
}