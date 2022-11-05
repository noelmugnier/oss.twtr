using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OSS.Twtr.Infrastructure.Migrations
{
    public partial class AddPinnedTweet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PinnedTweetId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_PinnedTweetId",
                table: "Users",
                column: "PinnedTweetId",
                unique: true,
                filter: "[PinnedTweetId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Tweets_PinnedTweetId",
                table: "Users",
                column: "PinnedTweetId",
                principalTable: "Tweets",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Tweets_PinnedTweetId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_PinnedTweetId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PinnedTweetId",
                table: "Users");
        }
    }
}
