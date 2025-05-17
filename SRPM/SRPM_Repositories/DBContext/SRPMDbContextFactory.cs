using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SRPM_Repositories.DBContext;
using System.IO;

namespace SRPM_Repositories.DBContext
{
    //public class SRPMDbContextFactory : IDesignTimeDbContextFactory<SRPMDbContext>
    //{
    //    public SRPMDbContext CreateDbContext(string[] args)
    //    {
    //        // Load appsettings.json from the current directory
    //        IConfigurationRoot configuration = new ConfigurationBuilder()
    //            .SetBasePath(Directory.GetCurrentDirectory())
    //            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    //            .Build();

    //        var optionsBuilder = new DbContextOptionsBuilder<SRPMDbContext>();
    //        optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

    //        return new SRPMDbContext(optionsBuilder.Options);
    //    }
    //}
}
