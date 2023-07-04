using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheCircleBackend.Migrations
{
    /// <inheritdoc />
    public partial class RemovedPrivateKeyStorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrivateKey",
                table: "UserKeys");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PrivateKey",
                table: "UserKeys",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "UserKeys",
                keyColumn: "Id",
                keyValue: 1,
                column: "PrivateKey",
                value: "MIICeAIBADANBgkqhkiG9w0BAQEFAASCAmIwggJeAgEAAoGBAKHk10g1evWq9t7KEEu8sKUGGKwwjx+X7fsn0ELt71G0jtdo43jjF8x4Q2S1Ho1VGV3XKMagRYf4t/Z3z8THd2c0g4SFwVSvQca/+BeJ9qmuVji7mw5x67PeOUWzJS0240i1tJFXHbLfXAgg666GIrkI135ElhwJH9Alu6xQ2VetAgMBAAECgYB/GuojGUoGo0nbtQ2CSQzvI5AvcJiOF3yS2blbMu/YWEhlu0YM3U8MC8ftw33PPOcDlC/Bcofkr1PPwFVxi6GkOBxDPiHthzruGGlnzbSMA9Ldo9qf/9ZUzKay26fhEVQoACNGsvw4GZxAblJ3UkqBnPea1chGQWh9v2xlo4YRiQJBAMwwtMER3URwxoUZGfyG4tTVfg/TK7AF5Iz6c5YQDx/UoSTrKLIM14f+APdvhMCAL5G+AEDKhesrUNpTXvNND3cCQQDK+MBpmfb/oDnqxVtFoY+pW3d7EjUyNXixM62xSJk5IdAX0gqwiMveb3svAM6SF2ro0b/IGItfNSaVxxtIxcL7AkEAiFwGeeDqOShvCreGqSOTG7svInZNeJGW3abrxc0XrJQcwUDhvnXhAYpZLuSkbMGuAtA17w7QfApDRmniwOw3ZQJBAKZnnkh1pB0bXaBuwU+rDz8H8EMEQHyzfgm5lrN8E7LVV+fPmlf1Lz9kIpf8j18St+G85QDFrq4Vw1aUcHgPOrUCQQDGLgVLCaOe4vE+0RMfwyI1FO8kXfARz0whL/8TevtnvvghLP2ogUNUswhyaZUS5bA8AxeVrHxYre1GGQCyv8DM");
        }
    }
}
