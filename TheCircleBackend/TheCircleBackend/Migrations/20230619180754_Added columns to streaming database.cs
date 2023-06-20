using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheCircleBackend.Migrations
{
    /// <inheritdoc />
    public partial class Addedcolumnstostreamingdatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Viewer_Stream_StreamId",
                table: "Viewer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Stream",
                table: "Stream");

            migrationBuilder.RenameTable(
                name: "Stream",
                newName: "VideoStream");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndStream",
                table: "VideoStream",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartStream",
                table: "VideoStream",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "StreamUserId",
                table: "VideoStream",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_VideoStream",
                table: "VideoStream",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_VideoStream_StreamUserId",
                table: "VideoStream",
                column: "StreamUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_VideoStream_WebsiteUser_StreamUserId",
                table: "VideoStream",
                column: "StreamUserId",
                principalTable: "WebsiteUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Viewer_VideoStream_StreamId",
                table: "Viewer",
                column: "StreamId",
                principalTable: "VideoStream",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VideoStream_WebsiteUser_StreamUserId",
                table: "VideoStream");

            migrationBuilder.DropForeignKey(
                name: "FK_Viewer_VideoStream_StreamId",
                table: "Viewer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VideoStream",
                table: "VideoStream");

            migrationBuilder.DropIndex(
                name: "IX_VideoStream_StreamUserId",
                table: "VideoStream");

            migrationBuilder.DropColumn(
                name: "EndStream",
                table: "VideoStream");

            migrationBuilder.DropColumn(
                name: "StartStream",
                table: "VideoStream");

            migrationBuilder.DropColumn(
                name: "StreamUserId",
                table: "VideoStream");

            migrationBuilder.RenameTable(
                name: "VideoStream",
                newName: "Stream");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Stream",
                table: "Stream",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Viewer_Stream_StreamId",
                table: "Viewer",
                column: "StreamId",
                principalTable: "Stream",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
