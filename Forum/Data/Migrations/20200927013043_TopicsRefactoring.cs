using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Forum.Data.Migrations
{
    public partial class TopicsRefactoring : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_AspNetUsers_AuthorId1",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_AspNetUsers_EditorId1",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Topics_AspNetUsers_AuthorId1",
                table: "Topics");

            migrationBuilder.DropForeignKey(
                name: "FK_Topics_AspNetUsers_EditorId1",
                table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_Topics_AuthorId1",
                table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_Topics_EditorId1",
                table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_Posts_AuthorId1",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_EditorId1",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "AuthorId1",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "Creation",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "Editing",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "EditorId1",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "AuthorId1",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Creation",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Editing",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "EditorId1",
                table: "Posts");

            migrationBuilder.AlterColumn<string>(
                name: "EditorId",
                table: "Topics",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AuthorId",
                table: "Topics",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Topics",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Edited",
                table: "Topics",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EditorId",
                table: "Posts",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AuthorId",
                table: "Posts",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Posts",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Edited",
                table: "Posts",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TopicViewModel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(maxLength: 60, nullable: false),
                    Body = table.Column<string>(nullable: true),
                    SectionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopicViewModel", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Topics_AuthorId",
                table: "Topics",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_EditorId",
                table: "Topics",
                column: "EditorId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_AuthorId",
                table: "Posts",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_EditorId",
                table: "Posts",
                column: "EditorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_AspNetUsers_AuthorId",
                table: "Posts",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_AspNetUsers_EditorId",
                table: "Posts",
                column: "EditorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_AspNetUsers_AuthorId",
                table: "Topics",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_AspNetUsers_EditorId",
                table: "Topics",
                column: "EditorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_AspNetUsers_AuthorId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_AspNetUsers_EditorId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Topics_AspNetUsers_AuthorId",
                table: "Topics");

            migrationBuilder.DropForeignKey(
                name: "FK_Topics_AspNetUsers_EditorId",
                table: "Topics");

            migrationBuilder.DropTable(
                name: "TopicViewModel");

            migrationBuilder.DropIndex(
                name: "IX_Topics_AuthorId",
                table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_Topics_EditorId",
                table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_Posts_AuthorId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_EditorId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "Edited",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Edited",
                table: "Posts");

            migrationBuilder.AlterColumn<int>(
                name: "EditorId",
                table: "Topics",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AuthorId",
                table: "Topics",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthorId1",
                table: "Topics",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Creation",
                table: "Topics",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Editing",
                table: "Topics",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EditorId1",
                table: "Topics",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EditorId",
                table: "Posts",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AuthorId",
                table: "Posts",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthorId1",
                table: "Posts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Creation",
                table: "Posts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Editing",
                table: "Posts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EditorId1",
                table: "Posts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Topics_AuthorId1",
                table: "Topics",
                column: "AuthorId1");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_EditorId1",
                table: "Topics",
                column: "EditorId1");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_AuthorId1",
                table: "Posts",
                column: "AuthorId1");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_EditorId1",
                table: "Posts",
                column: "EditorId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_AspNetUsers_AuthorId1",
                table: "Posts",
                column: "AuthorId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_AspNetUsers_EditorId1",
                table: "Posts",
                column: "EditorId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_AspNetUsers_AuthorId1",
                table: "Topics",
                column: "AuthorId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_AspNetUsers_EditorId1",
                table: "Topics",
                column: "EditorId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
