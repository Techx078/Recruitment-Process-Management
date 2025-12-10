using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApis.Migrations
{
    /// <inheritdoc />
    public partial class add_reviewer_id_and_round_number_toJobCandidate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReviewerId",
                table: "JobCandidate",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoundNumber",
                table: "JobCandidate",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_JobCandidate_ReviewerId",
                table: "JobCandidate",
                column: "ReviewerId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobCandidate_Reviewers_ReviewerId",
                table: "JobCandidate",
                column: "ReviewerId",
                principalTable: "Reviewers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobCandidate_Reviewers_ReviewerId",
                table: "JobCandidate");

            migrationBuilder.DropIndex(
                name: "IX_JobCandidate_ReviewerId",
                table: "JobCandidate");

            migrationBuilder.DropColumn(
                name: "ReviewerId",
                table: "JobCandidate");

            migrationBuilder.DropColumn(
                name: "RoundNumber",
                table: "JobCandidate");
        }
    }
}
