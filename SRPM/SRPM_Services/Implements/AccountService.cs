using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SRPM_Repositories.DTOs;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Services.Extensions.Exceptions;
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

        public AccountService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            // Read the allowed domain from configuration or default to "fe.edu.vn"
            _allowedEmailDomain = configuration["AllowedEmailDomain"] ?? "fe.edu.vn";
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

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

        public async Task<Account> LoginWithGoogleAsync(GoogleLoginRQ request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email))
                    throw new ArgumentException("Email is required for Google login.");

                string expectedDomain = "@" + _allowedEmailDomain;
                if (!request.Email.EndsWith(expectedDomain, StringComparison.OrdinalIgnoreCase))
                    throw new UnauthorizedAccessException($"Email must end with {expectedDomain}.");

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
