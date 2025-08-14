namespace SRPM_Services.Interfaces;

public interface IUserContextService
{
    string GetCurrentUserId();
    string GetCurrentUserRole();
    Task<Guid> GetCurrentUserBaseUserRoleIdAsync();
}