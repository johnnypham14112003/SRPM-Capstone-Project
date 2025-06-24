using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SRPM_Repositories.DTOs;
using SRPM_Services.Interfaces;
using System;
using System.Threading.Tasks;
using System.Security.Claims;
using SRPM_Services.Implements;
using SRPM_Services.Extensions.Exceptions;

namespace SRPM_APIServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IUserRoleService _roleService;
        private readonly ITokenService _tokenService;

        public AuthController(IAccountService accountService, ITokenService tokenService, IUserRoleService roleService)
        {
            _accountService = accountService;
            _tokenService = tokenService;
            _roleService = roleService; 
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

            var loginRequest = new GoogleLoginRQ
            {
                Email = email,
                Name = name,
                AvatarUrl = avatarUrl,
                SelectedRole = role
            };

            var account = await _accountService.LoginWithGoogleAsync(loginRequest);
            if (account == null)
                throw new NotFoundException("Account not found during Google login flow.");
            var isAuthorized = await _roleService.UserHasRoleAsync(account.Id, role);

            if (!isAuthorized)
                return Forbid("Selected role is not assigned to this user.");

            var allRoles = await _roleService.GetAllUserRole(account.Id);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
        new Claim("Id", account.Id.ToString()),
        new Claim(ClaimTypes.Role, role ?? string.Empty)
    };

            var token = _tokenService.GenerateJwtToken(claims);

            return Ok(new
            {
                Token = token,
                FullName = account.FullName,
                AvatarUrl = account.AvatarURL,
                Email = account.Email,
                SelectedRole = role,
                Roles = allRoles,
                ReturnUrl = returnUrl
            });
        }




    }
}
