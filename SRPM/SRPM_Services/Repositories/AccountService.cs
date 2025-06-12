using Microsoft.Extensions.Configuration;
using SRPM_Repositories.DTOs;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Services.Interfaces;
using System.Diagnostics;

namespace SRPM_Services.Repositories
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _allowedEmailDomain;

        public AccountService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            // Read the allowed domain from configuration or default to "fe.edu.vn"
            _allowedEmailDomain = configuration["AllowedEmailDomain"] ?? "fe.edu.vn";
        }

        public async Task<Account> LoginWithGoogleAsync(GoogleLoginRQ request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email))
                    throw new ArgumentException("Email is required for Google login.");

                // Check that the email ends with the configured domain.
                string expectedDomain = "@" + _allowedEmailDomain;
                if (!request.Email.EndsWith(expectedDomain, StringComparison.OrdinalIgnoreCase))
                    throw new UnauthorizedAccessException($"Email must end with {expectedDomain}.");

                // Attempt to get an existing account by email.
                var account = await _unitOfWork.GetAccountRepository()
                    .GetOneAsync(a => a.Email == request.Email, hasTrackings: false);

                if (account == null)
                {
                    // If account is not found, create a new account.
                    account = new Account
                    {
                        Id = Guid.NewGuid(),
                        Email = request.Email,
                        FullName = request.Name
                    };

                    await _unitOfWork.GetAccountRepository().AddAsync(account);
                    await _unitOfWork.SaveChangesAsync();
                }
                else
                {
                    // Optionally update the account information.
                    account.FullName = request.Name;
                    await _unitOfWork.GetAccountRepository().UpdateAsync(account);
                    await _unitOfWork.SaveChangesAsync();
                }

                return account;
            }
            catch (Exception ex)
            {
                // Log the exception for debugging.
                Debug.WriteLine($"Exception in LoginWithGoogleAsync: {ex.Message}");
                // Optionally, log the full exception stack trace:
                Debug.WriteLine(ex.ToString());

                // Rethrow the exception to let the caller handle it or return a friendly error message.
                throw;
            }
        }
    }
}
