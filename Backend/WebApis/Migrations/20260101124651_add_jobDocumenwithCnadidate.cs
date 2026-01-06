using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApis.Migrations
{
    /// <inheritdoc />
    public partial class add_jobDocumenwithCnadidate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobCandidateDocs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    JobCandidateId = table.Column<int>(nullable: false),
                    JobDocumentId = table.Column<int>(nullable: false),

                    DocumentUrl = table.Column<string>(nullable: false),
                    UploadedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        name: "PK_JobCandidateDocs",
                        columns: x => x.Id);

                    table.ForeignKey(
                        name: "FK_JobCandidateDocs_JobCandidates_JobCandidateId",
                        column: x => x.JobCandidateId,
                        principalTable: "JobCandidate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);

                    table.ForeignKey(
                        name: "FK_JobCandidateDocs_JobDocuments_JobDocumentId",
                        column: x => x.JobDocumentId,
                        principalTable: "JobDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobCandidateDocs_JobCandidateId",
                table: "JobCandidateDocs",
                column: "JobCandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_JobCandidateDocs_JobDocumentId",
                table: "JobCandidateDocs",
                column: "JobDocumentId");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
               name: "JobCandidateDocs");
        }
    }
}
