using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CineWorld.Services.ReactionAPI.Migrations
{
    /// <inheritdoc />
    public partial class seedTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserFavorites",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    FavoritedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFavorites", x => new { x.UserId, x.MovieId });
                });

            migrationBuilder.CreateTable(
                name: "UserRates",
                columns: table => new
                {
                    RatingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EpisodeId = table.Column<int>(type: "int", nullable: false),
                    RatingValue = table.Column<double>(type: "float", nullable: false),
                    RatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRates", x => x.RatingId);
                });

            migrationBuilder.InsertData(
                table: "UserFavorites",
                columns: new[] { "MovieId", "UserId", "FavoritedAt" },
                values: new object[,]
                {
                    { 101, "user1", new DateTime(2024, 11, 1, 10, 30, 0, 0, DateTimeKind.Unspecified) },
                    { 102, "user1", new DateTime(2024, 11, 1, 11, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 103, "user1", new DateTime(2024, 11, 2, 10, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 101, "user2", new DateTime(2024, 11, 1, 12, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 103, "user3", new DateTime(2024, 11, 2, 9, 15, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "UserRates",
                columns: new[] { "RatingId", "EpisodeId", "RatedAt", "RatingValue", "UserId" },
                values: new object[,]
                {
                    { 1, 101, new DateTime(2024, 11, 1, 10, 45, 0, 0, DateTimeKind.Unspecified), 4.0, "user1" },
                    { 2, 101, new DateTime(2024, 11, 1, 12, 30, 0, 0, DateTimeKind.Unspecified), 5.0, "user2" },
                    { 3, 102, new DateTime(2024, 11, 1, 11, 15, 0, 0, DateTimeKind.Unspecified), 3.0, "user1" },
                    { 4, 103, new DateTime(2024, 11, 2, 9, 30, 0, 0, DateTimeKind.Unspecified), 2.0, "user3" },
                    { 5, 103, new DateTime(2024, 11, 2, 10, 5, 0, 0, DateTimeKind.Unspecified), 4.0, "user1" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFavorites");

            migrationBuilder.DropTable(
                name: "UserRates");
        }
    }
}
