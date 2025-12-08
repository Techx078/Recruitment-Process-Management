using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApis.Migrations
{
    /// <inheritdoc />
    public partial class adddomain_in_jobOpening : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Experience",
                table: "JobOpening");

            migrationBuilder.AddColumn<int>(
                name: "minExperience",
                table: "jobSkills",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Domain",
                table: "JobOpening",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "minDomainExperience",
                table: "JobOpening",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "minExperience",
                table: "jobSkills");

            migrationBuilder.DropColumn(
                name: "Domain",
                table: "JobOpening");

            migrationBuilder.DropColumn(
                name: "minDomainExperience",
                table: "JobOpening");

            migrationBuilder.AddColumn<string>(
                name: "Experience",
                table: "JobOpening",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
