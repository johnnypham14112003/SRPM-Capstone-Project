using Google.Apis.Auth;
using Mapster;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Services.BusinessModels;
using SRPM_Services.BusinessModels.Others;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Extensions.Enumerables;
using SRPM_Services.Extensions.Exceptions;
using SRPM_Services.Extensions.FluentEmail;
using SRPM_Services.Interfaces;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace SRPM_Services.Implements;

public class AccountService : IAccountService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly string _allowedEmailDomain;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;
    private readonly IUserContextService _userContextService;

    public AccountService(IUnitOfWork unitOfWork, IConfiguration configuration, IEmailService emailService, IUserContextService userContextService)
    {
        _unitOfWork = unitOfWork;
        // Read the allowed domain from configuration or default to "fe.edu.vn"
        _allowedEmailDomain = configuration["AllowedEmailDomain"] ?? "fe.edu.vn";
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _userContextService = userContextService ?? throw new ArgumentNullException(nameof(userContextService));

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
                    CreateTime = DateTime.Now
                };

                await _unitOfWork.GetAccountRepository().AddAsync(account);
                await _unitOfWork.SaveChangesAsync();
            }
            if (account.Status.ToLower() == "deleted")
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
    public async Task<Account> HandleGoogleAsync(string accessToken)
    {
        var payload = await GetGoogleUserInfoAsync(accessToken)
            ?? throw new UnauthorizedAccessException("Unable to retrieve user info from Google.");

        if (string.IsNullOrWhiteSpace(payload.Email))
            throw new ArgumentException("Email is required for Google login.");

        string expectedDomain = "@" + _allowedEmailDomain;
        if (!payload.Email.EndsWith(expectedDomain, StringComparison.OrdinalIgnoreCase))
        {
            string errorRedirect = _configuration["ErrorRedirectUrl"];
            throw new RedirectException($"{errorRedirect}?reason=invalid_domain", "Invalid email domain.");
        }

        var accountRepo = _unitOfWork.GetAccountRepository();
        var account = await accountRepo.GetValidEmailAccountAsync(payload.Email);

        if (account == null)
        {
            account = await CreateNewAccountAsync(payload);
        }
        else
        {
            if (account.Status?.ToLower() == "deleted")
                throw new UnauthorizedException("This account has been deleted. Please contact support.");

            account.FullName = payload.Name;
            account.AvatarURL = payload.Picture;
            await accountRepo.UpdateAsync(account);
            await _unitOfWork.SaveChangesAsync();
        }

        return account;
    }

    private async Task<GoogleJsonWebSignature.Payload> GetGoogleUserInfoAsync(string accessToken)
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync($"https://www.googleapis.com/oauth2/v2/userinfo?access_token={accessToken}");

        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<GoogleJsonWebSignature.Payload>(json);
    }

    private async Task<Account> CreateNewAccountAsync(GoogleJsonWebSignature.Payload payload)
    {
        string randomPassword = GenerateRandomPassword(12);
        string hashedPassword = HashStringSHA256(randomPassword);
        string identityCode = ExtractIdentityCode(payload.Email);

        var newAccount = new Account
        {
            Id = Guid.NewGuid(),
            Email = payload.Email,
            FullName = payload.Name,
            AvatarURL = payload.Picture,
            IdentityCode = identityCode,
            Password = hashedPassword,
            Status = "created",
            CreateTime = DateTime.Now
        };

        var repo = _unitOfWork.GetAccountRepository();
        await repo.AddAsync(newAccount);
        await repo.SaveChangeAsync();

        return await repo.GetValidEmailAccountAsync(payload.Email)
            ?? throw new BadRequestException("Error occurred during Google signup.");
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
    private string GenerateOtp()
    {
        var random = new Random();
        int value = random.Next(0, 1000000); // allows numbers from 0 to 999999
        return value.ToString("D6"); // Pads with zeros to ensure 6 digits
    }

    public async Task<bool> ForgotPasswordAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.");

        var account = await _unitOfWork.GetAccountRepository()
            .GetOneAsync(a => a.Email == email, hasTrackings: false);

        if (account == null || account.Status.Equals("deleted", StringComparison.OrdinalIgnoreCase))
            throw new NotFoundException("No active account found for this email.");

        // Step 1: Generate OTP
        var lastOtp = await _unitOfWork.GetOTPCodeRepository()
            .GetListAsync(o => o.AccountId == account.Id);

        var recentOtp = lastOtp
            .OrderByDescending(o => o.ExpiresAt)
            .FirstOrDefault();

        if (recentOtp?.ExpiresAt != null && DateTime.Now < recentOtp.ExpiresAt.Value.AddHours(1))
        {
            throw new InvalidOperationException("You must wait 1 hour after the last OTP expired to request a new one.");
        }

        var otpCode = new OTPCode
        {
            AccountId = account.Id,
            Code = GenerateOtp(),
            ExpiresAt = DateTime.Now.AddMinutes(10),
            Attempt = 0
        };

        await _unitOfWork.GetOTPCodeRepository().AddAsync(otpCode);
        await _unitOfWork.SaveChangesAsync();

        // Step 2: Prepare model for Razor email
        var model = new PasswordEmailModel
        {
            UserName = account.FullName,
            WebsiteURL = _configuration["WebsiteURL"] ?? "https://SRPM.com",
            OtpCode = otpCode.Code
        };

        string body = await _emailService.RenderPasswordEmail(model);

        // Step 3: Send email
        var emailDto = new EmailDTO
        {
            ReceiverEmailAddress = email,
            Subject = "Reset Your Password - SRPM",
            Body = body
        };

        var emailSent = await _emailService.SendEmailAsync(emailDto);

        return emailSent;
    }



    public async Task<(bool IsVerified, int Attempt, DateTime? Expiration)> VerifyOtpAsync(string email, string inputOtp)
    {
        // Step 1: Get the account
        var account = await _unitOfWork.GetAccountRepository()
            .GetOneAsync(a => a.Email == email && a.Status.ToLower() != "deleted");

        if (account == null)
            throw new NotFoundException("Account not found.");

        // Step 2: Get latest OTP for the account
        var otpCodes = await _unitOfWork.GetOTPCodeRepository()
            .GetListAsync(o => o.AccountId == account.Id);

        var otpCode = otpCodes
            .OrderByDescending(o => o.ExpiresAt)
            .FirstOrDefault();

        if (otpCode == null)
            return (false, 0, DateTime.UtcNow); // No OTP issued

        // Step 3: Check expiration
        if (DateTime.Now < otpCode.ExpiresAt)
            return (false, otpCode.Attempt, otpCode.ExpiresAt); // Expired

        // Step 4: Lockout check
        if (otpCode.Attempt >= 3)
        {
            otpCode.ExpiresAt = DateTime.UtcNow; // Force expire on max attempts
            return (false, otpCode.Attempt, otpCode.ExpiresAt); // Max attempts reached — no retry
        }

        // Step 5: Verify OTP
        if (otpCode.Code == inputOtp)
            return (true, otpCode.Attempt, otpCode.ExpiresAt); // Correct and valid

        // Step 6: Increment only if still under limit
        otpCode.Attempt++;

        await _unitOfWork.GetOTPCodeRepository().UpdateAsync(otpCode);
        await _unitOfWork.SaveChangesAsync();

        return (false, otpCode.Attempt, otpCode.ExpiresAt); // Incorrect OTP
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

        if (otp.ExpiresAt < DateTime.Now)
            throw new UnauthorizedAccessException("OTP has expired.");

        if (otp.Attempt >= 3)
            throw new UnauthorizedAccessException("Maximum OTP attempts exceeded.");

        // All checks passed — increment attempt (for history) and expire OTP
        otp.Attempt++;
        otp.ExpiresAt = DateTime.Now;

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
    public async Task<PagingResult<RS_Account>> GetListAsync(RQ_AccountQuery query)
    {
        var list = await _unitOfWork.GetAccountRepository().GetListAsync(
            a =>
                (string.IsNullOrWhiteSpace(query.IdentityCode) || a.IdentityCode.Contains(query.IdentityCode)) &&
                (string.IsNullOrWhiteSpace(query.FullName) || a.FullName.Contains(query.FullName)) &&
                (string.IsNullOrWhiteSpace(query.Email) || a.Email.Contains(query.Email)) &&
                (string.IsNullOrWhiteSpace(query.PhoneNumber) || (a.PhoneNumber ?? "").Contains(query.PhoneNumber)) &&
                (string.IsNullOrWhiteSpace(query.Status) || a.Status == query.Status),
            hasTrackings: false
        );

        // Apply sorting
        list = query.SortBy?.ToLower() switch
        {
            "fullname" => query.Desc ? list.OrderByDescending(x => x.FullName).ToList() : list.OrderBy(x => x.FullName).ToList(),
            "email" => query.Desc ? list.OrderByDescending(x => x.Email).ToList() : list.OrderBy(x => x.Email).ToList(),
            "identitycode" => query.Desc ? list.OrderByDescending(x => x.IdentityCode).ToList() : list.OrderBy(x => x.IdentityCode).ToList(),
            _ => list.OrderBy(x => x.FullName).ToList() // Default sort
        };

        var total = list.Count;
        var paged = list
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        return new PagingResult<RS_Account>
        {
            PageIndex = query.PageIndex,
            PageSize = query.PageSize,
            TotalCount = total,
            DataList = paged.Adapt<List<RS_Account>>()
        };
    }


    public async Task<RS_Account> CreateAsync(RQ_Account request)
    {
        var entity = request.Adapt<Account>();
        entity.Id = Guid.NewGuid();
        entity.CreateTime = DateTime.Now;

        // Extract identity code from email — everything before '@'
        var identity = request.Email.Split('@')[0].Trim().ToLowerInvariant();
        entity.IdentityCode = identity;

        // Status conversion
        entity.Status = request.Status.ToStatus().ToString().ToLowerInvariant();

        await _unitOfWork.GetAccountRepository().AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return entity.Adapt<RS_Account>();
    }
    public async Task<RS_Account?> GetByIdAsync(Guid id)
    {
        var entity = await _unitOfWork.GetAccountRepository().GetByIdAsync<Guid>(id);
        return entity?.Adapt<RS_Account>();
    }
    public async Task<RS_Account?> GetOnlineUserInfoAsync()
    {
        var id = _userContextService.GetCurrentUserId();
        var entity = await _unitOfWork.GetAccountRepository().GetByIdAsync<Guid>(Guid.Parse(id));
        return entity?.Adapt<RS_Account>();
    }

    public async Task<RS_Account?> UpdateAsync(Guid id, RQ_Account request)
    {
        var repo = _unitOfWork.GetAccountRepository();
        var entity = await repo.GetByIdAsync<Guid>(id);
        if (entity == null) return null;

        entity.Status = request.Status.ToStatus().ToString().ToLowerInvariant();
        request.Adapt(entity);

        await repo.UpdateAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return entity.Adapt<RS_Account>();
    }

    public async Task<RS_Account?> ToggleStatusAsync(Guid id)
    {
        var repo = _unitOfWork.GetAccountRepository();
        var entity = await repo.GetByIdAsync<Guid>(id);
        if (entity == null) return null;

        var current = entity.Status.ToStatus();
        entity.Status = current == Status.Created
            ? Status.Deleted.ToString().ToLower()
            : Status.Created.ToString().ToLower();

        await repo.UpdateAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return entity.Adapt<RS_Account>();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _unitOfWork.GetAccountRepository().GetByIdAsync<Guid>(id);
        if (entity == null) return false;

        await _unitOfWork.GetAccountRepository().DeleteAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }


}