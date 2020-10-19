using Microsoft.EntityFrameworkCore.Migrations;

namespace Forum.Data.Migrations
{
    public partial class CreateModeratorsForEachSection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SectionId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SectionId",
                table: "AspNetUsers",
                column: "SectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Sections_SectionId",
                table: "AspNetUsers",
                column: "SectionId",
                principalTable: "Sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Sections_SectionId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SectionId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "AspNetUsers");
        }
    }
}
