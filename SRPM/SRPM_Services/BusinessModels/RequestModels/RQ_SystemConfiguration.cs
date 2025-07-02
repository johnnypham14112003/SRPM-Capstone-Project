using System.ComponentModel.DataAnnotations;

namespace SRPM_Services.BusinessModels.RequestModels;
public class RQ_SystemConfiguration
{
    public Guid? Id { get; set; }
    public string? ConfigKey { get; set; }//Name
    public string? ConfigValue { get; set; }
    [MaxLength(30)] public string? ConfigType { get; set; }

    public string? Description { get; set; }
    public DateTime? LastUpdate { get; set; }
    public DateTime? CreateDate { get; set; }
}