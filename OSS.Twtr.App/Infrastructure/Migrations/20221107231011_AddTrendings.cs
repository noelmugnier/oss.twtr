using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OSS.Twtr.Infrastructure.Migrations
{
    public partial class AddTrendings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Trendings",
                columns: table => new
                {
                    AnalyzedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TweetCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trendings", x => new { x.AnalyzedOn, x.Name });
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_CreatedOn",
                table: "Tokens",
                column: "CreatedOn");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Trendings");

            migrationBuilder.DropIndex(
                name: "IX_Tokens_CreatedOn",
                table: "Tokens");
        }
    }
}
