using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SRPM_Services.Interfaces;
using System;
using System.Threading.Tasks;
using System.Security.Claims;
using SRPM_Services.Extensions.Exceptions;
using SRPM_Services.BusinessModels.Others;
using SRPM_Services.Extensions;
using Microsoft.AspNetCore.WebUtilities;

namespace SRPM_APIServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IUserRoleService _roleService;
        private readonly ITokenService _tokenService;
        private readonly ISessionService _sessionService;

        public AuthController(
            IAccountService accountService,
            ITokenService tokenService,
            IUserRoleService roleService,
            ISessionService sessionService)
        {
            _accountService = accountService;
            _tokenService = tokenService;
            _roleService = roleService;
            _sessionService = sessionService;
        }


        //    [HttpPost("google-login")]
        //    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRQ request)
        //    {
        //        try
        //        {
        //            var account = await _accountService.LoginWithGoogleAsync(request);
        //            return Ok(account);
        //        }
        //        catch (UnauthorizedAccessException ex)
        //        {
        //            return Unauthorized(new { message = ex.Message });
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(new { message = ex.Message });
        //        }
        //    }
        //}
        // Endpoint to trigger the Google login challenge.
        [HttpGet("google-login")]
        public IActionResult GoogleLogin(string returnUrl = "/", string role = null)
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse", new { returnUrl, role })
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }


        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse(string returnUrl = "/", string role = null)
        {
            var user = HttpContext.User;

            var email = user.FindFirst(ClaimTypes.Email)?.Value;
            var name = user.FindFirst(ClaimTypes.Name)?.Value;
            var avatarUrl = user.FindFirst("AvatarUrl")?.Value;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(name))
                return BadRequest("Missing required information from Google account.");

            var loginRequest = new RQ_GoogleLogin
            {
                Email = email,
                Name = name,
                AvatarUrl = avatarUrl,
                SelectedRole = role
            };

            try
            {
                var account = await _accountService.LoginWithGoogleAsync(loginRequest);

                if (account == null)
                    throw new NotFoundException("Account not found during Google login flow.");

                if (role != null)
                {
                    var isAuthorized = await _roleService.UserHasRoleAsync(account.Id, role);
                    if (!isAuthorized)
                        return Forbid("Selected role is not assigned to this user.");
                }

                var allRoles = await _roleService.GetAllUserRole(account.Id);

                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
            new Claim("Id", account.Id.ToString()),
            new Claim(ClaimTypes.Role, role ?? string.Empty)
        };

                var token = _tokenService.GenerateJwtToken(claims);

                var sessionPayload = new
                {
                    Token = token,
                    FullName = account.FullName,
                    AvatarUrl = account.AvatarURL,
                    Email = account.Email,
                    SelectedRole = role,
                    Roles = allRoles
                };

                var sessionId = Guid.NewGuid().ToString();
                _sessionService.Store(sessionId, sessionPayload, TimeSpan.FromMinutes(5));

                Response.Cookies.Append("sessionId", sessionId, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddMinutes(10)
                });

                return Redirect(returnUrl);
            }
            catch (RedirectException redirectEx)
            {
                return Redirect(redirectEx.RedirectUrl);
            }
        }



        [HttpGet("session")]
        public IActionResult GetSessionFromCookie()
        {
            var sessionId = Request.Cookies["sessionId"];
            if (string.IsNullOrEmpty(sessionId))
                return BadRequest("Session cookie not found.");

            var sessionData = _sessionService.Retrieve(sessionId);
            if (sessionData == null)
                return NotFound("Session expired or invalid");

            return Ok(sessionData);
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

        // ✅ Verify OTP
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] string email, string otp)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(otp))
                return BadRequest("Email and OTP are required.");

            var isValid = await _accountService.VerifyOtpAsync(email, otp);

            if (!isValid)
                return Unauthorized("OTP is invalid, expired, or maximum attempts exceeded.");

            return Ok(new { Message = "OTP is valid." });
        }

        // 🔄 Reset Password
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
}
