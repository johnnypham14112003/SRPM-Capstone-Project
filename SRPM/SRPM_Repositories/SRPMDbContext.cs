using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SRPM_Repositories.Models;

namespace SRPM_Repositories;

public class SRPMDbContext : DbContext
{
    //Constructor
    public SRPMDbContext() { }
    public SRPMDbContext(DbContextOptions<SRPMDbContext> options) : base(options) { }

    //Binding Models
    public DbSet<Account> Account { get; set; }
    public DbSet<AccountNotification> AccountNotification { get; set; }
    public DbSet<AppraisalCouncil> AppraisalCouncil { get; set; }
    public DbSet<Document> Document { get; set; }
    public DbSet<Evaluation> Evaluation { get; set; }
    public DbSet<EvaluationStage> EvaluationStage { get; set; }
    public DbSet<Field> Field { get; set; }
    public DbSet<IndividualEvaluation> IndividualEvaluation { get; set; }
    public DbSet<Major> Major { get; set; }
    public DbSet<MemberTask> MemberTask { get; set; }
    public DbSet<Milestone> Milestone { get; set; }
    public DbSet<Notification> Notification { get; set; }
    public DbSet<OTPCode> OTPCode { get; set; }
    public DbSet<Project> Project { get; set; }
    public DbSet<ProjectMajor> ProjectMajor { get; set; }
    public DbSet<ProjectSimilarity> ProjectSimilarity { get; set; }
    public DbSet<ProjectTag> ProjectTag { get; set; }
    public DbSet<ProjectResult> ProjectResult { get; set; }
    public DbSet<ResultPublish> ResultPublish { get; set; }
    public DbSet<Role> Role { get; set; }
    public DbSet<Signature> Signature { get; set; }
    public DbSet<SystemConfiguration> SystemConfiguration { get; set; }
    public DbSet<Models.Task> Task { get; set; }
    public DbSet<Transaction> Transaction { get; set; }
    public DbSet<UserRole> UserRole { get; set; }

    private static string GetConnectionString()
    {
        string root = Directory.GetParent(Directory.GetCurrentDirectory())?.FullName ?? "";
        string apiDirectory = Path.Combine(root, "SRPM_APIServices");
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(apiDirectory)
            .AddJsonFile("appsettings.json", true, true).Build();
        return configuration["ConnectionStrings:DefaultConnection"]!;
    }

    //private string GetConnectionString()
    //{
    //    IConfiguration configuration = new ConfigurationBuilder()
    //        .SetBasePath(AppContext.BaseDirectory)
    //        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    //        .Build();

