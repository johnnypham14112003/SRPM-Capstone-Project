using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;
public class SystemConfiguration
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string? ConfigKey { get; set; }
    public string? ConfigValue { get; set; }

    [Required]
    [MaxLength(30)]
    public string ConfigType { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public DateTime LastUpdate { get; set; } = DateTime.Now;

    [Required]
    public DateTime CreateDate { get; set; } = DateTime.Now;

    // Navigation properties
    public virtual ICollection<Notification>? Notifications { get; set; }
}