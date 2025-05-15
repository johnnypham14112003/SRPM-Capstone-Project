using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SRPM_Repositories.DTOs;
using SRPM_Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace SRPM_APIServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AuthController(IAccountService accountService)
        {
            _accountService = accountService;
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
    public IActionResult GoogleLogin(string returnUrl = "/")
    {
        // Initiate the challenge; the challenge scheme is Google.
        var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse", new { returnUrl }) };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    // Callback endpoint once authentication is complete.
    [HttpGet("google-response")]
    public async Task<IActionResult> GoogleResponse(string returnUrl = "/")
    {
        // Perform the authentication (this triggers the OnCreatingTicket event configured in Program.cs)
        var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (!authenticateResult.Succeeded)
        {
            return Unauthorized("Google authentication failed.");
        }

        // At this point the user is authenticated and the allowed email domain has been checked.
        // You can access user claims via authenticateResult.Principal.
        var email = authenticateResult.Principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var name = authenticateResult.Principal.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

        // Here you would call your AccountService (via DI) to create or update the account in your database.
        // For example:
        // var account = await _accountService.LoginWithGoogleAsync(new GoogleLoginRQ { Email = email, Name = name });
        //
        // For simplicity, we just return a success message with claims.

        return Ok(new { Email = email, Name = name, ReturnUrl = returnUrl });
    }
}
}
