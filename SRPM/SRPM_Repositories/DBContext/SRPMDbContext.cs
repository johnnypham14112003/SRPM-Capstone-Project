

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SRPM_Repositories.Models;

namespace SRPM_Repositories.DBContext
{
    public class SRPMDbContext : DbContext
    {
        public SRPMDbContext(DbContextOptions<SRPMDbContext> options)
           : base(options)
        {
        }

        // DbSets
        public DbSet<Field> Fields { get; set; }
        public DbSet<Major> Majors { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<ResearchPaper> ResearchPapers { get; set; }
        public DbSet<OTPCode> OTPCodes { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectTag> ProjectTags { get; set; }
        public DbSet<ProjectTeam> ProjectTeams { get; set; }
        public DbSet<ProjectField> ProjectFields { get; set; }
        public DbSet<ProjectMajor> ProjectMajors { get; set; }
        public DbSet<Milestone> Milestones { get; set; }
        public DbSet<Models.Task> Tasks { get; set; }
        public DbSet<MemberTask> MemberTasks { get; set; }
        public DbSet<Criteria> Criterias { get; set; }
        public DbSet<Evaluation> Evaluations { get; set; }
        public DbSet<CriteriaEvaluate> CriteriaEvaluates { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Notification> Notifications { get; set; }


        private string GetConnectionString()
        {
            IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())   //Path.Combine(Directory.GetCurrentDirectory(),"testApi")
            .AddJsonFile("appsettings.json", true, true).Build();
            return configuration["ConnectionStrings:DefaultConnection"];
        }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string root = Directory.GetParent(Directory.GetCurrentDirectory())?.FullName ?? "";
            string apiDirectory = Path.Combine(root, "SRPM_APIServices");
            var configuration = new ConfigurationBuilder()
                .SetBasePath(apiDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString ?? "");

        }
    }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<ProjectField>()
                .HasKey(pf => new { pf.ProjectId, pf.FieldId });

            modelBuilder.Entity<ProjectMajor>()
                .HasKey(pm => new { pm.ProjectId, pm.MajorId });

            modelBuilder.Entity<CriteriaEvaluate>()
                .HasKey(ce => new { ce.CriteriaId, ce.EvaluationId });

