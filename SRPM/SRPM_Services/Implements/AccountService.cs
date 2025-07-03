using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Services.BusinessModels.Others;
using SRPM_Services.Extensions.Exceptions;
using SRPM_Services.Extensions.FluentEmail;
using SRPM_Services.Interfaces;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SRPM_Services.Implements
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _allowedEmailDomain;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AccountService(IUnitOfWork unitOfWork, IConfiguration configuration, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            // Read the allowed domain from configuration or default to "fe.edu.vn"
            _allowedEmailDomain = configuration["AllowedEmailDomain"] ?? "fe.edu.vn";
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));

        }

        private static string GenerateRandomPassword(int length = 12)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private static string ExtractIdentityCode(string email)
        {
            return email.Split('@')[0]; // Gets the portion before '@'
        }

        public async Task<Account> LoginWithGoogleAsync(RQ_GoogleLogin request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email))
                    throw new ArgumentException("Email is required for Google login.");

                string expectedDomain = "@" + _allowedEmailDomain;
                if (!request.Email.EndsWith(expectedDomain, StringComparison.OrdinalIgnoreCase))
                {
                    var errorRedirect = _configuration["ErrorRedirectUrl"];
                    throw new RedirectException($"{errorRedirect}?reason=invalid_domain", "Invalid email domain.");
                }


                var account = await _unitOfWork.GetAccountRepository()
                    .GetOneAsync(a => a.Email == request.Email, hasTrackings: false);

                if (account == null)
                {
                    string randomPassword = GenerateRandomPassword(12);
                    string hashedPassword = HashStringSHA256(randomPassword);
                    string identityCode = ExtractIdentityCode(request.Email);

                    account = new Account
                    {
                        Id = Guid.NewGuid(),
                        Email = request.Email,
                        FullName = request.Name,
                        AvatarURL = request.AvatarUrl,
                        IdentityCode = identityCode, // Extracted from email
                        Password = hashedPassword,
                        Status = "created",
                        CreateTime = DateTime.UtcNow
                    };

                    await _unitOfWork.GetAccountRepository().AddAsync(account);
                    await _unitOfWork.SaveChangesAsync();
                }
                if(account.Status.ToLower() == "deleted")
                {
                    throw new UnauthorizedException("This account has been deleted. Please contact support for assistance.");
                }
                    account.FullName = request.Name;
                    account.AvatarURL = request.AvatarUrl;
                    await _unitOfWork.GetAccountRepository().UpdateAsync(account);
                    await _unitOfWork.SaveChangesAsync();

                return account;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception in LoginWithGoogleAsync: {ex.Message}");
                Debug.WriteLine(ex.ToString());
                throw;
            }
        }

        public async Task<Account> LoginWithEmailPasswordAsync(string email, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                    throw new ArgumentException("Email and password are required.");

                string expectedDomain = "@" + _allowedEmailDomain;
                if (!email.EndsWith(expectedDomain, StringComparison.OrdinalIgnoreCase))
                    throw new UnauthorizedAccessException($"Email must end with {expectedDomain}.");

                var account = await _unitOfWork.GetAccountRepository()
                    .GetOneAsync(a => a.Email == email, hasTrackings: false);

                if (account == null)
                    throw new UnauthorizedAccessException("Invalid email or password.");

                if (account.Status.ToLower() == "deleted")
                    throw new UnauthorizedException("This account has been deleted. Please contact support for assistance.");

                string hashedInput = HashStringSHA256(password);
                if (!string.Equals(account.Password, hashedInput, StringComparison.OrdinalIgnoreCase))
                    throw new UnauthorizedAccessException("Invalid email or password.");

                return account;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception in LoginWithEmailPasswordAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.");

            // Check if the user exists
            var account = await _unitOfWork.GetAccountRepository()
                .GetOneAsync(a => a.Email == email, hasTrackings: false);

            if (account == null || account.Status.ToLower() == "deleted")
                throw new NotFoundException("No active account found for this email.");

            string body = await _emailService.RenderPasswordEmail(null);

            var emailDto = new EmailDTO
            {
                ReceiverEmailAddress = email,
                Subject = "🔐 Reset Your Password",
                Body = body
            };

            // Send the email
            var emailSent = await _emailService.SendEmailAsync(emailDto);

            return emailSent;
        }


        public async Task<bool> VerifyOtpAsync(string email, string otp)
        {
            var account = await _unitOfWork.GetAccountRepository()
                .GetOneAsync(a => a.Email == email && a.Status.ToLower() != "deleted");

            if (account == null)
                throw new NotFoundException("Account not found.");

            var otpCode = account.OTPCodes?
                .OrderByDescending(o => o.ExpiresAt)
                .FirstOrDefault(o => o.Code == otp);

            if (otpCode == null)
                return false;

            if (otpCode.ExpiresAt < DateTime.UtcNow)
                return false;

            if (otpCode.Attempt >= 3)
                return false;

            otpCode.Attempt++;
            await _unitOfWork.GetOTPCodeRepository().UpdateAsync(otpCode);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
 

        public async Task<bool> ResetPasswordAsync(RQ_ResetPassword request)
        {
            if (request.NewPassword != request.ConfirmPassword)
                throw new ArgumentException("New password and confirmation do not match.");

            var account = await _unitOfWork.GetAccountRepository()
                .GetOneAsync(a => a.Email == request.Email && a.Status.ToLower() != "deleted");

            if (account == null)
                throw new NotFoundException("Account not found.");

            var otp = account.OTPCodes?
                .OrderByDescending(o => o.ExpiresAt)
                .FirstOrDefault(o => o.Code == request.OTP);

            if (otp == null)
                throw new UnauthorizedAccessException("Invalid OTP.");

            if (otp.ExpiresAt < DateTime.UtcNow)
                throw new UnauthorizedAccessException("OTP has expired.");

            if (otp.Attempt >= 3)
                throw new UnauthorizedAccessException("Maximum OTP attempts exceeded.");

            // All checks passed — increment attempt (for history) and expire OTP
            otp.Attempt++;
            otp.ExpiresAt = DateTime.UtcNow; // Invalidate after use

            string hashedPassword = HashStringSHA256(request.NewPassword);
            account.Password = hashedPassword;

            await _unitOfWork.GetOTPCodeRepository().UpdateAsync(otp);
            await _unitOfWork.GetAccountRepository().UpdateAsync(account);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }



        private string HashStringSHA256(string input)//SHA-256 Algorithm (1 way)
        {
            using SHA256 sha256Hasher = SHA256.Create();
            // ComputeHash - returns byte array  
            byte[] bytes = sha256Hasher.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Convert byte array to a string   
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }

}
