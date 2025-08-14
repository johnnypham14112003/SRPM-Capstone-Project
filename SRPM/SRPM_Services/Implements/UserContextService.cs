using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SRPM_Services.Implements;

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration; // Ensure you have access to configuration
    private readonly IUnitOfWork _unitOfWork;

    public UserContextService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IUnitOfWork unitOfWork)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
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
        if (user == null || !user.Identity!.IsAuthenticated)
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
    public string GetCurrentUserRole()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            throw new ArgumentException("Http context is null. Please login.");
        }
        var user = httpContext.User;
        if (user == null || !user.Identity!.IsAuthenticated)
        {
            throw new ArgumentException("User is not authenticated or token is invalid");
        }

        var currentUserRole = user.FindFirstValue(ClaimTypes.Role);
        if (string.IsNullOrEmpty(currentUserRole))
        {
            throw new ArgumentException("User Role claim is not found");
        }

        return currentUserRole;
    }
    public async Task<Guid> GetCurrentUserBaseUserRoleIdAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            throw new ArgumentException("Http context is null. Please login.");

        var token = httpContext.Request.Cookies["jwt"];
        string? userId = null;

        if (!string.IsNullOrEmpty(token))
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            userId = jwtToken?.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        }

        if (string.IsNullOrEmpty(userId))
        {
            var user = httpContext.User;
            if (user?.Identity == null || !user.Identity.IsAuthenticated)
                throw new ArgumentException("User is not authenticated or token is invalid.");

            userId = user.FindFirstValue("Id");
        }

        if (string.IsNullOrEmpty(userId))
            throw new ArgumentException("User ID not found in token or claims.");

        var accountId = Guid.Parse(userId);

        var roleName = GetCurrentUserRole(); 
        var role = await _unitOfWork.GetRoleRepository().GetOneAsync(r => r.Name == roleName);
        if (role == null)
            throw new ArgumentException($"Role '{roleName}' not found.");

        var baseRole = await _unitOfWork.GetUserRoleRepository().GetOneAsync(a =>
            a.AccountId == accountId &&
            a.ProjectId == null &&
            a.AppraisalCouncilId == null &&
            a.RoleId == role.Id);

        if (baseRole == null)
            throw new ArgumentException("Base User Role not found for the current user.");

        return baseRole.Id;
    }
}