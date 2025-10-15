using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApis.Migrations
{
    /// <inheritdoc />
    public partial class changesinnavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Document",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Document", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "JobDocument",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobOpeningId = table.Column<int>(type: "int", nullable: false),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobDocument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobDocument_Document_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Document",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobDocument_JobOpening_JobOpeningId",
                        column: x => x.JobOpeningId,
                        principalTable: "JobOpening",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobDocument_DocumentId",
                table: "JobDocument",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_JobDocument_JobOpeningId",
                table: "JobDocument",
                column: "JobOpeningId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobDocument");

            migrationBuilder.DropTable(
                name: "Document");
        }
    }
}
