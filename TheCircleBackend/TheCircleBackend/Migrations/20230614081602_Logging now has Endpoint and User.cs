using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheCircleBackend.Migrations
{
    /// <inheritdoc />
    public partial class LoggingnowhasEndpointandUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Location",
                table: "LogItem",
                newName: "SubjectUser");

            migrationBuilder.AddColumn<string>(
                name: "Endpoint",
                table: "LogItem",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Endpoint",
                table: "LogItem");

            migrationBuilder.RenameColumn(
                name: "SubjectUser",
                table: "LogItem",
                newName: "Location");
        }
    }
}
