using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OSS.Twtr.Infrastructure.Migrations
{
    public partial class AddTablesFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MutedUsers_UserIdToMute",
                table: "MutedUsers",
                column: "UserIdToMute");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_TweetId",
                table: "Likes",
                column: "TweetId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookmarks_TweetId",
                table: "Bookmarks",
                column: "TweetId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockedUsers_UserIdToBlock",
                table: "BlockedUsers",
                column: "UserIdToBlock");

            migrationBuilder.AddForeignKey(
                name: "FK_BlockedUsers_Users_UserId",
                table: "BlockedUsers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BlockedUsers_Users_UserIdToBlock",
                table: "BlockedUsers",
                column: "UserIdToBlock",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookmarks_Tweets_TweetId",
                table: "Bookmarks",
                column: "TweetId",
                principalTable: "Tweets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookmarks_Users_UserId",
                table: "Bookmarks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Tweets_TweetId",
                table: "Likes",
                column: "TweetId",
                principalTable: "Tweets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Users_UserId",
                table: "Likes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MutedUsers_Users_UserId",
                table: "MutedUsers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MutedUsers_Users_UserIdToMute",
                table: "MutedUsers",
                column: "UserIdToMute",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlockedUsers_Users_UserId",
                table: "BlockedUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_BlockedUsers_Users_UserIdToBlock",
                table: "BlockedUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookmarks_Tweets_TweetId",
                table: "Bookmarks");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookmarks_Users_UserId",
                table: "Bookmarks");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Tweets_TweetId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Users_UserId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_MutedUsers_Users_UserId",
                table: "MutedUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_MutedUsers_Users_UserIdToMute",
                table: "MutedUsers");

            migrationBuilder.DropIndex(
                name: "IX_MutedUsers_UserIdToMute",
                table: "MutedUsers");

            migrationBuilder.DropIndex(
                name: "IX_Likes_TweetId",
                table: "Likes");

            migrationBuilder.DropIndex(
                name: "IX_Bookmarks_TweetId",
                table: "Bookmarks");

            migrationBuilder.DropIndex(
                name: "IX_BlockedUsers_UserIdToBlock",
                table: "BlockedUsers");
        }
    }
}
