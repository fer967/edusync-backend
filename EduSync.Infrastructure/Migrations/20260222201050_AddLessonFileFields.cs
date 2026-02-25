using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduSync.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLessonFileFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Lessons",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Lessons",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Lessons");
        }
    }
}
