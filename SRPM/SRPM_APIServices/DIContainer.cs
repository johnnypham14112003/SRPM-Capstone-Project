using Mapster;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Serialization;
using SRPM_Repositories;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Extensions;
using SRPM_Services.Interfaces;
using SRPM_Services.Extensions.Mapster;
using SRPM_Services.Implements;
using SRPM_Repositories.Repositories.Implements;
using Microsoft.OpenApi.Models;
using SRPM_Repositories.Models;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.Extensions.Enumerables;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace SRPM_APIServices;

public static class DIContainer
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        //System Services
        services.InjectDbContext(configuration);
        services.InjectBusinessServices();
        services.InjectRepository();
        services.ConfigCORS();
        services.ConfigKebabCase();
        services.ConfigMapster();
        services.ConfigJsonLoopDeserielize();
        services.InjectSwagger();

        //Third Party Services
<<<<<<< Updated upstream
=======
        services.AddCustomAuthentication(configuration);
        services.ConfigFluentEmail(configuration);
        services.ConfigRazorTemplate(configuration, env);
>>>>>>> Stashed changes
        //...

        return services;
    }

    //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
    //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
    private static IServiceCollection InjectDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<SRPMDbContext>(options => options.UseSqlServer(connectionString));
        return services;
    }

    private static IServiceCollection InjectBusinessServices(this IServiceCollection services)
    {
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ISystemConfigurationService, SystemConfigurationService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IEvaluationService, EvaluationService>();
        services.AddScoped<IEvaluationStageService, EvaluationStageService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IProjectMajorService, ProjectMajorService>();
        services.AddScoped<IMajorService, MajorService>();
        services.AddScoped<IProjectTagService, ProjectTagService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IUserRoleService, UserRoleService>();
        services.AddScoped<IIndividualEvaluationService, IndividualEvaluationService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<IMilestoneService, MilestoneService>();
        services.AddScoped<IMemberTaskService, MemberTaskService>();   
        services.AddScoped<IUserContextService, UserContextService>();


        //Add other BusinessServices here...

        return services;
    }

    private static IServiceCollection InjectRepository(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        //---------------------------------------------------------------------------
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ISystemConfigurationRepository, SystemConfigurationRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IAccountNotificationRepository, AccountNotificationRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IEvaluationRepository, EvaluationRepository>();
        services.AddScoped<IEvaluationStageRepository, EvaluationStageRepository>();
        services.AddScoped<IIndividualEvaluationRepository, IndividualEvaluationRepository>();
        services.AddScoped<IMemberTaskRepository, MemberTaskRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IMajorRepository, MajorRepository>();
        services.AddScoped<IMilestoneRepository, MilestoneRepository>();
        services.AddScoped<IOTPCodeRepository, OTPCodeRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IProjectMajorRepository, ProjectMajorRepository>();
        services.AddScoped<IProjectTagRepository, ProjectTagRepository>();
        services.AddScoped<IResearchPaperRepository, ResearchPaperRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<ISignatureRepository, SignatureRepository>();

        //Add other repository here...

        return services;
    }

    //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
    //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
    private static IServiceCollection ConfigKebabCase(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.Conventions.Add(new RouteTokenTransformerConvention(new KebabRouteTransform()));
        }).AddNewtonsoftJson(options =>
        {//If using NewtonSoft in project then must orride default Naming rule of System.text
            options.SerializerSettings.ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new KebabCaseNamingStrategy()
            };
        });

        services.AddSwaggerGen(c => { c.SchemaFilter<KebabSwaggerSchema>(); });
        return services;
    }

    private static IServiceCollection ConfigMapster(this IServiceCollection services)
    {
        //TypeAdapterConfig<AccountRequested, Account>.NewConfig().IgnoreNullValues(true);
        //TypeAdapterConfig<OrderDetail_InfoDto, OrderDetail>.NewConfig().IgnoreNullValues(true)
        //    .Map(destination => destination.Id, startFrom => startFrom.OrderDetailId);

        //=============================
        //How to use:
        /*
        ModelA objA = ... ;
        ModelB objB = ... ;

        //Transfer data from A -> B
        objA.Adapt(objB);

        //Other way:
        var objB = objA.Adapt<ModelB>();
        */
        //==============================

        //Config Example:
        //========================[ Language ]========================
        //AccountLanguage => LanguageBM
        /*
         * TypeAdapterConfig<AccountLanguage, LanguageBM>.NewConfig()
            .Map(dest => dest.Id, src => src.LanguageId)
            .Map(dest => dest.Name, src => src.Language.Name)
            .Map(dest => dest.Status, src => src.Language.Status);

        //LanguageBM => AccountLanguage
        TypeAdapterConfig<LanguageBM, AccountLanguage>.NewConfig()
            .Map(dest => dest.LanguageId, src => src.Id)
            .Ignore(dest => dest.Account)
            .Ignore(dest => dest.Language);

        //========================[ Skill ]========================
        //SkillBM => ProjectSkill
        TypeAdapterConfig<SkillBM, ProjectSkill>.NewConfig()
            .Map(dest => dest.SkillId, src => src.Id)
            .Ignore(dest => dest.Project)
            .Ignore(dest => dest.SkillId);

        //SkillBM => ProposalSkill
        TypeAdapterConfig<SkillBM, ProposalSkill>.NewConfig()
            .Map(dest => dest.SkillId, src => src.Id)
            .Ignore(dest => dest.Proposal)
            .Ignore(dest => dest.SkillId);

        //========================[ Certification ]========================
        //CertificationBM => Certification
        TypeAdapterConfig<CertificationBM, Certification>.NewConfig()
            .Map(dest => dest.Freelancer, src => new Account { Id = src.FreeLancerId });

        //========================[ Education ]========================
        //EducationBM => Education
        TypeAdapterConfig<EducationBM, Education>.NewConfig()
            .Map(dest => dest.Freelancer, src => new Account { Id = src.FreeLancerId });
        */

        // Config NotificationWithReadStatus -> RS_AccountNotification
        TypeAdapterConfig<NotificationWithReadStatus, RS_AccountNotification>.NewConfig()
            .Map(dest => dest.Id, src => src.Notification.Id)
            .Map(dest => dest.Title, src => src.Notification.Title)
            .Map(dest => dest.Type, src => src.Notification.Type)
            .Map(dest => dest.CreateDate, src => src.Notification.CreateDate)
            .Map(dest => dest.IsGlobalSend, src => src.Notification.IsGlobalSend)
            .Map(dest => dest.Status, src => src.Notification.Status)
            .Map(dest => dest.IsRead, src => src.IsRead)
            .Map(dest => dest.AccountId, src => src.AccountId)
            .Map(dest => dest.TypeObjectId, src => MapsterConfigMethods.GetTypeObjectIdByType(src.Notification));

        TypeAdapterConfig<RQ_Evaluation, Evaluation>.NewConfig()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Code)
            .Ignore(dest => dest.CreateDate)
            .Ignore(dest => dest.Status)
            .Ignore(dest => dest.Documents)
            .Ignore(dest => dest.EvaluationStages)
            .Ignore(dest => dest.Notifications)
            .IgnoreNullValues(true);
        TypeAdapterConfig<Evaluation, RS_Evaluation>.NewConfig();

        TypeAdapterConfig<RQ_EvaluationStage, EvaluationStage>.NewConfig()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Transactions)
            .Ignore(dest => dest.IndividualEvaluations)
            .Ignore(dest => dest.Notifications)
            .Ignore(dest => dest.AppraisalCouncil)
            .Ignore(dest => dest.Evaluation)
            .IgnoreNullValues(true);

        
        TypeAdapterConfig<EvaluationStage, RS_EvaluationStage>.NewConfig();

        TypeAdapterConfig<RQ_IndividualEvaluation, IndividualEvaluation>.NewConfig()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.SubmittedAt)
            .Ignore(dest => dest.Notifications)
            .Ignore(dest => dest.Documents)
            .Ignore(dest => dest.Project)
            .Ignore(dest => dest.Milestone)
            .Ignore(dest => dest.Reviewer)
            .Ignore(dest => dest.EvaluationStage)
            .IgnoreNullValues(true); 

        TypeAdapterConfig<IndividualEvaluation, RS_IndividualEvaluation>.NewConfig();
        
        TypeAdapterConfig<RQ_Project, Project>.NewConfig()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.UpdatedAt)
            .Ignore(dest => dest.Creator)
            .Ignore(dest => dest.ResearchPaper)
            .Ignore(dest => dest.Members)
            .Ignore(dest => dest.Milestones)
            .Ignore(dest => dest.Evaluations)
            .Ignore(dest => dest.IndividualEvaluations)
            .Ignore(dest => dest.Documents)
            .Ignore(dest => dest.ProjectMajors)
            .Ignore(dest => dest.ProjectTags)
            .Ignore(dest => dest.Transactions)
            .IgnoreNullValues(true);

        TypeAdapterConfig<Project, RS_Project>.NewConfig();
        TypeAdapterConfig<RQ_ProjectMajor, ProjectMajor>.NewConfig()
            .Ignore(dest => dest.Project)
            .Ignore(dest => dest.Major);

        TypeAdapterConfig<ProjectMajor, RS_ProjectMajor>.NewConfig();
        TypeAdapterConfig<RQ_Major, Major>.NewConfig()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Field)
            .Ignore(dest => dest.Accounts)
            .Ignore(dest => dest.ProjectMajors)
            .IgnoreNullValues(true);

        TypeAdapterConfig<Major, RS_Major>.NewConfig();

        TypeAdapterConfig<RQ_Role, Role>.NewConfig()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.UserRoles)
            .IgnoreNullValues(true);

        TypeAdapterConfig<Role, RS_Role>.NewConfig()
            .Map(dest => dest.Status, src => src.Status.ToStatus());

            TypeAdapterConfig<RQ_UserRole, UserRole>.NewConfig()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.Account)
                .Ignore(dest => dest.Role)
                .Ignore(dest => dest.Project)
                .Ignore(dest => dest.AppraisalCouncil)
                .Ignore(dest => dest.UploadedDocuments)
                .Ignore(dest => dest.Signatures)
                .Ignore(dest => dest.ResearchPapers)
                .Ignore(dest => dest.IndividualEvaluations)
                .Ignore(dest => dest.CreatedProjects)
                .Ignore(dest => dest.CreatedMilestones)
                .Ignore(dest => dest.CreatedTasks)
                .Ignore(dest => dest.MemberTasks)
                .Ignore(dest => dest.RequestTransactions)
                .Ignore(dest => dest.HandleTransactions)
                .Ignore(dest => dest.Notifications)
                .IgnoreNullValues(true);

            TypeAdapterConfig<UserRole, RS_UserRole>.NewConfig();
        TypeAdapterConfig<RQ_Task, SRPM_Repositories.Models.Task>.NewConfig()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Milestone)
            .Ignore(dest => dest.Creator)
            .Ignore(dest => dest.MemberTasks)
            .Ignore(dest => dest.Notifications)
            .IgnoreNullValues(true);

        TypeAdapterConfig<SRPM_Repositories.Models.Task, RS_Task>.NewConfig();
        TypeAdapterConfig<RQ_Milestone, Milestone>.NewConfig()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.Project)
            .Ignore(dest => dest.Creator)
            .Ignore(dest => dest.Evaluations)
            .Ignore(dest => dest.IndividualEvaluations)
            .Ignore(dest => dest.Tasks)
            .IgnoreNullValues(true);

        TypeAdapterConfig<Milestone, RS_Milestone>.NewConfig();

        TypeAdapterConfig<RQ_MemberTask, MemberTask>.NewConfig()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.JoinedAt)
            .Ignore(dest => dest.Member)
            .Ignore(dest => dest.Task)
            .Ignore(dest => dest.Notifications)
            .IgnoreNullValues(true);

        TypeAdapterConfig<MemberTask, RS_MemberTask>.NewConfig();

        return services;
    }
    


    private static IServiceCollection ConfigJsonLoopDeserielize(this IServiceCollection services)
    {
        services.AddControllers().AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        });
        return services;
    }

    private static IServiceCollection ConfigCORS(this IServiceCollection services)
    {
        services.AddCors(options => options.AddPolicy("AllowAll", b => b.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod()));
        return services;
    }
    public static IServiceCollection InjectSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "API"
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "JWT Authorization header sử dụng scheme Bearer.",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });
        });

        return services;
    }
    public static IServiceCollection AddCustomAuthentication(
        this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
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
                    Encoding.UTF8.GetBytes(config["Jwt:Key"])),
                RoleClaimType = ClaimTypes.Role
            };
        })
        .AddCookie("Cookies")
        .AddGoogle(GoogleDefaults.AuthenticationScheme, googleOptions =>
        {
            googleOptions.ClientId = config["Authentication:Google:ClientId"];
            googleOptions.ClientSecret = config["Authentication:Google:ClientSecret"];
            googleOptions.Scope.Add("profile");
            googleOptions.Scope.Add("email");
            googleOptions.ClaimActions.MapJsonKey("AvatarUrl", "picture");

            googleOptions.Events.OnCreatingTicket = async context =>
            {
                var emailClaim = context.Identity.FindFirst(ClaimTypes.Email);
                if (emailClaim is null)
                {
                    throw new Exception("Email claim not found.");
                }

                var allowedDomain = config["AllowedEmailDomain"] ?? "fe.edu.vn";
                var expectedDomain = "@" + allowedDomain;

                if (!emailClaim.Value.EndsWith(expectedDomain, StringComparison.OrdinalIgnoreCase))
                {
                    throw new UnauthorizedAccessException($"Email must end with {expectedDomain}.");
                }

                var pictureClaim = context.Identity.FindFirst("picture")?.Value;
                var nameClaim = context.Identity.FindFirst(ClaimTypes.Name)?.Value;

                if (pictureClaim != null)
                {
                    context.Identity.AddClaim(new Claim("AvatarUrl", pictureClaim));
                }
                if (nameClaim != null)
                {
                    context.Identity.AddClaim(new Claim("FullName", nameClaim));
                }
            };
        });

        return services;
    }
    public static IApplicationBuilder UseCustomCors(this IApplicationBuilder app)
    {
        var allowedOrigins = new[] {
            "http://localhost:5173"
        };

        app.UseCors("CustomPolicy");

        app.Use(async (context, next) =>
        {
            var origin = context.Request.Headers["Origin"].ToString();

            if (allowedOrigins.Contains(origin))
            {
                context.Response.Headers.Append("Access-Control-Allow-Origin", origin);
                context.Response.Headers.Append("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
                context.Response.Headers.Append("Access-Control-Allow-Headers", "Content-Type, Authorization");
            }

            if (context.Request.Method == "OPTIONS")
            {
                context.Response.StatusCode = 204;
                await context.Response.CompleteAsync();
                return;
            }

            await next();
        });

        return app;
    }

}