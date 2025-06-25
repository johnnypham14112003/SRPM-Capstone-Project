using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SRPM_APIServices;
using SRPM_Repositories;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Repositories.Repositories.Implements;
using System.Security.Claims;
using System.Text;
using SRPM_Services.Interfaces;
using SRPM_Services.Implements;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var env = builder.Environment;

// Add services to the container.
builder.Services.RegisterServices(config, env);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme; ;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = config["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = config["Jwt:Audience"],
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(config["Jwt:Key"]))
    };
})
.AddCookie("Cookies")
.AddGoogle(GoogleDefaults.AuthenticationScheme, googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    googleOptions.Scope.Add("profile");
    googleOptions.Scope.Add("email");
    googleOptions.ClaimActions.MapJsonKey("AvatarUrl", "picture");

    googleOptions.Events.OnCreatingTicket = async context =>
    {
        var emailClaim = context.Identity.FindFirst(System.Security.Claims.ClaimTypes.Email);
        if (emailClaim is null)
        {
            throw new Exception("Email claim not found.");
        }

        // Read allowed domain from configuration (or default)
        var allowedDomain = builder.Configuration["AllowedEmailDomain"] ?? "fe.edu.vn";
        string expectedDomain = "@" + allowedDomain;

        if (!emailClaim.Value.EndsWith(expectedDomain, StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException($"Email must end with {expectedDomain}.");
        }

        // **Extract additional claims (avatar, name, etc.)**
        var pictureClaim = context.Identity.FindFirst("picture")?.Value;
        var nameClaim = context.Identity.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

        if (pictureClaim != null)
        {
            context.Identity.AddClaim(new System.Security.Claims.Claim("AvatarUrl", pictureClaim));
        }
        if (nameClaim != null)
        {
            context.Identity.AddClaim(new System.Security.Claims.Claim("FullName", nameClaim));
        }

    };
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
