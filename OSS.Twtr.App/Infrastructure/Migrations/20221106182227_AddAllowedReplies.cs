using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OSS.Twtr.Infrastructure.Migrations
{
    public partial class AddAllowedReplies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AllowedReplies",
                table: "Tweets",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowedReplies",
                table: "Tweets");
        }
    }
}
