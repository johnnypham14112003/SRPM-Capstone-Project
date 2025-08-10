using Mapster;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using OpenAI.Embeddings;
using SRPM_Repositories;
using SRPM_Repositories.Models;
using SRPM_Repositories.Repositories.Implements;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Services.BusinessModels.Others;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Extensions;
using SRPM_Services.Extensions.Enumerables;
using SRPM_Services.Extensions.FluentEmail;
using SRPM_Services.Extensions.Mapster;
using SRPM_Services.Extensions.OpenAI;
using SRPM_Services.Implements;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using SRPM_Services.Interfaces;
using OpenAI.Chat;
using System.Collections.Generic;
using SRPM_Services.Extensions.MicrosoftBackgroundService;

namespace SRPM_APIServices;

public static class DIContainer
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
    {
        //System Services
        services.InjectDbContext(configuration);
        services.InjectBusinessServices();
        services.InjectRepository();
        services.ConfigCORS();
        services.ConfigKebabCase();
        services.ConfigEnumMember();
        services.ConfigJsonLoopDeserielize();
        services.AddCustomAuthentication(configuration);
        services.AddHttpContextAccessor();
        services.ConfigBackgroundService();
        services.InjectSwagger();


        //Third Party Services
        services.ConfigMapster();
        services.ConfigFluentEmail(configuration);
        services.ConfigRazorTemplate(configuration, env);
        services.ConfigOpenAI(configuration);
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
        services.AddScoped<IAppraisalCouncilService, AppraisalCouncilService>();
        services.AddScoped<IDocumentService, DocumentService>();
        services.AddScoped<IEvaluationService, EvaluationService>();
        services.AddScoped<IEvaluationStageService, EvaluationStageService>();
        services.AddScoped<IIndividualEvaluationService, IndividualEvaluationService>();
        services.AddScoped<IMajorService, MajorService>();
        services.AddScoped<IMemberTaskService, MemberTaskService>();
        services.AddScoped<IMilestoneService, MilestoneService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IProjectMajorService, ProjectMajorService>();
        services.AddScoped<IProjectTagService, ProjectTagService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<ISystemConfigurationService, SystemConfigurationService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<IUserRoleService, UserRoleService>();
        services.AddScoped<IUserContextService, UserContextService>();
        services.AddScoped<ISessionService, MemorySessionService>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<IFieldService, FieldService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<ISignatureService, SignatureService>();

        //Extensions Services
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IOpenAIService, OpenAIService>();

        //Add other BusinessServices here...

        return services;
    }

    private static IServiceCollection InjectRepository(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        //---------------------------------------------------------------------------
        services.AddScoped<IAccountNotificationRepository, AccountNotificationRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IAppraisalCouncilRepository, AppraisalCouncilRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IEvaluationRepository, EvaluationRepository>();
        services.AddScoped<IEvaluationStageRepository, EvaluationStageRepository>();
        services.AddScoped<IIndividualEvaluationRepository, IndividualEvaluationRepository>();
        services.AddScoped<IMajorRepository, MajorRepository>();
        services.AddScoped<IMemberTaskRepository, MemberTaskRepository>();
        services.AddScoped<IMilestoneRepository, MilestoneRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IOTPCodeRepository, OTPCodeRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IProjectMajorRepository, ProjectMajorRepository>();
        services.AddScoped<IProjectSimilarityRepository, ProjectSimilarityRepository>();
        services.AddScoped<IProjectTagRepository, ProjectTagRepository>();
        services.AddScoped<IProjectResultRepository, ProjectResultRepository>();
        services.AddScoped<IResultPublishRepository, ResultPublishRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<ISignatureRepository, SignatureRepository>();
        services.AddScoped<ISystemConfigurationRepository, SystemConfigurationRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IFieldRepository, FieldRepository>();

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
            .Map(dest => dest.Id, src => src.Id ?? Guid.NewGuid());

        TypeAdapterConfig<RQ_EvaluationStage, EvaluationStage>.NewConfig()
            .Map(dest => dest.Id, src => src.Id ?? Guid.NewGuid());

        TypeAdapterConfig<RQ_IndividualEvaluation, IndividualEvaluation>.NewConfig()
            .Map(dest => dest.Id, src => src.Id ?? Guid.NewGuid());

        TypeAdapterConfig<RQ_Notification, Notification>.NewConfig()
            .Map(dest => dest.Id, src => src.Id ?? Guid.NewGuid());

        TypeAdapterConfig<RQ_AppraisalCouncil, AppraisalCouncil>.NewConfig()
            .Map(dest => dest.Id, src => src.Id ?? Guid.NewGuid());

        TypeAdapterConfig<RQ_Document, Document>.NewConfig()
            .Map(dest => dest.Id, src => src.Id ?? Guid.NewGuid());

        TypeAdapterConfig<RQ_SystemConfiguration, SystemConfiguration>.NewConfig()
            .Map(dest => dest.Id, src => src.Id ?? Guid.NewGuid());

        TypeAdapterConfig<RQ_Transaction, Transaction>.NewConfig()
            .Map(dest => dest.Id, src => src.Id ?? Guid.NewGuid());

        TypeAdapterConfig<RQ_Project, Project>.NewConfig()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.UpdatedAt)
            .Ignore(dest => dest.Creator)
            .Ignore(dest => dest.ProjectResult)
            .Ignore(dest => dest.Members)
            .Ignore(dest => dest.Milestones)
            .Ignore(dest => dest.Evaluations)
            .Ignore(dest => dest.ProjectsSimilarity)
            .Ignore(dest => dest.Documents)
            .Ignore(dest => dest.ProjectMajors)
            .Ignore(dest => dest.ProjectTags)
            .Ignore(dest => dest.Transactions)
            .IgnoreNullValues(true);

        TypeAdapterConfig<Project, Project>.NewConfig()
            .Ignore(dest => dest.Creator)
            .Ignore(dest => dest.ProjectResult)
            .Ignore(dest => dest.Members)
            .Ignore(dest => dest.Milestones)
            .Ignore(dest => dest.Evaluations)
            .Ignore(dest => dest.ProjectsSimilarity)
            .Ignore(dest => dest.ProjectMajors)
            .Ignore(dest => dest.ProjectTags)
            .Ignore(dest => dest.Documents)
            .Ignore(dest => dest.Transactions);

        TypeAdapterConfig<Project, RS_Project>.NewConfig()
            .Map(dest => dest.Majors, src =>
                (src.ProjectMajors != null && src.ProjectMajors.Any())
                    ? src.ProjectMajors
                        .Where(pm => pm.Major != null && pm.Major.Field != null)
                        .Select(pm => new RS_MajorBrief
                        {
                            Id = pm.Major.Id,
                            Name = pm.Major.Name,
                            Field = new RS_FieldBrief
                            {
                                Id = pm.Major.Field.Id,
                                Name = pm.Major.Field.Name
                            }
                        }).ToList()
                    : new List<RS_MajorBrief>()
            )
            .IgnoreNullValues(true);

        TypeAdapterConfig<RQ_ProjectMajor, ProjectMajor>.NewConfig()
            .Ignore(dest => dest.Project)
            .Ignore(dest => dest.Major);

        TypeAdapterConfig<ProjectMajor, RS_ProjectMajor>.NewConfig()
            .Map(dest => dest.MajorId, src => src.MajorId)
            .Map(dest => dest.Major, src => src.Major == null ? null : new RS_MajorBrief
            {
                Id = src.MajorId,
                Name = src.Major.Name,
                Field = src.Major.Field == null ? null : new RS_FieldBrief
                {
                    Id = src.Major.FieldId,
                    Name = src.Major.Field.Name
                }
            });



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
            .Ignore(dest => dest.IndividualEvaluations)
            .Ignore(dest => dest.CreatedProjects)
            .Ignore(dest => dest.CreatedMilestones)
            .Ignore(dest => dest.CreatedTasks)
            .Ignore(dest => dest.MemberTasks)
            .Ignore(dest => dest.RequestTransactions)
            .Ignore(dest => dest.HandleTransactions)
            .Ignore(dest => dest.Notifications)
            .IgnoreNullValues(true);

        TypeAdapterConfig<UserRole, RS_UserRole>.NewConfig()
            .Map(dest => dest.FullName, src => src.Account != null ? src.Account.FullName : string.Empty)
            .Map(dest => dest.Email, src => src.Account != null ? src.Account.Email : string.Empty)
            .Map(dest => dest.AvatarURL, src => src.Account != null ? src.Account.AvatarURL : string.Empty)
            .Map(dest => dest.Name, src => src.Role != null ? src.Role.Name : string.Empty);
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
            .Ignore(dest => dest.EvaluationStages)
            .Ignore(dest => dest.Tasks)
            .IgnoreNullValues(true);

        TypeAdapterConfig<Milestone, RS_Milestone>.NewConfig()
            .Ignore(m => m.Project) // prevents recursive loop
            .IgnoreNullValues(true);



        TypeAdapterConfig<RQ_MemberTask, MemberTask>.NewConfig()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.JoinedAt)
            .Ignore(dest => dest.Member)
            .Ignore(dest => dest.Task)
            .Ignore(dest => dest.Notifications)
            .IgnoreNullValues(true);

        TypeAdapterConfig<MemberTask, RS_MemberTask>.NewConfig()
            .Map(dest => dest.FullName, src => src.Member.Account.FullName)
            .Map(dest => dest.AvatarUrl, src => src.Member.Account.AvatarURL)
            .Map(dest => dest.RoleName, src => src.Member.Role.Name);
        TypeAdapterConfig<RQ_Account, Account>.NewConfig()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreateTime)
            .Ignore(dest => dest.DeleteTime)
            .IgnoreNullValues(true);

        TypeAdapterConfig<Account, RS_Account>.NewConfig();

        TypeAdapterConfig<UserRole, RS_UserRoleDetail>.NewConfig()
        .Map(dest => dest.FullName, src => src.Account.FullName)
        .Map(dest => dest.Email, src => src.Account.Email)
        .Map(dest => dest.PhoneNumber, src => src.Account.PhoneNumber)
        .Map(dest => dest.Address, src => src.Account.Address)
        .Map(dest => dest.CompanyName, src => src.Account.CompanyName)
        .Map(dest => dest.AvatarURL, src => src.Account.AvatarURL)
        .Map(dest => dest.Name, src => src.Role.Name)
        // Optional: Map other fields as needed
        .IgnoreNullValues(true); // optional setting for cleanliness


        return services;
    }

    private static IServiceCollection ConfigEnumMember(this IServiceCollection services)
    {
        services.AddControllers().AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.Converters.Add(new StringEnumConverter());
        });
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
        services.AddCors(options => options.AddPolicy("AllowAll", b =>
            b.WithOrigins("https://localhost:5173")  // ⬅️ Change this
             .AllowAnyHeader()
             .AllowAnyMethod()
             .AllowCredentials()));
        return services;
    }

    private static IServiceCollection ConfigBackgroundService(this IServiceCollection services)
    {
        services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
        services.AddSingleton<ITaskQueueHandler, TaskQueueHandler>();
        services.AddSingleton<ITaskTracker, TaskTracker>();
        services.AddHostedService<BackgroundTaskService>();
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

    private static IServiceCollection ConfigFluentEmail(this IServiceCollection services, IConfiguration configuration)
    {
        //strong-typed config
        /*prevent compile error if missing appsettings vars*/
        var feOpts = new FluentEmailOptionModel();
        configuration.GetSection("FluentEmail").Bind(feOpts);

        services.AddFluentEmail(feOpts.Address).AddSmtpSender(feOpts.Host, feOpts.Port, feOpts.Address, feOpts.AppPassword);
        return services;
    }

    private static IServiceCollection ConfigRazorTemplate(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
    {
        //This path work only when run projectWeb.exe in the built folder
        var templatePath = Path.Combine(env.ContentRootPath, "Extensions/FluentEmail/UIEmail");

        //This will prevent compile error if it doesn't exist in projectWeb folder.
        if (!Directory.Exists(templatePath))
            Directory.CreateDirectory(templatePath);//Still empty, must copy files from "bin/Debug/...templatePath" after compile

        services.AddMvcCore().AddRazorRuntimeCompilation();
        services.Configure<MvcRazorRuntimeCompilationOptions>(opts =>
        {
            opts.FileProviders.Add(
                new PhysicalFileProvider(templatePath)
            );
        });
        services.AddRazorTemplating();
        return services;
    }
    public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
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
        });
        return services;
    }
    public static IApplicationBuilder UseCustomCors(this IApplicationBuilder app)
    {
        var allowedOrigins = new[] {
            "http://localhost:5173",
            "http://localhost:3000"
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

    private static IServiceCollection ConfigOpenAI(this IServiceCollection services, IConfiguration configuration)
    {
        //strong-typed config
        /*prevent compile error if missing appsettings vars*/
        var oaiOpts = new OpenAIOptionModel();
        configuration.GetSection("OpenAI").Bind(oaiOpts);

        //new instance OpenAIOptionModel
        services.AddSingleton(oaiOpts);

        //new instance ChatClient
        services.AddSingleton(new ChatClient(oaiOpts.ChatModel, oaiOpts.ApiKey));
        //new instance EmbeddingClient
        services.AddSingleton(new EmbeddingClient(oaiOpts.EmbeddingModel, oaiOpts.ApiKey));

        services.AddSingleton(new ChatCompletionOptions { Temperature = 0.7f, MaxOutputTokenCount = 8100 });

        //new instance TokenizerProvider
        services.AddSingleton<ITokenizerProvider, TokenizerProvider>();

        return services;
    }
}