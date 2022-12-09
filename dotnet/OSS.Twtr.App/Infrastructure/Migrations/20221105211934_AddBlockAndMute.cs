using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OSS.Twtr.Infrastructure.Migrations
{
    public partial class AddBlockAndMute : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlockedUsers",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserIdToBlock = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BlockedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockedUsers", x => new { x.UserId, x.UserIdToBlock });
                });

            migrationBuilder.CreateTable(
                name: "MutedUsers",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserIdToMute = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MutedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MutedUsers", x => new { x.UserId, x.UserIdToMute });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlockedUsers");

            migrationBuilder.DropTable(
                name: "MutedUsers");
        }
    }
}
