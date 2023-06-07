using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheCircleBackend.Migrations
{
    /// <inheritdoc />
    public partial class updatetousermodel2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_WebsiteUser_UserName",
                table: "WebsiteUser");

            migrationBuilder.CreateIndex(
                name: "IX_WebsiteUser_UserName",
                table: "WebsiteUser",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WebsiteUser_UserName",
                table: "WebsiteUser");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_WebsiteUser_UserName",
                table: "WebsiteUser",
                column: "UserName");
        }
    }
}
