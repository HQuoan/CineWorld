using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineWorld.Services.ViewAPI.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Views",
                columns: table => new
                {
                    ViewId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IpAddress = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    EpisodeId = table.Column<int>(type: "int", nullable: false),
                    ViewDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Views", x => x.ViewId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Views_EpisodeId",
                table: "Views",
                column: "EpisodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Views_IpAddress",
                table: "Views",
                column: "IpAddress");

            migrationBuilder.CreateIndex(
                name: "IX_Views_MovieId",
                table: "Views",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_Views_UserId",
                table: "Views",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Views_ViewDate",
                table: "Views",
                column: "ViewDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Views");
        }
    }
}
