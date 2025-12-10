using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApis.Migrations
{
    /// <inheritdoc />
    public partial class Prevention_Of_Dublicate_jobCandidate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JobCandidate_JobOpeningId",
                table: "JobCandidate");

            migrationBuilder.CreateIndex(
                name: "IX_JobCandidate_JobOpeningId_CandidateId",
                table: "JobCandidate",
                columns: new[] { "JobOpeningId", "CandidateId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JobCandidate_JobOpeningId_CandidateId",
                table: "JobCandidate");

            migrationBuilder.CreateIndex(
                name: "IX_JobCandidate_JobOpeningId",
                table: "JobCandidate",
                column: "JobOpeningId");
        }
    }
}
