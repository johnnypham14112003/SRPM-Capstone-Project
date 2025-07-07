
using Microsoft.AspNetCore.Identity;
using SRPM_Repositories.Models;
using SRPM_Services.BusinessModels.Others;


namespace SRPM_Services.Interfaces
{
    public interface IAccountService
    {
        Task<Account> LoginWithGoogleAsync(RQ_GoogleLogin request);
        // ... other account methods
        Task<Account> LoginWithEmailPasswordAsync(string email, string password);
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(RQ_ResetPassword request);
        Task<bool> VerifyOtpAsync(string email, string otp);
        Task<Account> HandleGoogleAsync(string googleToken);
    }
}
