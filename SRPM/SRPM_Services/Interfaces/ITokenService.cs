using System.Security.Claims;

namespace SRPM_Services.Implements
{
    public interface ITokenService
    {
        string GenerateJwtToken(IEnumerable<Claim> claims);
    }
}