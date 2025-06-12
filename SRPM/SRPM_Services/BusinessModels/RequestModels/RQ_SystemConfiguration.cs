using System.ComponentModel.DataAnnotations;

namespace SRPM_Services.BusinessModels.RequestModels;
public class RQ_SystemConfiguration
{
    public Guid? Id { get; set; }
    public string? ConfigKey { get; set; }//Name
    public string? ConfigValue { get; set; }

    [Required]
    [MaxLength(30)]
    public string? ConfigType { get; set; }

    public string? Description { get; set; }

    [Required]
    public DateTime LastUpdate { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
}