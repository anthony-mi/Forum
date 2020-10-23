using Microsoft.EntityFrameworkCore.Migrations;

namespace Forum.Data.Migrations
{
    public partial class CreateModerators : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ModeratorId",
                table: "Sections",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sections_ModeratorId",
                table: "Sections",
                column: "ModeratorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sections_AspNetUsers_ModeratorId",
                table: "Sections",
                column: "ModeratorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sections_AspNetUsers_ModeratorId",
                table: "Sections");

            migrationBuilder.DropIndex(
                name: "IX_Sections_ModeratorId",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "ModeratorId",
                table: "Sections");
        }
    }
}
