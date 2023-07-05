using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OSS.Twtr.Migrations
{
    public partial class InitDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    FollowerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscribedToUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscribedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => new { x.FollowerUserId, x.SubscribedToUserId });
                });

            migrationBuilder.CreateTable(
                name: "Trendings",
                columns: table => new
                {
                    AnalyzedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    TweetCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trendings", x => new { x.AnalyzedOn, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "BlockedUsers",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserIdToBlock = table.Column<Guid>(type: "uuid", nullable: false),
                    BlockedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockedUsers", x => new { x.UserId, x.UserIdToBlock });
                });

            migrationBuilder.CreateTable(
                name: "Bookmarks",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TweetId = table.Column<Guid>(type: "uuid", nullable: false),
                    BookmarkedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookmarks", x => new { x.UserId, x.TweetId });
                });

            migrationBuilder.CreateTable(
                name: "Likes",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TweetId = table.Column<Guid>(type: "uuid", nullable: false),
                    LikedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Likes", x => new { x.UserId, x.TweetId });
                });

            migrationBuilder.CreateTable(
                name: "MutedUsers",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserIdToMute = table.Column<Guid>(type: "uuid", nullable: false),
                    MutedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MutedUsers", x => new { x.UserId, x.UserIdToMute });
                });

            migrationBuilder.CreateTable(
                name: "ProfilePictures",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Data = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "Tokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Kind = table.Column<int>(type: "integer", nullable: false),
                    TweetId = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tweets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LikesCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    RetweetsCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Kind = table.Column<int>(type: "integer", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: true),
                    AllowedReplies = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    PostedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReferenceTweetId = table.Column<Guid>(type: "uuid", nullable: true),
                    ThreadId = table.Column<Guid>(type: "uuid", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tweets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tweets_Tweets_ReferenceTweetId",
                        column: x => x.ReferenceTweetId,
                        principalTable: "Tweets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            
            migrationBuilder.AddColumn<string>("DisplayName", "Users", nullable:true);
            migrationBuilder.AddColumn<DateTime>("MemberSince", "Users", defaultValueSql:"now()");

            migrationBuilder.AddColumn<Guid>(
                name: "PinnedTweetId",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Users",
                type: "bytea",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDate",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Job",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlockedUsers_UserIdToBlock",
                table: "BlockedUsers",
                column: "UserIdToBlock");

            migrationBuilder.CreateIndex(
                name: "IX_Bookmarks_TweetId",
                table: "Bookmarks",
                column: "TweetId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_TweetId",
                table: "Likes",
                column: "TweetId");

            migrationBuilder.CreateIndex(
                name: "IX_MutedUsers_UserIdToMute",
                table: "MutedUsers",
                column: "UserIdToMute");

            migrationBuilder.CreateIndex(
                name: "IX_ProfilePictures_UserId",
                table: "ProfilePictures",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_CreatedOn",
                table: "Tokens",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_TweetId",
                table: "Tokens",
                column: "TweetId");

            migrationBuilder.CreateIndex(
                name: "IX_Tweets_AuthorId",
                table: "Tweets",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Tweets_ReferenceTweetId",
                table: "Tweets",
                column: "ReferenceTweetId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PinnedTweetId",
                table: "Users",
                column: "PinnedTweetId",
                unique: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_ProfilePictures_Users_UserId",
                table: "ProfilePictures",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tokens_Tweets_TweetId",
                table: "Tokens",
                column: "TweetId",
                principalTable: "Tweets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tweets_Users_AuthorId",
                table: "Tweets",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tweets_Users_AuthorId",
                table: "Tweets");

            migrationBuilder.DropTable(
                name: "BlockedUsers");

            migrationBuilder.DropTable(
                name: "Bookmarks");

            migrationBuilder.DropTable(
                name: "Likes");

            migrationBuilder.DropTable(
                name: "MutedUsers");

            migrationBuilder.DropTable(
                name: "ProfilePictures");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "Tokens");

            migrationBuilder.DropTable(
                name: "Trendings");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Tweets");
        }
    }
}
