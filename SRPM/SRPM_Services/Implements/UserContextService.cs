using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SRPM_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.Implements
{
    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration; // Ensure you have access to configuration

        public UserContextService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public string GetCurrentUserId()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                throw new ArgumentException("Http context is null. Please login.");
            }
            var token = httpContext.Request.Cookies["jwt"];
            if (!string.IsNullOrEmpty(token))
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                if (jwtToken != null)
                {
                    var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                    if (userIdClaim != null)
                    {
                        return userIdClaim.Value;
                    }
                }
            }
            var user = httpContext.User;
            if (user == null || !user.Identity.IsAuthenticated)
            {
                throw new ArgumentException("User is not authenticated or token is invalid");
            }

            var currentUserId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId))
            {
                throw new ArgumentException("User ID claim is not found");
            }

            return currentUserId;
        }
    }
}
