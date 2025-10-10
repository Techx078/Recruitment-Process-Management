using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApis.Migrations
{
    /// <inheritdoc />
    public partial class addJobOpeningandJobReviewrandJobInteeviewer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Interviewer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interviewer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Interviewer_Users_Id",
                        column: x => x.Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Recruiter",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recruiter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recruiter_Users_Id",
                        column: x => x.Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviewer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviewer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviewer_Users_Id",
                        column: x => x.Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobOpening",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Requirnment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SalryRange = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Benefits = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeadLine = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobOpening", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobOpening_Recruiter_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Recruiter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "jobInterviewer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobOpeningId = table.Column<int>(type: "int", nullable: false),
                    InterviewerId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_jobInterviewer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_jobInterviewer_Interviewer_InterviewerId",
                        column: x => x.InterviewerId,
                        principalTable: "Interviewer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_jobInterviewer_JobOpening_JobOpeningId",
                        column: x => x.JobOpeningId,
                        principalTable: "JobOpening",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobReviewer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobOpeningId = table.Column<int>(type: "int", nullable: false),
                    ReviewerId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobReviewer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobReviewer_JobOpening_JobOpeningId",
                        column: x => x.JobOpeningId,
                        principalTable: "JobOpening",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobReviewer_Reviewer_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Reviewer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_jobInterviewer_InterviewerId",
                table: "jobInterviewer",
                column: "InterviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_jobInterviewer_JobOpeningId",
                table: "jobInterviewer",
                column: "JobOpeningId");

            migrationBuilder.CreateIndex(
                name: "IX_JobOpening_CreatedById",
                table: "JobOpening",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobReviewer_JobOpeningId",
                table: "JobReviewer",
                column: "JobOpeningId");

            migrationBuilder.CreateIndex(
                name: "IX_JobReviewer_ReviewerId",
                table: "JobReviewer",
                column: "ReviewerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "jobInterviewer");

            migrationBuilder.DropTable(
                name: "JobReviewer");

            migrationBuilder.DropTable(
                name: "Interviewer");

            migrationBuilder.DropTable(
                name: "JobOpening");

            migrationBuilder.DropTable(
                name: "Reviewer");

            migrationBuilder.DropTable(
                name: "Recruiter");
        }
    }
}
