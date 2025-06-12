using System.ComponentModel.DataAnnotations;

namespace SRPM_Services.BusinessModels.RequestModels
{
    public class RQ_AccountAuth
    {
        public string? FullName { get; set; }
        [Required][EmailAddress] public required string Email { get; set; }
        [Required] public required string Password { get; set; }
    }
}
