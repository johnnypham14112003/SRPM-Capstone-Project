using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SRPM_APIServices;
using SRPM_Repositories;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Repositories.Repositories.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.RegisterServices(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<SRPMDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; // for persistence
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme; // for external login
})
.AddCookie()
.AddGoogle(googleOptions =>
{
    // These values should be stored in configuration (appsettings.json or secrets)
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

    // Event to check allowed email domain
    googleOptions.Events.OnCreatingTicket = context =>
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

        // Optionally, you can also capture additional claims and add them to the principal here.
        return Task.CompletedTask;
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
