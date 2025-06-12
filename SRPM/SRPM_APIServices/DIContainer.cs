using Mapster;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using SRPM_Repositories;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Repositories.Repositories.Repositories;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Extensions;
using SRPM_Services.Interfaces;
using SRPM_Services.Repositories;
using SRPM_Services.Extensions.Mapster;

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


        //Third Party Services
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
}