    //    return configuration["ConnectionStrings:DefaultConnection"]!;
    //}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(GetConnectionString());
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //Account
        modelBuilder.Entity<Account>(acc =>
        {
            acc.HasOne(a => a.Major)
            .WithMany(m => m.Accounts)
            .HasForeignKey(a => a.MajorId)
            .OnDelete(DeleteBehavior.SetNull); // when delete Major - setNull in Account
            //xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            acc.HasMany(a => a.OTPCodes)
            .WithOne(o => o.Account)
            .HasForeignKey(o => o.AccountId)
            .OnDelete(DeleteBehavior.Cascade); // when delete Account - cascade OTP

            acc.HasMany(a => a.UserRoles)
           .WithOne(u => u.Account)
           .HasForeignKey(u => u.AccountId)
           .OnDelete(DeleteBehavior.Restrict);//too many reference to cascade

            acc.HasMany(a => a.AccountNotifications)
            .WithOne(n => n.Account)
            .HasForeignKey(n => n.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
        });

        //AccountNotification
        modelBuilder.Entity<AccountNotification>(accNoti =>
        {
            //Composite Key
            accNoti.HasKey(anModel => new { anModel.AccountId, anModel.NotificationId });

            accNoti.HasOne(an => an.Account)
            .WithMany(a => a.AccountNotifications)
            .HasForeignKey(an => an.AccountId)
            .OnDelete(DeleteBehavior.Cascade);//when delete account - also delete these account notification

            accNoti.HasOne(an => an.Notification)
            .WithMany(n => n.AccountNotifications)
            .HasForeignKey(an => an.NotificationId)
            .OnDelete(DeleteBehavior.Cascade);//when delete noti - also delete these account notification
        });

        //AppraisalCouncil
        modelBuilder.Entity<AppraisalCouncil>(appCoun =>
        {
            appCoun.HasMany(c => c.Notifications)
            .WithOne(n => n.AppraisalCouncil)
            .HasForeignKey(n => n.AppraisalCouncilId)
            .OnDelete(DeleteBehavior.Restrict); //when delete AppraisalCouncil - block if not handle CouncilId in Notifications

            appCoun.HasMany(c => c.Evaluations)
            .WithOne(e => e.AppraisalCouncil)
            .HasForeignKey(e => e.AppraisalCouncilId)
            .OnDelete(DeleteBehavior.Restrict); //when delete AppraisalCouncil - block if not handle CouncilId in Evaluation

            appCoun.HasMany(c => c.EvaluationStages)
            .WithOne(e => e.AppraisalCouncil)
            .HasForeignKey(e => e.AppraisalCouncilId)
            .OnDelete(DeleteBehavior.Restrict); //when delete AppraisalCouncil - block if not handle CouncilId in EvaluationStage

            appCoun.HasMany(c => c.CouncilMembers)
            .WithOne(e => e.AppraisalCouncil)
            .HasForeignKey(e => e.AppraisalCouncilId)
            .OnDelete(DeleteBehavior.Restrict); //when delete AppraisalCouncil - block if not handle CouncilId in UserRole
        });

        //Document
        modelBuilder.Entity<Document>(doc =>
        {
            doc.HasOne(d => d.Uploader)
            .WithMany(ur => ur.UploadedDocuments)
            .HasForeignKey(d => d.UploaderId)
            .OnDelete(DeleteBehavior.Restrict); //when delete Uploader - block if not handle UploaderId in Doc

            doc.HasOne(d => d.Editor)
            .WithMany(ur => ur.ModifiedDocuments)
            .HasForeignKey(d => d.EditorId)
            .OnDelete(DeleteBehavior.Restrict); //when delete Uploader - block if not handle UploaderId in Doc

            doc.HasOne(d => d.Project)
            .WithMany(p => p.Documents)
            .HasForeignKey(d => d.ProjectId)
            .OnDelete(DeleteBehavior.Restrict); //when delete Project - block if not handle Documents of Project

            doc.HasOne(d => d.Evaluation)
            .WithMany(e => e.Documents)
            .HasForeignKey(d => d.EvaluationId)
            .OnDelete(DeleteBehavior.Restrict); //when delete Evaluation - block if not handle Documents of Evaluation

            doc.HasOne(d => d.IndividualEvaluation)
            .WithMany(ie => ie.Documents)
            .HasForeignKey(d => d.IndividualEvaluationId)
            .OnDelete(DeleteBehavior.Restrict); //when delete IndividualEvaluation - block if not handle Documents of IndividualEvaluation

            doc.HasOne(d => d.Transaction)
            .WithMany(tr => tr.Documents)
            .HasForeignKey(d => d.TransactionId)
            .OnDelete(DeleteBehavior.Restrict); //when delete Transaction - block if not handle Documents of Transaction
            //xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            doc.HasMany(d => d.Signatures)
            .WithOne(s => s.Document)
            .HasForeignKey(s => s.DocumentId)
            .OnDelete(DeleteBehavior.Cascade); //when delete Document - cascade Signatures

            doc.HasMany(d => d.Notifications)
            .WithOne(n => n.Document)
            .HasForeignKey(n => n.DocumentId)
            .OnDelete(DeleteBehavior.Restrict);//when delete Document - block if not handle Notifications
        });

        //Evaluation
        modelBuilder.Entity<Evaluation>(eva =>
        {
            eva.HasOne(e => e.Project)
            .WithMany(pro => pro.Evaluations)
            .HasForeignKey(e => e.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

            eva.HasOne(e => e.AppraisalCouncil)
            .WithMany(ac => ac.Evaluations)
            .HasForeignKey(e => e.AppraisalCouncilId)
            .OnDelete(DeleteBehavior.Restrict);
            //xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            eva.HasMany(e => e.Documents)
            .WithOne(d => d.Evaluation)
            .HasForeignKey(d => d.EvaluationId)
            .OnDelete(DeleteBehavior.Restrict);

            eva.HasMany(e => e.EvaluationStages)
            .WithOne(es => es.Evaluation)
            .HasForeignKey(es => es.EvaluationId)
            .OnDelete(DeleteBehavior.Restrict);

            eva.HasMany(df => df.Notifications)
            .WithOne(n => n.Evaluation)
            .HasForeignKey(n => n.EvaluationId)
            .OnDelete(DeleteBehavior.Restrict);
        });

        //EvaluationStage
        modelBuilder.Entity<EvaluationStage>(evas =>
        {
            evas.HasOne(es => es.Evaluation)
            .WithMany(e => e.EvaluationStages)
            .HasForeignKey(es => es.EvaluationId)
            .OnDelete(DeleteBehavior.Restrict);

            evas.HasOne(es => es.Milestone)
            .WithMany(ms => ms.EvaluationStages)
            .HasForeignKey(es => es.MilestoneId)
            .OnDelete(DeleteBehavior.Restrict);

            evas.HasOne(es => es.AppraisalCouncil)
            .WithMany(ac => ac.EvaluationStages)
            .HasForeignKey(ac => ac.AppraisalCouncilId)
            .OnDelete(DeleteBehavior.Restrict);
            //xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            evas.HasMany(es => es.Transactions)
            .WithOne(tr => tr.EvaluationStage)
            .HasForeignKey(tr => tr.EvaluationStageId)
            .OnDelete(DeleteBehavior.Restrict);

            evas.HasMany(es => es.IndividualEvaluations)
            .WithOne(ie => ie.EvaluationStage)
            .HasForeignKey(ie => ie.EvaluationStageId)
            .OnDelete(DeleteBehavior.Restrict);

            evas.HasMany(es => es.Notifications)
            .WithOne(n => n.EvaluationStage)
            .HasForeignKey(n => n.EvaluationStageId)
            .OnDelete(DeleteBehavior.Restrict);
        });

        //Field
        modelBuilder.Entity<Field>(fie =>
        {
            fie.HasMany(f => f.Majors)
            .WithOne(m => m.Field)
            .HasForeignKey(m => m.FieldId)
            .OnDelete(DeleteBehavior.Restrict);
        });

        //IndividualEvaluation
        modelBuilder.Entity<IndividualEvaluation>(inev =>
        {
            inev.HasOne(ie => ie.EvaluationStage)
            .WithMany(es => es.IndividualEvaluations)
            .HasForeignKey(ie => ie.EvaluationStageId)
            .OnDelete(DeleteBehavior.Restrict);

            inev.HasOne(ie => ie.Reviewer)
            .WithMany(ur => ur.IndividualEvaluations)
            .HasForeignKey(ie => ie.ReviewerId)
            .OnDelete(DeleteBehavior.Restrict);

            //xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            inev.HasMany(ie => ie.ProjectsSimilarity)
            .WithOne(pros => pros.IndividualEvaluation)
            .HasForeignKey(pros => pros.IndividualEvaluationId)
            .OnDelete(DeleteBehavior.Cascade);

            inev.HasMany(ie => ie.Documents)
            .WithOne(d => d.IndividualEvaluation)
            .HasForeignKey(d => d.IndividualEvaluationId)
            .OnDelete(DeleteBehavior.Restrict);

            inev.HasMany(ie => ie.Notifications)
            .WithOne(n => n.IndividualEvaluation)
            .HasForeignKey(n => n.IndividualEvaluationId)
            .OnDelete(DeleteBehavior.Restrict);
        });

        //Major
        modelBuilder.Entity<Major>(maj =>
        {
            maj.HasOne(m => m.Field)
            .WithMany(f => f.Majors)
            .HasForeignKey(m => m.FieldId)
            .OnDelete(DeleteBehavior.Restrict);

            maj.HasMany(m => m.Accounts)
            .WithOne(a => a.Major)
            .HasForeignKey(a => a.MajorId)
            .OnDelete(DeleteBehavior.SetNull); // when delete Major - setNull in Account
            //xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            maj.HasMany(m => m.ProjectMajors)
            .WithOne(pm => pm.Major)
            .HasForeignKey(pm => pm.MajorId)
            .OnDelete(DeleteBehavior.Cascade);// when delete Major - cascade in ProjectMajor
        });

        //MemberTask
        modelBuilder.Entity<MemberTask>(memta =>
        {
            //Config Specific ColumnType
            memta.Property(mt => mt.Progress).HasColumnType("DECIMAL(5,2)");

            memta.HasOne(mt => mt.Member)
            .WithMany(ur => ur.MemberTasks)
            .HasForeignKey(mt => mt.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

            memta.HasOne(mt => mt.Task)
            .WithMany(t => t.MemberTasks)
            .HasForeignKey(mt => mt.TaskId)
            .OnDelete(DeleteBehavior.Restrict);
            //xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            memta.HasMany(mt => mt.Notifications)
            .WithOne(n => n.MemberTask)
            .HasForeignKey(n => n.MemberTaskId)
            .OnDelete(DeleteBehavior.Restrict);
        });

        //Milestone
        modelBuilder.Entity<Milestone>(miles =>
        {
            //Config Specific ColumnType
            miles.Property(ms => ms.Cost).HasColumnType("MONEY");

            miles.HasOne(ms => ms.Project)
            .WithMany(pro => pro.Milestones)
            .HasForeignKey(ms => ms.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

            miles.HasOne(ms => ms.Creator)
            .WithMany(ur => ur.CreatedMilestones)
            .HasForeignKey(ms => ms.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);

            //xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            miles.HasMany(ms => ms.EvaluationStages)
            .WithOne(es => es.Milestone)
            .HasForeignKey(es => es.MilestoneId)
            .OnDelete(DeleteBehavior.Restrict);

            miles.HasMany(ms => ms.Tasks)
            .WithOne(t => t.Milestone)
            .HasForeignKey(t => t.MilestoneId)
            .OnDelete(DeleteBehavior.Restrict);
        });

        // Notification
        modelBuilder.Entity<Notification>(noti =>
        {
            noti.HasMany(n => n.AccountNotifications)
            .WithOne(an => an.Notification)
            .HasForeignKey(an => an.NotificationId)
            .OnDelete(DeleteBehavior.Cascade);  // when delete Notification - cascade AccountNotifications
            //xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            noti.HasOne(n => n.Project)
            .WithMany(p => p.Notifications)
            .HasForeignKey(n => n.ProjectId)
            .OnDelete(DeleteBehavior.Restrict); // when delete Document - block if not handle Notifications of Project

            noti.HasOne(n => n.AppraisalCouncil)
            .WithMany(ac => ac.Notifications)
            .HasForeignKey(n => n.AppraisalCouncilId)
            .OnDelete(DeleteBehavior.Restrict); // when delete Document - block if not handle Notifications of AppraisalCouncil

            noti.HasOne(n => n.Document)
            .WithMany(d => d.Notifications)
            .HasForeignKey(n => n.DocumentId)
            .OnDelete(DeleteBehavior.Restrict); // when delete Document - block if not handle Notifications of Document

            noti.HasOne(n => n.Evaluation)
            .WithMany(e => e.Notifications)
            .HasForeignKey(n => n.EvaluationId)
            .OnDelete(DeleteBehavior.Restrict);

            noti.HasOne(n => n.EvaluationStage)
            .WithMany(es => es.Notifications)
            .HasForeignKey(n => n.EvaluationStageId)
            .OnDelete(DeleteBehavior.Restrict);

            noti.HasOne(n => n.IndividualEvaluation)
            .WithMany(ie => ie.Notifications)
            .HasForeignKey(n => n.IndividualEvaluationId)
            .OnDelete(DeleteBehavior.Restrict);

            noti.HasOne(n => n.MemberTask)
            .WithMany(mt => mt.Notifications)
            .HasForeignKey(n => n.MemberTaskId)
            .OnDelete(DeleteBehavior.Restrict);

            noti.HasOne(n => n.Signature)
            .WithMany(s => s.Notifications)
            .HasForeignKey(n => n.SignatureId)
            .OnDelete(DeleteBehavior.Restrict);

            noti.HasOne(n => n.Task)
            .WithMany(t => t.Notifications)
            .HasForeignKey(n => n.TaskId)
            .OnDelete(DeleteBehavior.Restrict);

            noti.HasOne(n => n.Transaction)
            .WithMany(tr => tr.Notifications)
            .HasForeignKey(n => n.TransactionId)
            .OnDelete(DeleteBehavior.Restrict);

            noti.HasOne(n => n.SystemConfiguration)
            .WithMany(sc => sc.Notifications)
            .HasForeignKey(n => n.SystemConfigurationId)
            .OnDelete(DeleteBehavior.Restrict);

            noti.HasOne(n => n.UserRole)
            .WithMany(ur => ur.Notifications)
            .HasForeignKey(n => n.UserRoleId)
            .OnDelete(DeleteBehavior.Restrict);
        });

        //OTPCode
        modelBuilder.Entity<OTPCode>(opCo =>
        {
            opCo.HasOne(oc => oc.Account)
            .WithMany(a => a.OTPCodes)
            .HasForeignKey(oc => oc.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
        });

        //Project
        modelBuilder.Entity<Project>(proj =>
        {
            //Config Specific ColumnType
            proj.Property(p => p.Budget).HasColumnType("MONEY");
            proj.Property(p => p.Progress).HasColumnType("DECIMAL(5,2)");

            proj.HasOne(p => p.Creator)
            .WithMany(ur => ur.CreatedProjects)
            .HasForeignKey(p => p.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);

            proj.HasOne(p => p.ProjectResult)   // 1:1 Relationship
            .WithOne(pr => pr.Project)
            .HasForeignKey<ProjectResult>(pr => pr.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);  //When delete Project - cascade ProjectResult
            //xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            proj.HasMany(p => p.Notifications)
            .WithOne(n => n.Project)
            .HasForeignKey(n => n.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

            proj.HasMany(p => p.Members)
            .WithOne(ur => ur.Project)
            .HasForeignKey(ur => ur.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

            proj.HasMany(p => p.Milestones)
            .WithOne(ms => ms.Project)
            .HasForeignKey(ms => ms.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

            proj.HasMany(p => p.Evaluations)
            .WithOne(e => e.Project)
            .HasForeignKey(e => e.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

            proj.HasMany(p => p.ProjectsSimilarity)
            .WithOne(pros => pros.Project)
            .HasForeignKey(pros => pros.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
            
            proj.HasMany(p => p.ProjectMajors)
            .WithOne(pm => pm.Project)
            .HasForeignKey(pm => pm.ProjectId)
            .OnDelete(DeleteBehavior.Cascade); // When delete project - cascade ProjectMajors

            proj.HasMany(p => p.ProjectTags)
            .WithOne(pt => pt.Project)
            .HasForeignKey(pt => pt.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);// When delete project - cascade ProjectTags

            proj.HasMany(p => p.Documents)
            .WithOne(d => d.Project)
            .HasForeignKey(d => d.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

            proj.HasMany(p => p.Transactions)
            .WithOne(tra => tra.Project)
            .HasForeignKey(tra => tra.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);
        });

        //ProjectMajor
        modelBuilder.Entity<ProjectMajor>(projMaj =>
        {
            //Compsite key
            projMaj.HasKey(pm => new { pm.ProjectId, pm.MajorId });

            projMaj.HasOne(pm => pm.Project)
            .WithMany(p => p.ProjectMajors)
            .HasForeignKey(pm => pm.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);// When delete project - cascade ProjectMajors

            projMaj.HasOne(pm => pm.Major)
            .WithMany(m => m.ProjectMajors)
            .HasForeignKey(pm => pm.MajorId)
            .OnDelete(DeleteBehavior.Cascade);// when delete Major - cascade in ProjectMajor
        });

        //ProjectResult
        modelBuilder.Entity<ProjectResult>(prore =>
        {
            prore.HasOne(pr => pr.Project)   // 1:1 Relationship
            .WithOne(p => p.ProjectResult)
            .HasForeignKey<ProjectResult>(pr => pr.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

            prore.HasMany(pr => pr.ResultPublishs)
            .WithOne(rp => rp.ProjectResult)
            .HasForeignKey(rp => rp.ProjectResultId)
            .OnDelete(DeleteBehavior.Cascade);      // when delete ProjectResult - cascade ResultPublishs
        });

        //ResultPublish
        modelBuilder.Entity<ResultPublish>(prore =>
        {
            prore.HasOne(rp => rp.ProjectResult)
            .WithMany(pr => pr.ResultPublishs)
            .HasForeignKey(rp => rp.ProjectResultId)
            .OnDelete(DeleteBehavior.Cascade);      // when delete ProjectResult - cascade ResultPublishs
        });

        //ProjectSimilarity
        modelBuilder.Entity<ProjectSimilarity>(proSim =>
        {
            proSim.HasKey(proSimModel => new { proSimModel.ProjectId, proSimModel.IndividualEvaluationId });

            proSim.HasOne(ps => ps.Project)
            .WithMany(p => p.ProjectsSimilarity)
            .HasForeignKey(ps => ps.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

            proSim.HasOne(ps => ps.IndividualEvaluation)
            .WithMany(ie => ie.ProjectsSimilarity)
            .HasForeignKey(ps => ps.IndividualEvaluationId)
            .OnDelete(DeleteBehavior.Cascade);
        });

        //ProjectTag
        modelBuilder.Entity<ProjectTag>(projtag =>
        {
            projtag.HasOne(pt => pt.Project)
            .WithMany(p => p.ProjectTags)
            .HasForeignKey(pt => pt.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);// When delete project - cascade ProjectTags
        });

        //Role
        modelBuilder.Entity<Role>(rol =>
        {
            rol.HasMany(r => r.UserRoles)
            .WithOne(ur => ur.Role)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
        });

        //Signature
        modelBuilder.Entity<Signature>(sign =>
        {
            sign.HasOne(s => s.Signer)
            .WithMany(ur => ur.Signatures)
            .HasForeignKey(s => s.SignerId)
            .OnDelete(DeleteBehavior.Restrict);

            sign.HasOne(s => s.Document)
            .WithMany(d => d.Signatures)
            .HasForeignKey(s => s.DocumentId)
            .OnDelete(DeleteBehavior.Restrict);
            //xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            sign.HasMany(s => s.Notifications)
            .WithOne(n => n.Signature)
            .HasForeignKey(n => n.SignatureId)
            .OnDelete(DeleteBehavior.Restrict);
        });

        //SystemConfiguration
        modelBuilder.Entity<SystemConfiguration>(syscf =>
        {
            syscf.HasMany(sc => sc.Notifications)
            .WithOne(n => n.SystemConfiguration)
            .HasForeignKey(n => n.SystemConfigurationId)
            .OnDelete(DeleteBehavior.Restrict);
        });

        //Task
        modelBuilder.Entity<Models.Task>(tas =>
        {
            //Config Specific ColumnType
            tas.Property(t => t.Progress).HasColumnType("DECIMAL(5,2)");
            tas.Property(t => t.Cost).HasColumnType("MONEY");

            tas.HasOne(t => t.Milestone)
            .WithMany(m => m.Tasks)
            .HasForeignKey(t => t.MilestoneId)
            .OnDelete(DeleteBehavior.Restrict);

            tas.HasOne(t => t.Creator)
            .WithMany(ur => ur.CreatedTasks)
            .HasForeignKey(t => t.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);
            //xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            tas.HasMany(t => t.MemberTasks)
            .WithOne(mt => mt.Task)
            .HasForeignKey(mt => mt.TaskId)
            .OnDelete(DeleteBehavior.Restrict);

            tas.HasMany(t => t.Notifications)
            .WithOne(n => n.Task)
            .HasForeignKey(n => n.TaskId)
            .OnDelete(DeleteBehavior.Restrict);
        });

        //Transaction
        modelBuilder.Entity<Transaction>(tran =>
        {
            //Config Specific ColumnType
            tran.Property(tr => tr.FeeCost).HasColumnType("MONEY");
            tran.Property(tr => tr.TotalMoney).HasColumnType("MONEY");

            tran.HasOne(htr => htr.HandlePerson)
            .WithMany(ur => ur.HandleTransactions)
            .HasForeignKey(htr => htr.HandlePersonId)
            .OnDelete(DeleteBehavior.Restrict);

            tran.HasOne(rtr => rtr.RequestPerson)
            .WithMany(ur => ur.RequestTransactions)
            .HasForeignKey(rtr => rtr.RequestPersonId)
            .OnDelete(DeleteBehavior.Restrict);

            tran.HasOne(tr => tr.Project)
            .WithMany(ur => ur.Transactions)
            .HasForeignKey(tr => tr.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

            tran.HasOne(tr => tr.EvaluationStage)
            .WithMany(ur => ur.Transactions)
            .HasForeignKey(tr => tr.EvaluationStageId)
            .OnDelete(DeleteBehavior.Restrict);
            //xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            tran.HasMany(tr => tr.Documents)
            .WithOne(d => d.Transaction)
            .HasForeignKey(d => d.TransactionId)
            .OnDelete(DeleteBehavior.Restrict);

            tran.HasMany(tr => tr.Notifications)
            .WithOne(n => n.Transaction)
            .HasForeignKey(n => n.TransactionId)
            .OnDelete(DeleteBehavior.Restrict);
        });

        // UserRole
        modelBuilder.Entity<UserRole>(user =>
        {
            user.HasOne(ur => ur.Account)
            .WithMany(a => a.UserRoles)
            .HasForeignKey(ur => ur.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

            user.HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

            user.HasOne(ur => ur.Project)
            .WithMany(p => p.Members)
            .HasForeignKey(ur => ur.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

            user.HasOne(ur => ur.AppraisalCouncil)
            .WithMany(apc => apc.CouncilMembers)
            .HasForeignKey(ur => ur.AppraisalCouncilId)
            .OnDelete(DeleteBehavior.Restrict);
            //xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            user.HasMany(ur => ur.UploadedDocuments)
            .WithOne(d => d.Uploader)
            .HasForeignKey(d => d.UploaderId)
            .OnDelete(DeleteBehavior.Restrict);

            user.HasMany(ur => ur.ModifiedDocuments)
            .WithOne(d => d.Editor)
            .HasForeignKey(d => d.EditorId)
            .OnDelete(DeleteBehavior.Restrict);

            user.HasMany(ur => ur.Signatures)
            .WithOne(s => s.Signer)
            .HasForeignKey(s => s.SignerId)
            .OnDelete(DeleteBehavior.Restrict);

            user.HasMany(ur => ur.IndividualEvaluations)
            .WithOne(n => n.Reviewer)
            .HasForeignKey(n => n.ReviewerId)
            .OnDelete(DeleteBehavior.Restrict);

            user.HasMany(ur => ur.CreatedProjects)
            .WithOne(p => p.Creator)
            .HasForeignKey(p => p.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);

            user.HasMany(ur => ur.CreatedMilestones)
            .WithOne(ms => ms.Creator)
            .HasForeignKey(ms => ms.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);

            user.HasMany(ur => ur.CreatedTasks)
            .WithOne(t => t.Creator)
            .HasForeignKey(t => t.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);

            user.HasMany(ur => ur.MemberTasks)
            .WithOne(n => n.Member)
            .HasForeignKey(n => n.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

            user.HasMany(ur => ur.RequestTransactions)
            .WithOne(n => n.RequestPerson)
            .HasForeignKey(n => n.RequestPersonId)
            .OnDelete(DeleteBehavior.Restrict);

            user.HasMany(ur => ur.HandleTransactions)
            .WithOne(n => n.HandlePerson)
            .HasForeignKey(n => n.HandlePersonId)
            .OnDelete(DeleteBehavior.Restrict);

            user.HasMany(ur => ur.Notifications)
            .WithOne(n => n.UserRole)
            .HasForeignKey(n => n.UserRoleId)
            .OnDelete(DeleteBehavior.Restrict);
        });
    }
}