using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApis.Migrations
{
    /// <inheritdoc />
    public partial class changedomaintype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Rename column
            migrationBuilder.RenameColumn(
                name: "PrimaryDomain",
                table: "Users",
                newName: "Domain");

            // Change type from int -> string
            migrationBuilder.AlterColumn<string>(
                name: "Domain",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Change type back string -> int
            migrationBuilder.AlterColumn<int>(
                name: "Domain",
                table: "Users",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            // Rename back
            migrationBuilder.RenameColumn(
                name: "Domain",
                table: "Users",
                newName: "PrimaryDomain");
        }

    }
}
