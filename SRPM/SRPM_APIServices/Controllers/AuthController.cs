using Microsoft.AspNetCore.Mvc;
using SRPM_Services.Interfaces;
using System.Security.Claims;
using SRPM_Services.Extensions.Exceptions;
using SRPM_Services.BusinessModels.Others;
using SRPM_Services.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace SRPM_APIServices.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : Controller
{
    private readonly IAccountService _accountService;
    private readonly IUserRoleService _roleService;
    private readonly ITokenService _tokenService;
    private readonly IUserContextService _userContextService;

    public AuthController(
        IAccountService accountService,
        ITokenService tokenService,
        IUserRoleService roleService,
        IUserContextService userContextService)
    {
        _accountService = accountService;
        _tokenService = tokenService;
        _roleService = roleService;
       _userContextService = userContextService;
    }

    [HttpPost]
    [Route("google-authentication")]
    [AllowAnonymous]
    public async Task<IActionResult> GoogleLogin([FromQuery] string Token)
    {
        var account = await _accountService.HandleGoogleAsync(Token);
        if (account == null)
            return BadRequest("Account not found during Google login flow.");

        var allRoles = await _roleService.GetAllUserRole(account.Id);

        var selectedRole = allRoles.Contains("Researcher")
            ? "Researcher"
            : allRoles.FirstOrDefault();

        if (!string.IsNullOrEmpty(selectedRole))
        {
            var isAuthorized = await _roleService.UserHasRoleAsync(account.Id, selectedRole);
            if (!isAuthorized)
                return Forbid("Selected role is not assigned to this user.");
        }

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
        new Claim("Id", account.Id.ToString()),
        new Claim(ClaimTypes.Role, selectedRole ?? string.Empty)
    };

        var jwtToken = _tokenService.GenerateJwtToken(claims);
        if (string.IsNullOrEmpty(jwtToken))
            return BadRequest("There is an error during generate JWT Token!");

        return Ok(new
        {
            Token = jwtToken,
            FullName = account.FullName,
            AvatarUrl = account.AvatarURL,
            Email = account.Email,
            SelectedRole = selectedRole,
            Roles = allRoles
        });
    }
    [HttpPost]
    [Route("switch-role")]
    [Authorize(Roles ="Principal Investigator, Researcher")]
    public async Task<IActionResult> SwitchRole(string selectedSwitchRole)
    {
        var account = await _accountService.GetOnlineUserInfoAsync();
        var currentRole = _userContextService.GetCurrentUserRole();
        var allRoles = await _roleService.GetAllUserRole(account!.Id);

        var isAuthorized = await _roleService.UserHasRoleAsync(account.Id, selectedSwitchRole);
         if (!isAuthorized)
           return Forbid("Selected role is not assigned to this user.");

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
        new Claim("Id", account.Id.ToString()),
        new Claim(ClaimTypes.Role, selectedSwitchRole ?? string.Empty)
    };

        var jwtToken = _tokenService.GenerateJwtToken(claims);
        if (string.IsNullOrEmpty(jwtToken))
            return BadRequest("There is an error during generate JWT Token!");

        return Ok(new
        {
            Token = jwtToken,
            FullName = account.FullName,
            AvatarUrl = account.AvatarURL,
            Email = account.Email,
            SelectedRole = selectedSwitchRole,
            Roles = allRoles
        });
    }


    [HttpPost("login")]
    public async Task<IActionResult> LoginWithEmailPassword([FromBody] RQ_EmailPasswordLogin request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Email and password are required.");

        var account = await _accountService.LoginWithEmailPasswordAsync(request.Email, request.Password);
        if (account == null)
            return Unauthorized("Invalid credentials.");

        // Validate selected role
        if (!string.IsNullOrEmpty(request.SelectedRole))
        {
            var isAuthorized = await _roleService.UserHasRoleAsync(account.Id, request.SelectedRole);
            if (!isAuthorized)
                return Forbid("Selected role is not assigned to this user.");
        }

        var allRoles = await _roleService.GetAllUserRole(account.Id);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
            new Claim("Id", account.Id.ToString()),
            new Claim(ClaimTypes.Role, request.SelectedRole ?? string.Empty)
        };

        var token = _tokenService.GenerateJwtToken(claims);

        return Ok(new
        {
            Token = token,
            FullName = account.FullName,
            AvatarUrl = account.AvatarURL,
            Email = account.Email,
            SelectedRole = request.SelectedRole,
            Roles = allRoles
        });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return BadRequest("Email is required.");

        try
        {
            var result = await _accountService.ForgotPasswordAsync(email);

            if (!result)
                return StatusCode(500, "Failed to send password reset email. Try again later.");

            return Ok(new
            {
                Message = "A password reset email has been sent if this email is associated with a valid account."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                Error = ex.Message
            });
        }
    }


    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] string Email, string Otp)
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Otp))
            return BadRequest(new { Message = "Email and OTP are required." });

        var (isVerified, attempt, expiration) = await _accountService.VerifyOtpAsync(Email, Otp);

        if (!isVerified)
        {
            return Unauthorized(new
            {
                Message = "OTP is invalid, expired, or maximum attempts exceeded.",
                Attempt = attempt,
                ExpirationTime = expiration
            });
        }

        return Ok(new
        {
            Message = "OTP is valid.",
            Attempt = attempt,
            ExpirationTime = expiration
        });
    }



    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] RQ_ResetPassword request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _accountService.ResetPasswordAsync(request);

        if (!result)
            return StatusCode(500, "Failed to reset password. Try again later.");

        return Ok(new { Message = "Password has been reset successfully." });
    }

}