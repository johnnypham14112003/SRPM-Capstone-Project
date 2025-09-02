using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using SRPM_APIServices;
using SRPM_APIServices.Middlewares;
using SRPM_Services.Extensions.Hubs;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var env = builder.Environment;
var configBuild = builder.Configuration;

// Add services to the container.
builder.Configuration.AddAzureKeyVault(
    builder.Configuration["KeyVault:KeyVaultURL"],
    new DefaultKeyVaultSecretManager()
);

builder.Services.RegisterServices(config, env, configBuild);
builder.Services.AddSignalR();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCustomCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/notificationhub"); 

app.Run();