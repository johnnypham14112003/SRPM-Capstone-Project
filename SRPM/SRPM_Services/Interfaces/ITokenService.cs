using System.Security.Claims;

namespace SRPM_Services.Interfaces;

public interface ITokenService
{
    string GenerateJwtToken(IEnumerable<Claim> claims);
}