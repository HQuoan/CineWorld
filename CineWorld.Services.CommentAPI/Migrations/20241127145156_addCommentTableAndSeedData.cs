using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CineWorld.Services.CommentAPI.Migrations
{
    /// <inheritdoc />
    public partial class addCommentTableAndSeedData : Migration
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
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                columns: new[] { "CommentId", "Avatar", "CommentContent", "CommentParentId", "CreatedAt", "FullName", "MovieId", "UserId" },
                values: new object[,]
                {
                    { 1, "https://cineworld-user-avatars.s3.amazonaws.com/users/4986f53c-a2db-4b71-80df-404bcad5413a/c7a45e1d-6b3b-4ef7-bd46-f28b32578dc1_bx170468-kD2X9O2XM9KH.jpg", "This movie is amazing! I loved the characters and the plot.", null, new DateTime(2024, 11, 27, 21, 51, 55, 407, DateTimeKind.Local).AddTicks(3262), "Admin", 1, "4986f53c-a2db-4b71-80df-404bcad5413a" },
                    { 2, "https://cineworld-user-avatars.s3.amazonaws.com/users/4986f53c-a2db-4b71-80df-404bcad5413a/c7a45e1d-6b3b-4ef7-bd46-f28b32578dc1_bx170468-kD2X9O2XM9KH.jpg", "I agree, the movie was fantastic, especially the special effects!", 1, new DateTime(2024, 11, 27, 21, 51, 55, 407, DateTimeKind.Local).AddTicks(3275), "Admin", 1, "4986f53c-a2db-4b71-80df-404bcad5413a" },
                    { 3, "https://cineworld-user-avatars.s3.amazonaws.com/users/4986f53c-a2db-4b71-80df-404bcad5413a/c7a45e1d-6b3b-4ef7-bd46-f28b32578dc1_bx170468-kD2X9O2XM9KH.jpg", "Episode 1 had a slow start, but it picked up towards the end!", null, new DateTime(2024, 11, 27, 21, 51, 55, 407, DateTimeKind.Local).AddTicks(3277), "Admin", 2, "4986f53c-a2db-4b71-80df-404bcad5413a" }
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
