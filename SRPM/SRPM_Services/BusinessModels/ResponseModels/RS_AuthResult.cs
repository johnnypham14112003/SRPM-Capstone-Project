namespace SRPM_Services.BusinessModels.ResponseModels
{
    public class RS_AuthResult
    {
        public Guid Id { get; set; }
        public string FirebaseUid { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string FullName { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public string? Cccd { get; set; }

        public string? Gender { get; set; }

        public string Role { get; set; } = null!;

        public string Status { get; set; } = null!;
    }
}
