using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheCircleBackend.Migrations
{
    /// <inheritdoc />
    public partial class Revertcolumnadd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StreamKey",
                table: "VideoStream");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StreamKey",
                table: "VideoStream",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
