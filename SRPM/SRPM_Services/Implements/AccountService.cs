using Google.Apis.Auth;
using Mapster;
using Microsoft.EntityFrameworkCore;
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
using SRPM_Services.Extensions.MicrosoftBackgroundService;
using SRPM_Services.Interfaces;
using System.Data;
using System.Diagnostics;
using System.Linq.Expressions;
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
    private readonly ITaskQueueHandler _taskQueueHandler;
    public AccountService(ITaskQueueHandler taskQueueHandler, IUnitOfWork unitOfWork, IConfiguration configuration, IEmailService emailService, IUserContextService userContextService)
    {
        _unitOfWork = unitOfWork;
        // Read the allowed domain from configuration or default to "fe.edu.vn"
        _allowedEmailDomain = configuration["AllowedEmailDomain"] ?? "fe.edu.vn";
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _userContextService = userContextService ?? throw new ArgumentNullException(nameof(userContextService));
        _taskQueueHandler = taskQueueHandler ?? throw new ArgumentNullException(nameof(taskQueueHandler));
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

    public async Task<Account> HandleGoogleAsync(string accessToken)
    {
        var payload = await GetGoogleUserInfoAsync(accessToken)
            ?? throw new UnauthorizedAccessException("Unable to retrieve user info from Google.");

        if (string.IsNullOrWhiteSpace(payload.Email))
            throw new ArgumentException("Email is required for Google login.");

        string expectedDomain = "@" + _allowedEmailDomain;
        var accountRepo = _unitOfWork.GetAccountRepository();

        if (!payload.Email.EndsWith(expectedDomain, StringComparison.OrdinalIgnoreCase))
        {

            var existingAccount = await accountRepo.GetValidEmailAccountAsync(payload.Email);
            if (existingAccount == null)
            {
                string errorRedirect = _configuration["ErrorRedirectUrl"];
                throw new RedirectException($"{errorRedirect}?reason=invalid_domain", "Invalid email domain.");
            }

            if (existingAccount.Status?.ToLower() == "deleted")
                throw new UnauthorizedException("This account has been deleted. Please contact support.");

            existingAccount.FullName = payload.Name;
            existingAccount.AvatarURL = payload.Picture;
            await accountRepo.UpdateAsync(existingAccount);
            await _unitOfWork.SaveChangesAsync();

            return existingAccount;
        }

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
            return null!;

        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<GoogleJsonWebSignature.Payload>(json)!;
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

    public async Task<(bool isSuccess, int TTL)> ForgotPasswordAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.");

        // Get config
        var ttlConfig = await _unitOfWork.GetSystemConfigurationRepository()
            .GetOneAsync(c => c.ConfigKey.Contains("otp ttl") && c.ConfigType == "security");
        var maxRetryTimeConfig = await _unitOfWork.GetSystemConfigurationRepository()
            .GetOneAsync(c => c.ConfigKey.Contains("otp retry time") && c.ConfigType == "security");
        var ttlMinutes = ttlConfig != null && int.TryParse(ttlConfig.ConfigValue, out var parsedTtl) ? parsedTtl : 15; // Default 15 minutes
        var maxRetryTime = maxRetryTimeConfig != null && int.TryParse(maxRetryTimeConfig.ConfigValue, out var parsedRetry) ? parsedRetry : 60; // Default 60 minutes
        var account = await _unitOfWork.GetAccountRepository()
            .GetOneAsync(a => a.Email == email, hasTrackings: false);

        if (account == null || account.Status.Equals("deleted", StringComparison.OrdinalIgnoreCase))
            throw new NotFoundException("No active account found for this email.");

        // Step 1: Check rate limiting - prevent too frequent OTP requests
        var lastOtp = await _unitOfWork.GetOTPCodeRepository()
            .GetListAsync(o => o.AccountId == account.Id);

        var recentOtp = lastOtp?
            .OrderByDescending(o => o.ExpiresAt)
            .FirstOrDefault();

        if (recentOtp?.ExpiresAt != null && DateTime.Now < recentOtp.ExpiresAt.Value.AddMinutes(maxRetryTime))
        {
            throw new InvalidOperationException("You must wait " + maxRetryTime + " minutes after the last OTP expired to request a new one.");
        }
        var otpCode = new OTPCode
        {
            AccountId = account.Id,
            Code = GenerateOtp(),
            ExpiresAt = DateTime.Now.AddMinutes(ttlMinutes), // Use configurable TTL
            TTL = ttlMinutes,
            Attempt = 0
        };

        await _unitOfWork.GetOTPCodeRepository().AddAsync(otpCode);
        await _unitOfWork.SaveChangesAsync();


        _taskQueueHandler.EnqueueTracked(async (serviceProvider, token, progress) =>
        {
            var model = new DTO_PasswordEmail
            {
                UserName = account.FullName,
                WebsiteURL = _configuration["WebsiteURL"] ?? "https://srpm.com",
                OtpCode = otpCode.Code
            };

            string body = await _emailService.RenderPasswordEmail(model);

            var emailDto = new EmailDTO
            {
                ReceiverEmailAddress = email,
                Subject = "[SRPM] Reset Your Password",
                Body = body
            };
            await _emailService.SendEmailAsync(emailDto);
        });
        return (true, otpCode.TTL);
    }

    public async Task<(bool IsVerified, int Attempt, DateTime? Expiration)> VerifyOtpAsync(string email, string inputOtp)
    {
        var ttlConfig = await _unitOfWork.GetSystemConfigurationRepository()
            .GetOneAsync(c => c.ConfigKey.ToLower().Contains("otp ttl") && c.ConfigType == "security");

        var otpAttemptLimitConfig = await _unitOfWork.GetSystemConfigurationRepository()
            .GetOneAsync(c => c.ConfigKey.ToLower().Contains("otp attempt") && c.ConfigType == "security");

        var ttlMinutes = ttlConfig != null && int.TryParse(ttlConfig.ConfigValue, out var parsedTtl)
            ? parsedTtl
            : 15; // Default 15 minutes

        var maxAttempts = otpAttemptLimitConfig != null && int.TryParse(otpAttemptLimitConfig.ConfigValue, out var parsedAttempts)
            ? parsedAttempts
            : 3; // Default 3 attempts
        var account = await _unitOfWork.GetAccountRepository()
            .GetOneAsync(a => a.Email == email && a.Status.ToLower() != "deleted");

        if (account == null)
            throw new NotFoundException("Account not found.");

        var otpCodes = await _unitOfWork.GetOTPCodeRepository()
            .GetListAsync(o => o.AccountId == account.Id);

        var otpCode = otpCodes?
            .OrderByDescending(o => o.ExpiresAt)
            .FirstOrDefault();

        if (otpCode == null)
            return (false, 0, null); 

        var currentTime = DateTime.Now;

        if (currentTime > otpCode.ExpiresAt) 
            return (false, otpCode.Attempt, otpCode.ExpiresAt);

        if (otpCode.Attempt >= maxAttempts)
        {            otpCode.ExpiresAt = currentTime;
            await _unitOfWork.GetOTPCodeRepository().UpdateAsync(otpCode);
            await _unitOfWork.SaveChangesAsync();

            return (false, otpCode.Attempt, otpCode.ExpiresAt); 
        }

        if (otpCode.Code == inputOtp)
        {
            await _unitOfWork.GetOTPCodeRepository().UpdateAsync(otpCode);
            await _unitOfWork.SaveChangesAsync();

            return (true, otpCode.Attempt, otpCode.ExpiresAt);
        }

        otpCode.Attempt++;
        await _unitOfWork.GetOTPCodeRepository().UpdateAsync(otpCode);
        await _unitOfWork.SaveChangesAsync();
        _taskQueueHandler.EnqueueTracked(async (serviceProvider, token, progress) =>
        {
            await CleanupExpiredOtpsAsync();
        });
            return (false, otpCode.Attempt, otpCode.ExpiresAt); 
    }


    public async Task<bool> ResetPasswordAsync(RQ_ResetPassword request)
    {
        if (request.NewPassword != request.ConfirmPassword)
            throw new ArgumentException("New password and confirmation do not match.");

        var otpAttemptLimitConfig = await _unitOfWork.GetSystemConfigurationRepository()
            .GetOneAsync(c => c.ConfigKey.Contains("otp attempt") && c.ConfigType == "security");

        var maxAttempts = otpAttemptLimitConfig != null && int.TryParse(otpAttemptLimitConfig.ConfigValue, out var parsedAttempts) ? parsedAttempts : 3;

        var account = await _unitOfWork.GetAccountRepository()
            .GetOneAsync(a => a.Email == request.Email && a.Status.ToLower() != "deleted");

        if (account == null)
            throw new NotFoundException("Account not found.");

        // Get the latest OTP for this account
        var otpCodes = await _unitOfWork.GetOTPCodeRepository()
            .GetListAsync(o => o.AccountId == account.Id);

        var otp = otpCodes?
            .OrderByDescending(o => o.ExpiresAt)
            .FirstOrDefault(o => o.Code == request.OTP);

        if (otp == null)
            throw new UnauthorizedAccessException("Invalid OTP.");

        var currentTime = DateTime.Now;

        if (currentTime > otp.ExpiresAt) // Current time is AFTER expiration = expired
            throw new UnauthorizedAccessException("OTP has expired.");

        // Check max attempts
        if (otp.Attempt >= maxAttempts) // Use configurable max attempts
            throw new UnauthorizedAccessException("Maximum OTP attempts exceeded.");


        string hashedPassword = HashStringSHA256(request.NewPassword);
        account.Password = hashedPassword;
        otp.ExpiresAt = currentTime;
        await _unitOfWork.GetOTPCodeRepository().UpdateAsync(otp);
        await _unitOfWork.GetAccountRepository().UpdateAsync(account);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // Optional: Helper method to clean up expired OTPs
    public async System.Threading.Tasks.Task CleanupExpiredOtpsAsync()
    {
        var expiredOtps = await _unitOfWork.GetOTPCodeRepository()
            .GetListAsync(o => o.ExpiresAt < DateTime.Now);

        foreach (var expiredOtp in expiredOtps)
        {
            await _unitOfWork.GetOTPCodeRepository().DeleteAsync(expiredOtp);
        }

        await _unitOfWork.SaveChangesAsync();
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
            "fullname" => query.Desc ? list!.OrderByDescending(x => x.FullName).ToList() : list!.OrderBy(x => x.FullName).ToList(),
            "email" => query.Desc ? list!.OrderByDescending(x => x.Email).ToList() : list!.OrderBy(x => x.Email).ToList(),
            "identitycode" => query.Desc ? list!.OrderByDescending(x => x.IdentityCode).ToList() : list!.OrderBy(x => x.IdentityCode).ToList(),
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
    public async Task<List<object>> SearchByNameOrEmailAsync(string? input, string? userRole)
    {
        var keyword = input?.Trim();
        var roleKeyword = userRole?.Trim();
        var onlineUser = _userContextService.GetCurrentUserId();
        List<Account> results;

        // Build dynamic predicate
        Expression<Func<Account, bool>> predicate = a =>
            (string.IsNullOrWhiteSpace(keyword) ||
             (!string.IsNullOrEmpty(a.FullName) && a.FullName.Contains(keyword)) ||
             (!string.IsNullOrEmpty(a.Email) && a.Email.Contains(keyword))) &&
             a.Id.ToString() != onlineUser && // Exclude the current user
            (string.IsNullOrWhiteSpace(roleKeyword) ||
             (a.UserRoles != null &&
              a.UserRoles.Any(ur => ur.Role != null &&
                                    !string.IsNullOrEmpty(ur.Role.Name) &&
                                    ur.Role.Name.Contains(roleKeyword))));

        results = await _unitOfWork.GetAccountRepository()
            .GetListAsync(predicate, hasTrackings: false,
                include: q => q.Include(a => a.UserRoles).ThenInclude(ur => ur.Role));

        var projected = results!
            .Select(a => new
            {
                a.Id,
                a.FullName,
                a.Email,
                a.AvatarURL
            })
            .Cast<object>()
            .ToList();

        return projected;
    }


    public async Task<RS_Account> CreateAsync(RQ_Account request)
    {
        var existing = await _unitOfWork.GetAccountRepository()
            .GetOneAsync(a => a.Email == request.Email, hasTrackings: false);

        if (existing != null)
            throw new DuplicateNameException($"An account with email '{request.Email}' already exists.");

        var entity = request.Adapt<Account>();
        entity.Id = Guid.NewGuid();
        entity.CreateTime = DateTime.Now;

        var identity = request.Email.Split('@')[0].Trim().ToLowerInvariant();
        entity.IdentityCode = identity;

        entity.Status = request.Status.ToStatus().ToString().ToLowerInvariant();
        entity.Password = HashStringSHA256(request.Password);

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
        var accountRepo = _unitOfWork.GetAccountRepository();
        var userRoleRepo = _unitOfWork.GetUserRoleRepository();

        // Include UserRoles when retrieving the account
        var entity = await accountRepo.GetOneAsync(
            a => a.Id == id,
            include: q => q.Include(a => a.UserRoles)
        );

        if (entity == null) return false;

        // Delete associated UserRoles
        if (entity.UserRoles != null && entity.UserRoles.Any())
        {
            foreach (var userRole in entity.UserRoles)
            {
                await userRoleRepo.DeleteAsync(userRole);
            }
        }

        // Delete the account
        await accountRepo.DeleteAsync(entity);

        // Commit all changes
        await _unitOfWork.SaveChangesAsync();

        return true;
    }


}