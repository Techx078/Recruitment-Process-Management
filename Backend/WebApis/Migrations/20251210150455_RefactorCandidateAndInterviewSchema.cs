using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApis.Migrations
{
    /// <inheritdoc />
    public partial class RefactorCandidateAndInterviewSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsHrRound",
                table: "JobInterview");

            migrationBuilder.DropColumn(
                name: "IsNextRound",
                table: "JobInterview");

            migrationBuilder.DropColumn(
                name: "IsRejected",
                table: "JobInterview");

            migrationBuilder.AddColumn<bool>(
                name: "IsNextHrRound",
                table: "JobCandidate",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsNextTechnicalRound",
                table: "JobCandidate",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsNextHrRound",
                table: "JobCandidate");

            migrationBuilder.DropColumn(
                name: "IsNextTechnicalRound",
                table: "JobCandidate");

            migrationBuilder.AddColumn<bool>(
                name: "IsHrRound",
                table: "JobInterview",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsNextRound",
                table: "JobInterview",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRejected",
                table: "JobInterview",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
