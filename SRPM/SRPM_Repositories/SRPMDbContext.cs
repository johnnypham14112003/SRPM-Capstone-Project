using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SRPM_Repositories.Models;
using Task = SRPM_Repositories.Models.Task;

namespace SRPM_Repositories;

public class SRPMDbContext : DbContext
{
    public SRPMDbContext(DbContextOptions<SRPMDbContext> options) : base(options)
    {
    }
    //Binding Models
    public DbSet<Field> Fields { get; set; }
    public DbSet<Major> Majors { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<OTPCode> OTPCodes { get; set; }
    public DbSet<SystemConfiguration> SystemConfigurations { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ResearchPaper> ResearchPapers { get; set; }
    public DbSet<ProjectTag> ProjectTags { get; set; }
    public DbSet<ProjectMajor> ProjectMajors { get; set; }
    public DbSet<Milestone> Milestones { get; set; }
    public DbSet<Task> Tasks { get; set; }
    public DbSet<MemberTask> MemberTasks { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<DocumentField> DocumentFields { get; set; }
    public DbSet<FieldContent> FieldContents { get; set; }
    public DbSet<ContentTable> ContentTables { get; set; }
    public DbSet<AppraisalCouncil> AppraisalCouncils { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Evaluation> Evaluations { get; set; }
    public DbSet<EvaluationStage> EvaluationStages { get; set; }
    public DbSet<IndividualEvaluation> IndividualEvaluations { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    private string GetConnectionString()
    {
        string root = Directory.GetParent(Directory.GetCurrentDirectory())?.FullName ?? "";
        string apiDirectory = Path.Combine(root, "SRPM_APIServices");
        IConfiguration configuration = new ConfigurationBuilder()
            //.SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
            .SetBasePath(apiDirectory)
            .AddJsonFile("appsettings.json", true, true).Build();
        return configuration["ConnectionStrings:DefaultConnection"]!;
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(GetConnectionString());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProjectMajor>()
            .HasKey(pm => new { pm.ProjectId, pm.MajorId });

        modelBuilder.Entity<Transaction>()
            .HasIndex(t => t.Code)
            .IsUnique();

        modelBuilder.Entity<Major>()
            .HasOne(m => m.Field)
            .WithMany(f => f.Majors)
            .HasForeignKey(m => m.FieldId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Account>()
            .HasOne(a => a.Major)
            .WithMany(m => m.Accounts)
            .HasForeignKey(a => a.MajorId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Project>()
            .HasOne(p => p.HostInstitution)
            .WithMany(a => a.ProjectsAsHost)
            .HasForeignKey(p => p.HostInstitutionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Project>()
            .HasOne(p => p.CreatedByAccount)
            .WithMany(a => a.CreatedProjects)
            .HasForeignKey(p => p.CreateBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Milestone>()
            .HasOne(m => m.Project)
            .WithMany(p => p.Milestones)
            .HasForeignKey(m => m.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Milestone>()
            .HasOne(m => m.CreateByAccount)
            .WithMany(a => a.CreatedMilestones)
            .HasForeignKey(m => m.CreateBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Task>()
            .HasOne(t => t.Milestone)
            .WithMany(m => m.Tasks)
            .HasForeignKey(t => t.MilestoneId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Task>()
            .HasOne(t => t.CreateByAccount)
            .WithMany(a => a.CreatedTasks)
            .HasForeignKey(t => t.CreateBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MemberTask>()
            .HasOne(mt => mt.Task)
            .WithMany(t => t.MemberTasks)
            .HasForeignKey(mt => mt.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MemberTask>()
            .HasOne(mt => mt.Member)
            .WithMany(a => a.MemberTasks)
            .HasForeignKey(mt => mt.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Document>()
            .HasOne(d => d.UploaderAccount)
            .WithMany(a => a.UploadedDocuments)
            .HasForeignKey(d => d.Uploader)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Document>()
            .HasOne(d => d.Project)
            .WithMany(p => p.Documents)
            .HasForeignKey(d => d.ProjectId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<DocumentField>()
            .HasOne(df => df.Document)
            .WithMany(d => d.DocumentFields)
            .HasForeignKey(df => df.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FieldContent>()
            .HasOne(fc => fc.DocumentField)
            .WithMany(df => df.FieldContents)
            .HasForeignKey(fc => fc.DocumentFieldId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ContentTable>()
            .HasOne(ct => ct.FieldContent)
            .WithMany(fc => fc.ContentTables)
            .HasForeignKey(ct => ct.FieldContentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Account)
            .WithMany(a => a.UserRoles)
            .HasForeignKey(ur => ur.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Project)
            .WithMany(p => p.UserRoles)
            .HasForeignKey(ur => ur.ProjectId)
            .OnDelete(DeleteBehavior.SetNull);


        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Council)
            .WithMany(c => c.UserRoles)
            .HasForeignKey(ur => ur.CouncilId)
            .OnDelete(DeleteBehavior.SetNull);


        modelBuilder.Entity<Evaluation>()
            .HasOne(e => e.Council)
            .WithMany(c => c.Evaluations)
            .HasForeignKey(e => e.CouncilId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Evaluation>()
            .HasOne(e => e.Project)
            .WithMany(p => p.Evaluations)
            .HasForeignKey(e => e.ProjectId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Evaluation>()
            .HasOne(e => e.Milestone)
            .WithMany(m => m.Evaluations)
            .HasForeignKey(e => e.MilestoneId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Evaluation>()
            .HasOne(e => e.FinalDoc)
            .WithMany()
            .HasForeignKey(e => e.FinalDocId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<EvaluationStage>()
            .HasOne(es => es.Evaluation)
            .WithMany(e => e.EvaluationStages)
            .HasForeignKey(es => es.EvaluationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<IndividualEvaluation>()
            .HasOne(ie => ie.EvaluationStage)
            .WithMany(es => es.IndividualEvaluations)
            .HasForeignKey(ie => ie.EvaluationStageId)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<IndividualEvaluation>()
            .HasOne(ie => ie.Reviewer)
            .WithMany()
            .HasForeignKey(ie => ie.ReviewerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<IndividualEvaluation>()
            .HasOne(ie => ie.Project)
            .WithMany()
            .HasForeignKey(ie => ie.ProjectId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<IndividualEvaluation>()
            .HasOne(ie => ie.Milestone)
            .WithMany()
            .HasForeignKey(ie => ie.MilestoneId)
            .OnDelete(DeleteBehavior.NoAction);

        // Transaction relationships
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.RequestPerson)
            .WithMany()
            .HasForeignKey(t => t.RequestPersonId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.HandlePerson)
            .WithMany()
            .HasForeignKey(t => t.HandlePersonId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Project)
            .WithMany()
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.EvaluationStage)
            .WithMany(es => es.Transactions)
            .HasForeignKey(t => t.EvaluationStageId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.FundRequestDoc)
            .WithMany()
            .HasForeignKey(t => t.FundRequestDocId)
            .OnDelete(DeleteBehavior.SetNull);


        modelBuilder.Entity<Notification>()
            .HasOne(n => n.Transaction)
            .WithMany(t => t.Notifications)
            .HasForeignKey(n => n.TransactionId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.IndividualEvaluation)
            .WithMany(ie => ie.Notifications)
            .HasForeignKey(n => n.IndividualEvaluationId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.EvaluationStage)
            .WithMany(es => es.Notifications)
            .HasForeignKey(n => n.EvaluationStageId)
            .OnDelete(DeleteBehavior.SetNull);



        modelBuilder.Entity<Notification>()
            .HasOne(n => n.Evaluation)
            .WithMany(e => e.Notifications)
            .HasForeignKey(n => n.EvaluationId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.GroupUser)
            .WithMany()
            .HasForeignKey(n => n.GroupUserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.Document)
            .WithMany(d => d.Notifications)
            .HasForeignKey(n => n.DocumentId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.MemberTask)
            .WithMany(mt => mt.Notifications)
            .HasForeignKey(n => n.MemberTaskId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.Task)
            .WithMany()
            .HasForeignKey(n => n.TaskId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.SystemConfiguration)
            .WithMany(sc => sc.Notifications)
            .HasForeignKey(n => n.SystemConfigurationId)
            .OnDelete(DeleteBehavior.SetNull);


        modelBuilder.Entity<Project>()
            .Property(p => p.Progress)
            .HasColumnType("decimal(5,2)");

        modelBuilder.Entity<Project>()
            .Property(p => p.Budget)
            .HasColumnType("money");

        modelBuilder.Entity<Task>()
            .Property(t => t.Progress)
            .HasColumnType("decimal(5,2)");

        modelBuilder.Entity<Milestone>()
            .Property(m => m.Cost)
            .HasColumnType("money");

        modelBuilder.Entity<Transaction>()
            .Property(t => t.FeeCost)
            .HasColumnType("money");

        modelBuilder.Entity<Transaction>()
            .Property(t => t.TotalMoney)
            .HasColumnType("money");
    }
}