            modelBuilder.Entity<Field>()
                .HasMany(f => f.Accounts)
                .WithOne(a => a.Field)
                .HasForeignKey(a => a.FieldId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Field>()
                .HasMany(f => f.ResearchPapers)
                .WithOne(rp => rp.Field)
                .HasForeignKey(rp => rp.FieldId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Field>()
                .HasMany(f => f.ProjectFields)
                .WithOne(pf => pf.Field)
                .HasForeignKey(pf => pf.FieldId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Major>()
                .HasMany(m => m.Accounts)
                .WithOne(a => a.Major)
                .HasForeignKey(a => a.MajorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Major>()
                .HasMany(m => m.ProjectMajors)
                .WithOne(pm => pm.Major)
                .HasForeignKey(pm => pm.MajorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.OTPCodes)
                .WithOne(o => o.Account)
                .HasForeignKey(o => o.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.ResearchPapers)
                .WithOne(rp => rp.PrincipalInvestigator)
                .HasForeignKey(rp => rp.PrincipalInvestigatorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.Evaluations)
                .WithOne(e => e.Creator)
                .HasForeignKey(e => e.CreatorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.ProjectTeams)
                .WithOne(pt => pt.Account)
                .HasForeignKey(pt => pt.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.RequestedTransactions)
                .WithOne(t => t.RequestPerson)
                .HasForeignKey(t => t.RequestPersonId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.HandledTransactions)
                .WithOne(t => t.HandlePerson)
                .HasForeignKey(t => t.HandlePersonId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.ReceivedNotifications)
                .WithOne(n => n.Receiver)
                .HasForeignKey(n => n.ReceiverId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.SentNotifications)
                .WithOne(n => n.Sender)
                .HasForeignKey(n => n.SenderId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Project>()
                .HasMany(p => p.Milestones)
                .WithOne(m => m.Project)
                .HasForeignKey(m => m.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Project>()
                .HasMany(p => p.ProjectTags)
                .WithOne(pt => pt.Project)
                .HasForeignKey(pt => pt.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Project>()
                .HasMany(p => p.ProjectTeams)
                .WithOne(pt => pt.Project)
                .HasForeignKey(pt => pt.ProjectId)
                .OnDelete(DeleteBehavior.NoAction); 


            modelBuilder.Entity<Project>()
                .HasMany(p => p.Transactions)
                .WithOne(t => t.Project)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Project>()
                .HasMany(p => p.ProjectFields)
                .WithOne(pf => pf.Project)
                .HasForeignKey(pf => pf.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Project>()
                .HasMany(p => p.ProjectMajors)
                .WithOne(pm => pm.Project)
                .HasForeignKey(pm => pm.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Project>()
                .HasMany(p => p.Notifications)
                .WithOne(n => n.Project)
                .HasForeignKey(n => n.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectTeam>()
                .HasOne(pt => pt.Account)
                .WithMany(a => a.ProjectTeams)
                .HasForeignKey(pt => pt.AccountId)
                .OnDelete(DeleteBehavior.Cascade); 


            modelBuilder.Entity<Milestone>()
                .HasMany(m => m.Tasks)
                .WithOne(t => t.Milestone)
                .HasForeignKey(t => t.MilestoneId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Milestone>()
                .HasMany(m => m.Evaluations)
                .WithOne(e => e.Milestone)
                .HasForeignKey(e => e.MilestoneId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Models.Task>()
                .HasMany(t => t.MemberTasks)
                .WithOne(mt => mt.Task)
                .HasForeignKey(mt => mt.TaskId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Models.Task>()
                .HasMany(t => t.Evaluations)
                .WithOne(e => e.Task)
                .HasForeignKey(e => e.TaskId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<MemberTask>()
                .HasOne(mt => mt.Member)
                .WithMany(a => a.MemberTasks)
                .HasForeignKey(mt => mt.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MemberTask>()
                .HasMany(mt => mt.Notifications)
                .WithOne(n => n.MemberTask)
                .HasForeignKey(n => n.MemberTaskId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Evaluation>()
                .HasOne(e => e.Creator)
                .WithMany(a => a.Evaluations)
                .HasForeignKey(e => e.CreatorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Evaluation>()
                .HasOne(e => e.Project)
                .WithMany(p => p.Evaluations)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Evaluation>()
                .HasMany(e => e.CriteriaEvaluates)
                .WithOne(ce => ce.Evaluation)
                .HasForeignKey(ce => ce.EvaluationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Evaluation>()
                .HasMany(e => e.Notifications)
                .WithOne(n => n.Evaluation)
                .HasForeignKey(n => n.EvaluationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CriteriaEvaluate>()
                .HasOne(ce => ce.Criteria)
                .WithMany(c => c.CriteriaEvaluates)
                .HasForeignKey(ce => ce.CriteriaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Transaction>()
                .HasMany(t => t.Notifications)
                .WithOne(n => n.Transaction)
                .HasForeignKey(n => n.TransactionId)
                .OnDelete(DeleteBehavior.Restrict);


            // Explicitly set precision and scale for decimal properties
            modelBuilder.Entity<Milestone>()
                .Property(m => m.Cost)
                .HasPrecision(18, 2); 

            modelBuilder.Entity<Project>()
                .Property(p => p.Budget)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Project>()
                .Property(p => p.Progress)
                .HasPrecision(5, 2); 

            modelBuilder.Entity<Models.Task>()
                .Property(t => t.Progress)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.FeeCost)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.TotalMoney)
                .HasPrecision(18, 2);
        }
    }
}
