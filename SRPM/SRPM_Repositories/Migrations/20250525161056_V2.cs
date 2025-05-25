using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SRPM_Repositories.Migrations
{
    /// <inheritdoc />
    public partial class V2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Fields_FieldId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Majors_MajorId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Evaluations_Accounts_CreatorId",
                table: "Evaluations");

            migrationBuilder.DropForeignKey(
                name: "FK_Evaluations_Milestones_MilestoneId",
                table: "Evaluations");

            migrationBuilder.DropForeignKey(
                name: "FK_Evaluations_Projects_ProjectId",
                table: "Evaluations");

            migrationBuilder.DropForeignKey(
                name: "FK_Evaluations_Tasks_TaskId",
                table: "Evaluations");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberTasks_Accounts_MemberId",
                table: "MemberTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberTasks_Tasks_TaskId",
                table: "MemberTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Accounts_ReceiverId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Accounts_SenderId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Evaluations_EvaluationId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_MemberTasks_MemberTaskId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_ProjectTeams_ProjectTeamId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Projects_ProjectId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Transactions_TransactionId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Accounts_HostInstitutionId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_ResearchPapers_Fields_FieldId",
                table: "ResearchPapers");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Projects_ProjectId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "CriteriaEvaluates");

            migrationBuilder.DropTable(
                name: "ProjectFields");

            migrationBuilder.DropTable(
                name: "ProjectTeams");

            migrationBuilder.DropTable(
                name: "Criterias");

            migrationBuilder.DropIndex(
                name: "IX_ResearchPapers_FieldId",
                table: "ResearchPapers");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_FieldId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "DocURL",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "FieldId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "FieldId",
                table: "ResearchPapers",
                newName: "ProjectId");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Projects",
                newName: "EnglishTitle");

            migrationBuilder.RenameColumn(
                name: "SenderId",
                table: "Notifications",
                newName: "UserRoleId");

            migrationBuilder.RenameColumn(
                name: "ReceiverId",
                table: "Notifications",
                newName: "TaskId");

            migrationBuilder.RenameColumn(
                name: "ProjectTeamId",
                table: "Notifications",
                newName: "SystemConfigurationId");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "Notifications",
                newName: "IndividualEvaluationId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_SenderId",
                table: "Notifications",
                newName: "IX_Notifications_UserRoleId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_ReceiverId",
                table: "Notifications",
                newName: "IX_Notifications_TaskId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_ProjectTeamId",
                table: "Notifications",
                newName: "IX_Notifications_SystemConfigurationId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_ProjectId",
                table: "Notifications",
                newName: "IX_Notifications_IndividualEvaluationId");

            migrationBuilder.RenameColumn(
                name: "TaskId",
                table: "Evaluations",
                newName: "FinalDocId");

            migrationBuilder.RenameColumn(
                name: "CreatorId",
                table: "Evaluations",
                newName: "DocumentId");

            migrationBuilder.RenameIndex(
                name: "IX_Evaluations_TaskId",
                table: "Evaluations",
                newName: "IX_Evaluations_FinalDocId");

            migrationBuilder.RenameIndex(
                name: "IX_Evaluations_CreatorId",
                table: "Evaluations",
                newName: "IX_Evaluations_DocumentId");

            migrationBuilder.AlterColumn<string>(
                name: "TransferContent",
                table: "Transactions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalMoney",
                table: "Transactions",
                type: "money",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "SenderName",
                table: "Transactions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "SenderBankName",
                table: "Transactions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "SenderAccount",
                table: "Transactions",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ReceiverName",
                table: "Transactions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ReceiverBankName",
                table: "Transactions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ReceiverAccount",
                table: "Transactions",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<decimal>(
                name: "FeeCost",
                table: "Transactions",
                type: "money",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AddColumn<Guid>(
                name: "DocumentId",
                table: "Transactions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EvaluationStageId",
                table: "Transactions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FundRequestDocId",
                table: "Transactions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreateBy",
                table: "Tasks",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "ResearchPapers",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "ProviderName",
                table: "ResearchPapers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "ResearchPapers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ProjectTags",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<decimal>(
                name: "Budget",
                table: "Projects",
                type: "money",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Projects",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "CreateBy",
                table: "Projects",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Projects",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Projects",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Projects",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "DocumentId",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EvaluationStageId",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "GroupUserId",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Cost",
                table: "Milestones",
                type: "money",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AddColumn<Guid>(
                name: "CreateBy",
                table: "Milestones",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Milestones",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryDate",
                table: "MemberTasks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "JoinedAt",
                table: "MemberTasks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "MemberTasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Overdue",
                table: "MemberTasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Progress",
                table: "MemberTasks",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "MemberTasks",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "FieldId",
                table: "Majors",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<byte>(
                name: "TotalRate",
                table: "Evaluations",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProjectId",
                table: "Evaluations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "Evaluations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "CouncilId",
                table: "Evaluations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Evaluations",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "ProficiencyLevel",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<Guid>(
                name: "MajorId",
                table: "Accounts",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<bool>(
                name: "Gender",
                table: "Accounts",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Degree",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.CreateTable(
                name: "AppraisalCouncils",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppraisalCouncils", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HaveHeader = table.Column<bool>(type: "bit", nullable: true),
                    Header = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HeaderAlign = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    HeaderStyle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SubHeader = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubHeaderAlign = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    SubHeaderStyle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TitleAlign = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    TitleStyle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Subtitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubTitleAlign = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    SubTitleStyle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IsSigned = table.Column<bool>(type: "bit", nullable: true),
                    DateInDoc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UploadAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Uploader = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_Accounts_Uploader",
                        column: x => x.Uploader,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Documents_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "EvaluationStages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    StageOrder = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    EvaluationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluationStages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EvaluationStages_Evaluations_EvaluationId",
                        column: x => x.EvaluationId,
                        principalTable: "Evaluations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    IsGroupRole = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConfigKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConfigValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConfigType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentFields",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Chapter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChapterAlign = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ChapterStyle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TitleAlign = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    TitleStyle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Subtitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubTitleAlign = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    SubTitleStyle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IndexInDoc = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentFields_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IndividualEvaluations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalRate = table.Column<byte>(type: "tinyint", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    EvaluationStageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MilestoneId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndividualEvaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IndividualEvaluations_Accounts_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IndividualEvaluations_EvaluationStages_EvaluationStageId",
                        column: x => x.EvaluationStageId,
                        principalTable: "EvaluationStages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IndividualEvaluations_Milestones_MilestoneId",
                        column: x => x.MilestoneId,
                        principalTable: "Milestones",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IndividualEvaluations_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CouncilId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRoles_AppraisalCouncils_CouncilId",
                        column: x => x.CouncilId,
                        principalTable: "AppraisalCouncils",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_UserRoles_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FieldContents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TitleAlign = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    TitleStyle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContentAlign = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ContentStyle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IndexInField = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentFieldId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FieldContents_DocumentFields_DocumentFieldId",
                        column: x => x.DocumentFieldId,
                        principalTable: "DocumentFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContentTables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ColumnIndex = table.Column<int>(type: "int", nullable: false),
                    RowIndex = table.Column<int>(type: "int", nullable: false),
                    ColumnTitle = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ColumnTitleAlign = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ColumnTitleStyle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SubColumnTitle = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SubColumnTitleAlign = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    SubColumnTitleStyle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CellContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CellContentAlign = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    CellContentStyle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FieldContentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentTables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentTables_FieldContents_FieldContentId",
                        column: x => x.FieldContentId,
                        principalTable: "FieldContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_Code",
                table: "Transactions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_DocumentId",
                table: "Transactions",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_EvaluationStageId",
                table: "Transactions",
                column: "EvaluationStageId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_FundRequestDocId",
                table: "Transactions",
                column: "FundRequestDocId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CreateBy",
                table: "Tasks",
                column: "CreateBy");

            migrationBuilder.CreateIndex(
                name: "IX_ResearchPapers_ProjectId",
                table: "ResearchPapers",
                column: "ProjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CreateBy",
                table: "Projects",
                column: "CreateBy");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_DocumentId",
                table: "Notifications",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_EvaluationStageId",
                table: "Notifications",
                column: "EvaluationStageId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_GroupUserId",
                table: "Notifications",
                column: "GroupUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Milestones_CreateBy",
                table: "Milestones",
                column: "CreateBy");

            migrationBuilder.CreateIndex(
                name: "IX_Majors_FieldId",
                table: "Majors",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluations_CouncilId",
                table: "Evaluations",
                column: "CouncilId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentTables_FieldContentId",
                table: "ContentTables",
                column: "FieldContentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFields_DocumentId",
                table: "DocumentFields",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ProjectId",
                table: "Documents",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_Uploader",
                table: "Documents",
                column: "Uploader");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationStages_EvaluationId",
                table: "EvaluationStages",
                column: "EvaluationId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldContents_DocumentFieldId",
                table: "FieldContents",
                column: "DocumentFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_IndividualEvaluations_EvaluationStageId",
                table: "IndividualEvaluations",
                column: "EvaluationStageId");

            migrationBuilder.CreateIndex(
                name: "IX_IndividualEvaluations_MilestoneId",
                table: "IndividualEvaluations",
                column: "MilestoneId");

            migrationBuilder.CreateIndex(
                name: "IX_IndividualEvaluations_ProjectId",
                table: "IndividualEvaluations",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_IndividualEvaluations_ReviewerId",
                table: "IndividualEvaluations",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_AccountId",
                table: "UserRoles",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_CouncilId",
                table: "UserRoles",
                column: "CouncilId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_ProjectId",
                table: "UserRoles",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Majors_MajorId",
                table: "Accounts",
                column: "MajorId",
                principalTable: "Majors",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Evaluations_AppraisalCouncils_CouncilId",
                table: "Evaluations",
                column: "CouncilId",
                principalTable: "AppraisalCouncils",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Evaluations_Documents_DocumentId",
                table: "Evaluations",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Evaluations_Documents_FinalDocId",
                table: "Evaluations",
                column: "FinalDocId",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Evaluations_Milestones_MilestoneId",
                table: "Evaluations",
                column: "MilestoneId",
                principalTable: "Milestones",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Evaluations_Projects_ProjectId",
                table: "Evaluations",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Majors_Fields_FieldId",
                table: "Majors",
                column: "FieldId",
                principalTable: "Fields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MemberTasks_Accounts_MemberId",
                table: "MemberTasks",
                column: "MemberId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MemberTasks_Tasks_TaskId",
                table: "MemberTasks",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Milestones_Accounts_CreateBy",
                table: "Milestones",
                column: "CreateBy",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Documents_DocumentId",
                table: "Notifications",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_EvaluationStages_EvaluationStageId",
                table: "Notifications",
                column: "EvaluationStageId",
                principalTable: "EvaluationStages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Evaluations_EvaluationId",
                table: "Notifications",
                column: "EvaluationId",
                principalTable: "Evaluations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_IndividualEvaluations_IndividualEvaluationId",
                table: "Notifications",
                column: "IndividualEvaluationId",
                principalTable: "IndividualEvaluations",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_MemberTasks_MemberTaskId",
                table: "Notifications",
                column: "MemberTaskId",
                principalTable: "MemberTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_SystemConfigurations_SystemConfigurationId",
                table: "Notifications",
                column: "SystemConfigurationId",
                principalTable: "SystemConfigurations",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Tasks_TaskId",
                table: "Notifications",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Transactions_TransactionId",
                table: "Notifications",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_UserRoles_GroupUserId",
                table: "Notifications",
                column: "GroupUserId",
                principalTable: "UserRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_UserRoles_UserRoleId",
                table: "Notifications",
                column: "UserRoleId",
                principalTable: "UserRoles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Accounts_CreateBy",
                table: "Projects",
                column: "CreateBy",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Accounts_HostInstitutionId",
                table: "Projects",
                column: "HostInstitutionId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ResearchPapers_Projects_ProjectId",
                table: "ResearchPapers",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Accounts_CreateBy",
                table: "Tasks",
                column: "CreateBy",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Documents_DocumentId",
                table: "Transactions",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Documents_FundRequestDocId",
                table: "Transactions",
                column: "FundRequestDocId",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_EvaluationStages_EvaluationStageId",
                table: "Transactions",
                column: "EvaluationStageId",
                principalTable: "EvaluationStages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Projects_ProjectId",
                table: "Transactions",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Majors_MajorId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Evaluations_AppraisalCouncils_CouncilId",
                table: "Evaluations");

            migrationBuilder.DropForeignKey(
                name: "FK_Evaluations_Documents_DocumentId",
                table: "Evaluations");

            migrationBuilder.DropForeignKey(
                name: "FK_Evaluations_Documents_FinalDocId",
                table: "Evaluations");

            migrationBuilder.DropForeignKey(
                name: "FK_Evaluations_Milestones_MilestoneId",
                table: "Evaluations");

            migrationBuilder.DropForeignKey(
                name: "FK_Evaluations_Projects_ProjectId",
                table: "Evaluations");

            migrationBuilder.DropForeignKey(
                name: "FK_Majors_Fields_FieldId",
                table: "Majors");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberTasks_Accounts_MemberId",
                table: "MemberTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberTasks_Tasks_TaskId",
                table: "MemberTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Milestones_Accounts_CreateBy",
                table: "Milestones");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Documents_DocumentId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_EvaluationStages_EvaluationStageId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Evaluations_EvaluationId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_IndividualEvaluations_IndividualEvaluationId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_MemberTasks_MemberTaskId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_SystemConfigurations_SystemConfigurationId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Tasks_TaskId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Transactions_TransactionId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_UserRoles_GroupUserId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_UserRoles_UserRoleId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Accounts_CreateBy",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Accounts_HostInstitutionId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_ResearchPapers_Projects_ProjectId",
                table: "ResearchPapers");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Accounts_CreateBy",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Documents_DocumentId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Documents_FundRequestDocId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_EvaluationStages_EvaluationStageId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Projects_ProjectId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "ContentTables");

            migrationBuilder.DropTable(
                name: "IndividualEvaluations");

            migrationBuilder.DropTable(
                name: "SystemConfigurations");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "FieldContents");

            migrationBuilder.DropTable(
                name: "EvaluationStages");

            migrationBuilder.DropTable(
                name: "AppraisalCouncils");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "DocumentFields");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_Code",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_DocumentId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_EvaluationStageId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_FundRequestDocId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_CreateBy",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_ResearchPapers_ProjectId",
                table: "ResearchPapers");

            migrationBuilder.DropIndex(
                name: "IX_Projects_CreateBy",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_DocumentId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_EvaluationStageId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_GroupUserId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Milestones_CreateBy",
                table: "Milestones");

            migrationBuilder.DropIndex(
                name: "IX_Majors_FieldId",
                table: "Majors");

            migrationBuilder.DropIndex(
                name: "IX_Evaluations_CouncilId",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "EvaluationStageId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "FundRequestDocId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "CreateBy",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "CreateBy",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "EvaluationStageId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "GroupUserId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "CreateBy",
                table: "Milestones");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Milestones");

            migrationBuilder.DropColumn(
                name: "DeliveryDate",
                table: "MemberTasks");

            migrationBuilder.DropColumn(
                name: "JoinedAt",
                table: "MemberTasks");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "MemberTasks");

            migrationBuilder.DropColumn(
                name: "Overdue",
                table: "MemberTasks");

            migrationBuilder.DropColumn(
                name: "Progress",
                table: "MemberTasks");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "MemberTasks");

            migrationBuilder.DropColumn(
                name: "FieldId",
                table: "Majors");

            migrationBuilder.DropColumn(
                name: "CouncilId",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Evaluations");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "ResearchPapers",
                newName: "FieldId");

            migrationBuilder.RenameColumn(
                name: "EnglishTitle",
                table: "Projects",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "UserRoleId",
                table: "Notifications",
                newName: "SenderId");

            migrationBuilder.RenameColumn(
                name: "TaskId",
                table: "Notifications",
                newName: "ReceiverId");

            migrationBuilder.RenameColumn(
                name: "SystemConfigurationId",
                table: "Notifications",
                newName: "ProjectTeamId");

            migrationBuilder.RenameColumn(
                name: "IndividualEvaluationId",
                table: "Notifications",
                newName: "ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_UserRoleId",
                table: "Notifications",
                newName: "IX_Notifications_SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_TaskId",
                table: "Notifications",
                newName: "IX_Notifications_ReceiverId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_SystemConfigurationId",
                table: "Notifications",
                newName: "IX_Notifications_ProjectTeamId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_IndividualEvaluationId",
                table: "Notifications",
                newName: "IX_Notifications_ProjectId");

            migrationBuilder.RenameColumn(
                name: "FinalDocId",
                table: "Evaluations",
                newName: "TaskId");

            migrationBuilder.RenameColumn(
                name: "DocumentId",
                table: "Evaluations",
                newName: "CreatorId");

            migrationBuilder.RenameIndex(
                name: "IX_Evaluations_FinalDocId",
                table: "Evaluations",
                newName: "IX_Evaluations_TaskId");

            migrationBuilder.RenameIndex(
                name: "IX_Evaluations_DocumentId",
                table: "Evaluations",
                newName: "IX_Evaluations_CreatorId");

            migrationBuilder.AlterColumn<string>(
                name: "TransferContent",
                table: "Transactions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalMoney",
                table: "Transactions",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "money");

            migrationBuilder.AlterColumn<string>(
                name: "SenderName",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SenderBankName",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SenderAccount",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReceiverName",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReceiverBankName",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReceiverAccount",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "FeeCost",
                table: "Transactions",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "money");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "ResearchPapers",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProviderName",
                table: "ResearchPapers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "ResearchPapers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ProjectTags",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Budget",
                table: "Projects",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "money");

            migrationBuilder.AddColumn<string>(
                name: "DocURL",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<decimal>(
                name: "Cost",
                table: "Milestones",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "money");

            migrationBuilder.AlterColumn<int>(
                name: "TotalRate",
                table: "Evaluations",
                type: "int",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProjectId",
                table: "Evaluations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "Evaluations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProficiencyLevel",
                table: "Accounts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<Guid>(
                name: "MajorId",
                table: "Accounts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Gender",
                table: "Accounts",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Degree",
                table: "Accounts",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "FieldId",
                table: "Accounts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Accounts",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Criterias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsEvaluation = table.Column<bool>(type: "bit", nullable: false),
                    MailDomain = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Criterias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectFields",
                columns: table => new
                {
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FieldId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectFields", x => new { x.ProjectId, x.FieldId });
                    table.ForeignKey(
                        name: "FK_ProjectFields_Fields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "Fields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectFields_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectTeams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsLeader = table.Column<bool>(type: "bit", nullable: false),
                    IsPrincipal = table.Column<bool>(type: "bit", nullable: false),
                    IsSecretary = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTeams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectTeams_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectTeams_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CriteriaEvaluates",
                columns: table => new
                {
                    CriteriaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EvaluationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriteriaEvaluates", x => new { x.CriteriaId, x.EvaluationId });
                    table.ForeignKey(
                        name: "FK_CriteriaEvaluates_Criterias_CriteriaId",
                        column: x => x.CriteriaId,
                        principalTable: "Criterias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CriteriaEvaluates_Evaluations_EvaluationId",
                        column: x => x.EvaluationId,
                        principalTable: "Evaluations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResearchPapers_FieldId",
                table: "ResearchPapers",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_FieldId",
                table: "Accounts",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_CriteriaEvaluates_EvaluationId",
                table: "CriteriaEvaluates",
                column: "EvaluationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectFields_FieldId",
                table: "ProjectFields",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTeams_AccountId",
                table: "ProjectTeams",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTeams_ProjectId",
                table: "ProjectTeams",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Fields_FieldId",
                table: "Accounts",
                column: "FieldId",
                principalTable: "Fields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Majors_MajorId",
                table: "Accounts",
                column: "MajorId",
                principalTable: "Majors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Evaluations_Accounts_CreatorId",
                table: "Evaluations",
                column: "CreatorId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Evaluations_Milestones_MilestoneId",
                table: "Evaluations",
                column: "MilestoneId",
                principalTable: "Milestones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Evaluations_Projects_ProjectId",
                table: "Evaluations",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Evaluations_Tasks_TaskId",
                table: "Evaluations",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MemberTasks_Accounts_MemberId",
                table: "MemberTasks",
                column: "MemberId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MemberTasks_Tasks_TaskId",
                table: "MemberTasks",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Accounts_ReceiverId",
                table: "Notifications",
                column: "ReceiverId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Accounts_SenderId",
                table: "Notifications",
                column: "SenderId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Evaluations_EvaluationId",
                table: "Notifications",
                column: "EvaluationId",
                principalTable: "Evaluations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_MemberTasks_MemberTaskId",
                table: "Notifications",
                column: "MemberTaskId",
                principalTable: "MemberTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_ProjectTeams_ProjectTeamId",
                table: "Notifications",
                column: "ProjectTeamId",
                principalTable: "ProjectTeams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Projects_ProjectId",
                table: "Notifications",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Transactions_TransactionId",
                table: "Notifications",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Accounts_HostInstitutionId",
                table: "Projects",
                column: "HostInstitutionId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ResearchPapers_Fields_FieldId",
                table: "ResearchPapers",
                column: "FieldId",
                principalTable: "Fields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Projects_ProjectId",
                table: "Transactions",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
