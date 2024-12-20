using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CineWorld.Services.HistoryAPI.Migrations
{
    /// <inheritdoc />
    public partial class addAndSeedTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "watchHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    EpisodeId = table.Column<int>(type: "int", nullable: true),
                    MovieUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WatchedDuration = table.Column<TimeSpan>(type: "time", nullable: false),
                    LastWatched = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_watchHistories", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "watchHistories",
                columns: new[] { "Id", "EpisodeId", "LastWatched", "MovieId", "MovieUrl", "UserId", "WatchedDuration" },
                values: new object[,]
                {
                    { 1, 2, new DateTime(2024, 11, 1, 17, 30, 0, 0, DateTimeKind.Local), 1, "seedTable.com", "user123", new TimeSpan(0, 0, 20, 0, 0) },
                    { 2, 1, new DateTime(2024, 11, 2, 18, 0, 0, 0, DateTimeKind.Local), 2, "seedTable.com", "user456", new TimeSpan(0, 0, 15, 0, 0) },
                    { 3, 3, new DateTime(2024, 11, 3, 21, 45, 0, 0, DateTimeKind.Local), 3, "seedTable.com", "user789", new TimeSpan(0, 0, 5, 0, 0) },
                    { 4, 1, new DateTime(2024, 11, 4, 23, 0, 0, 0, DateTimeKind.Local), 4, "seedTable.com", "user123", new TimeSpan(0, 0, 30, 0, 0) },
                    { 5, 2, new DateTime(2024, 11, 6, 1, 15, 0, 0, DateTimeKind.Local), 5, "seedTable.com", "user456", new TimeSpan(0, 0, 10, 0, 0) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "watchHistories");
        }
    }
}
