using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheCircleBackend.Migrations
{
    /// <inheritdoc />
    public partial class Newdatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LogItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogItem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stream",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stream", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WebsiteUser",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsOnline = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebsiteUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WebUserId = table.Column<int>(type: "int", nullable: false),
                    ReceiverId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessage_WebsiteUser_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "WebsiteUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ChatMessage_WebsiteUser_WebUserId",
                        column: x => x.WebUserId,
                        principalTable: "WebsiteUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Viewer",
                columns: table => new
                {
                    ConnectionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StreamId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Viewer", x => x.ConnectionId);
                    table.ForeignKey(
                        name: "FK_Viewer_Stream_StreamId",
                        column: x => x.StreamId,
                        principalTable: "Stream",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Viewer_WebsiteUser_UserId",
                        column: x => x.UserId,
                        principalTable: "WebsiteUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessage_ReceiverId",
                table: "ChatMessage",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessage_WebUserId",
                table: "ChatMessage",
                column: "WebUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Viewer_StreamId",
                table: "Viewer",
                column: "StreamId");

            migrationBuilder.CreateIndex(
                name: "IX_Viewer_UserId",
                table: "Viewer",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WebsiteUser_UserName",
                table: "WebsiteUser",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatMessage");

            migrationBuilder.DropTable(
                name: "LogItem");

            migrationBuilder.DropTable(
                name: "Viewer");

            migrationBuilder.DropTable(
                name: "Stream");

            migrationBuilder.DropTable(
                name: "WebsiteUser");
        }
    }
}
