using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CineWorld.Services.CommentAPI.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
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
                    CommentContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.CommentId);
                });

            migrationBuilder.InsertData(
                table: "Comments",
                columns: new[] { "CommentId", "CommentContent", "CommentParentId", "CreatedAt", "MovieId", "UserId" },
                values: new object[,]
                {
                    { 1, "This movie is amazing! I loved the characters and the plot.", null, new DateTime(2024, 11, 26, 16, 55, 10, 743, DateTimeKind.Local).AddTicks(9577), 1, "4986f53c-a2db-4b71-80df-404bcad5413a" },
                    { 2, "I agree, the movie was fantastic, especially the special effects!", 1, new DateTime(2024, 11, 26, 16, 55, 10, 743, DateTimeKind.Local).AddTicks(9590), 1, "4986f53c-a2db-4b71-80df-404bcad5413a" },
                    { 3, "Episode 1 had a slow start, but it picked up towards the end!", null, new DateTime(2024, 11, 26, 16, 55, 10, 743, DateTimeKind.Local).AddTicks(9591), 2, "4986f53c-a2db-4b71-80df-404bcad5413a" },
                    { 4, "Totally agree, the ending was the best part!", 3, new DateTime(2024, 11, 26, 16, 55, 10, 743, DateTimeKind.Local).AddTicks(9592), 2, "4986f53c-a2db-4b71-80df-404bcad5413a" },
                    { 5, "I didn't enjoy this movie as much as the previous ones. The pacing was off.", null, new DateTime(2024, 11, 26, 16, 55, 10, 743, DateTimeKind.Local).AddTicks(9593), 3, "4986f53c-a2db-4b71-80df-404bcad5413a" }
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
