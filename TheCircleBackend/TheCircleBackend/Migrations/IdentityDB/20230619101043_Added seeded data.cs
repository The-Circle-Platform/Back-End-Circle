using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheCircleBackend.Migrations.IdentityDB
{
    /// <inheritdoc />
    public partial class Addedseededdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "b74ddd14-6340-4840-95c2-db12554843e5", 0, "a57d94db-ca4b-41e6-b743-465186d695d4", "Admin@example.com", true, false, null, "ADMIN@EXAMPLE.COM", "ADMIN", "AQAAAAEAACcQAAAAEN/fGhX6NvJ5QJJkDTS6ZQDjqWN9jCMejNBZljSex98H0L/h/n8Iuc81pn9rD1jzdg==", null, false, "7e27c93a-22d5-4e92-bfc9-0cb30e5b5674", false, "Admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b74ddd14-6340-4840-95c2-db12554843e5");
        }
    }
}
