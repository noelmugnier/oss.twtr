using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OSS.Twtr.Infrastructure.Migrations
{
    public partial class InitDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bookmarks",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TweetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookmarkedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookmarks", x => new { x.UserId, x.TweetId });
                });

            migrationBuilder.CreateTable(
                name: "Likes",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TweetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LikedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Likes", x => new { x.UserId, x.TweetId });
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    FollowerUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubscribedToUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubscribedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => new { x.FollowerUserId, x.SubscribedToUserId });
                });

            migrationBuilder.AddColumn<string>("DisplayName", "Users", nullable:true);
            migrationBuilder.AddColumn<DateTime>("MemberSince", "Users", defaultValueSql:"getUtcDate()");

            migrationBuilder.CreateTable(
                name: "Tweets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Kind = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReferenceTweetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ThreadId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
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
                    table.ForeignKey(
                        name: "FK_Tweets_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tweets_AuthorId",
                table: "Tweets",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Tweets_ReferenceTweetId",
                table: "Tweets",
                column: "ReferenceTweetId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookmarks");

            migrationBuilder.DropTable(
                name: "Likes");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "Tweets");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
