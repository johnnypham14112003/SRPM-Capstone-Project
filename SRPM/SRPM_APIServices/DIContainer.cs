using Mapster;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using SRPM_Repositories;
using SRPM_Repositories.Repositories.Interfaces;
using SRPM_Repositories.Repositories.Repositories;
using SRPM_Services.Extensions;
using SRPM_Services.Interfaces;
using SRPM_Services.Repositories;

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
        //Add other BusinessServices here...

        return services;
    }

    private static IServiceCollection InjectRepository(this IServiceCollection services)
    {
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        //---------------------------------------------------------------------------
        services.AddScoped<IAccountRepository, AccountRepository>();
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
        //services.AddMapster();
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