using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CineWorld.Services.CommentAPI.Migrations
{
    /// <inheritdoc />
    public partial class seedTableComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    CommentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommentParentId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    EpisodeId = table.Column<int>(type: "int", nullable: true),
                    CommentContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.CommentId);
                });

            migrationBuilder.InsertData(
                table: "Comments",
                columns: new[] { "CommentId", "CommentContent", "CommentParentId", "CreatedAt", "EpisodeId", "MovieId", "UserId" },
                values: new object[,]
                {
                    { 1, "This movie is amazing! I loved the characters and the plot.", null, new DateTime(2024, 10, 31, 22, 30, 37, 231, DateTimeKind.Local).AddTicks(4285), null, 1, "3a5a3321-ebbe-49ac-b3aa-0466a2072515" },
                    { 2, "I agree, the movie was fantastic, especially the special effects!", 1, new DateTime(2024, 10, 31, 22, 30, 37, 231, DateTimeKind.Local).AddTicks(4301), null, 1, "af4f152c-5cdd-4027-8024-28919b019a2e" },
                    { 3, "Episode 1 had a slow start, but it picked up towards the end!", null, new DateTime(2024, 10, 31, 22, 30, 37, 231, DateTimeKind.Local).AddTicks(4302), 1, 2, "3a5a3321-ebbe-49ac-b3aa-0466a2072515" },
                    { 4, "Totally agree, the ending was the best part!", 3, new DateTime(2024, 10, 31, 22, 30, 37, 231, DateTimeKind.Local).AddTicks(4304), 1, 2, "af4f152c-5cdd-4027-8024-28919b019a2e" },
                    { 5, "I didn't enjoy this movie as much as the previous ones. The pacing was off.", null, new DateTime(2024, 10, 31, 22, 30, 37, 231, DateTimeKind.Local).AddTicks(4305), null, 3, "b736c9ed-4b4f-4f75-be82-186c25a69e4e" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");
        }
    }
}
