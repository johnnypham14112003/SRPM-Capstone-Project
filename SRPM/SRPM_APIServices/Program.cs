using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using SRPM_Repositories.DBContext;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Repositories.Repositories.Repositories;
using SRPM_Services.Interfaces;
using SRPM_Services.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register DbContext before building the app
builder.Services.AddDbContext<SRPMDbContext>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

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
