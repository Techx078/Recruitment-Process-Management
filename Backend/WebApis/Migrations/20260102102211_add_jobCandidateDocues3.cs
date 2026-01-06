using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApis.Migrations
{
    /// <inheritdoc />
    public partial class add_jobCandidateDocues3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
        name: "jobCandidateDocs", 
        columns: table => new
        {
            Id = table.Column<int>(type: "int", nullable: false)
                .Annotation("SqlServer:Identity", "1, 1"),
            JobCandidateId = table.Column<int>(type: "int", nullable: false),
            JobDocumentId = table.Column<int>(type: "int", nullable: false),
            DocumentUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
            UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
        },
        constraints: table =>
        {
            table.PrimaryKey("PK_jobCandidateDocs", x => x.Id);

            table.ForeignKey(
                name: "FK_jobCandidateDocs_JobCandidates_JobCandidateId",
                column: x => x.JobCandidateId,
                principalTable: "JobCandidate", 
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

           
            table.ForeignKey(
                name: "FK_jobCandidateDocs_JobDocuments_JobDocumentId",
                column: x => x.JobDocumentId,
                principalTable: "JobDocuments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        });

           
            migrationBuilder.CreateIndex(
                name: "IX_jobCandidateDocs_JobCandidateId",
                table: "jobCandidateDocs",
                column: "JobCandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_jobCandidateDocs_JobDocumentId",
                table: "jobCandidateDocs",
                column: "JobDocumentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
         name: "jobCandidateDocs");
        }
    }
}
