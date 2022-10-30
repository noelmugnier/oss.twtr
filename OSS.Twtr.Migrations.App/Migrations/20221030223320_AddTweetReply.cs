using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OSS.Twtr.Migrations.App.Migrations
{
    public partial class AddTweetReply : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ReplyToTweetId",
                table: "Tweets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tweets_ReplyToTweetId",
                table: "Tweets",
                column: "ReplyToTweetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tweets_Tweets_ReplyToTweetId",
                table: "Tweets",
                column: "ReplyToTweetId",
                principalTable: "Tweets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tweets_Tweets_ReplyToTweetId",
                table: "Tweets");

            migrationBuilder.DropIndex(
                name: "IX_Tweets_ReplyToTweetId",
                table: "Tweets");

            migrationBuilder.DropColumn(
                name: "ReplyToTweetId",
                table: "Tweets");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
