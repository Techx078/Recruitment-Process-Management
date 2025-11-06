using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApis.Migrations
{
    /// <inheritdoc />
    public partial class changeinJobOpening : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SalryRange",
                table: "JobOpening",
                newName: "SalaryRange");

            migrationBuilder.RenameColumn(
                name: "Requirnment",
                table: "JobOpening",
                newName: "Responsibilities");

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "JobOpening",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Education",
                table: "JobOpening",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Experience",
                table: "JobOpening",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobType",
                table: "JobOpening",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "JobOpening",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Requirement",
                table: "JobOpening",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "jobSkills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobOpeningId = table.Column<int>(type: "int", nullable: false),
                    SkillId = table.Column<int>(type: "int", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_jobSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_jobSkills_JobOpening_JobOpeningId",
                        column: x => x.JobOpeningId,
                        principalTable: "JobOpening",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_jobSkills_Skill_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skill",
                        principalColumn: "SkillId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_jobSkills_JobOpeningId",
                table: "jobSkills",
                column: "JobOpeningId");

            migrationBuilder.CreateIndex(
                name: "IX_jobSkills_SkillId",
                table: "jobSkills",
                column: "SkillId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "jobSkills");

            migrationBuilder.DropColumn(
                name: "Department",
                table: "JobOpening");

            migrationBuilder.DropColumn(
                name: "Education",
                table: "JobOpening");

            migrationBuilder.DropColumn(
                name: "Experience",
                table: "JobOpening");

            migrationBuilder.DropColumn(
                name: "JobType",
                table: "JobOpening");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "JobOpening");

            migrationBuilder.DropColumn(
                name: "Requirement",
                table: "JobOpening");

            migrationBuilder.RenameColumn(
                name: "SalaryRange",
                table: "JobOpening",
                newName: "SalryRange");

            migrationBuilder.RenameColumn(
                name: "Responsibilities",
                table: "JobOpening",
                newName: "Requirnment");
        }
    }
}
