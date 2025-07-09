
using Microsoft.AspNetCore.Identity;
using SRPM_Repositories.Models;
using SRPM_Services.BusinessModels;
using SRPM_Services.BusinessModels.Others;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;


namespace SRPM_Services.Interfaces
{
    public interface IAccountService
    {
        Task<Account> LoginWithGoogleAsync(RQ_GoogleLogin request);
        // ... other account methods
        Task<Account> LoginWithEmailPasswordAsync(string email, string password);
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(RQ_ResetPassword request);
        Task<(bool IsVerified, int Attempt, DateTime Expiration)> VerifyOtpAsync(string email, string otp);
        Task<Account> HandleGoogleAsync(string googleToken);
        Task<RS_Account?> GetByIdAsync(Guid id);
        Task<RS_Account?> GetOnlineUserInfoAsync();
        Task<PagingResult<RS_Account>> GetListAsync(RQ_AccountQuery query);
        Task<RS_Account> CreateAsync(RQ_Account request);
        Task<RS_Account?> UpdateAsync(Guid id, RQ_Account request);
        Task<RS_Account?> ToggleStatusAsync(Guid id);
        Task<bool> DeleteAsync(Guid id);
    }
}
