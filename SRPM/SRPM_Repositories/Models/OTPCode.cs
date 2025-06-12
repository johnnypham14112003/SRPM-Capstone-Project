using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;
public class OTPCode
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string Code { get; set; } // Stores the OTP code for authentication

    public DateTime? ExpiresAt { get; set; } // Expiration timestamp for the OTP

    [Required]
    public byte Attempt { get; set; } = 1; // Tracks the number of attempts

    [Required]
    public int TTL { get; set; } = 1; // Time to live (TTL) in minutes

    // Foreign Key
    [Required]
    public Guid AccountId { get; set; }
    public Account Account { get; set; } = null!;// One Account -> Many OTP Codes
}