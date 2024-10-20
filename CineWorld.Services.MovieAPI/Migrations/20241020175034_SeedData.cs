using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CineWorld.Services.MovieAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "MovieId", "CategoryId", "CountryId", "CreatedDate", "Description", "Duration", "EpisodeCurrent", "EpisodeTotal", "ImageUrl", "IsHot", "Name", "OriginName", "SeriesId", "Slug", "Status", "Trailer", "UpdatedDate", "Year" },
                values: new object[,]
                {
                    { 1, 4, 1, new DateTime(2024, 10, 20, 17, 42, 0, 869, DateTimeKind.Utc), "Phﾆｰﾆ｡ng Hﾃ�n, thﾃ｢n sinh ra lﾃ� nﾃｴ b盻冂 c盻ｧa m盻冲 ﾄ黛ｺ｡i gia t盻冂. Khﾃｴng ch蘯･p nh蘯ｭn s盻� ph蘯ｭn c盻ｧa mﾃｬnh, h蘯ｯn h盻皇 lﾃｩn vﾃｵ cﾃｴng, sau ﾄ妥ｳ nh盻� m盻冲 ﾄ黛ｺ｡i k盻ｳ ng盻� mﾃ� d蘯･n thﾃ｢n vﾃ�o con ﾄ柁ｰ盻拵g tu tiﾃｪn ﾄ黛ｺｧy tr蘯ｯc tr盻�.", "25 phﾃｺt/t蘯ｭp", 28, null, "https://img.ophim.live/uploads/movies/vinh-sinh-phan-3-thumb.jpg", false, "Vﾄｩnh Sinh (Ph蘯ｧn 3)", "Immortality (Season 3)", null, "vinh-sinh-phan-3", true, "https://www.youtube.com/watch?v=DjylTsonlSw", new DateTime(2024, 10, 20, 17, 42, 0, 869, DateTimeKind.Utc), 2024 },
                    { 2, 1, 5, new DateTime(2024, 10, 20, 17, 42, 0, 869, DateTimeKind.Utc), "<p>M盻冲 thi蘯ｿu niﾃｪn b盻� b蘯ｯt n蘯｡t tham gia cﾃ｡c cu盻冂 thi s蘯ｯc ﾄ黛ｺｹp nhﾆｰ m盻冲 cﾃ｡ch ﾄ黛ｻ� tr蘯｣ thﾃｹ. Cﾃｴ ﾄ柁ｰ盻｣c m盻冲 hu蘯･n luy盻㌻ viﾃｪn tai ti蘯ｿng giﾃｺp ﾄ黛ｻ｡, ngﾆｰ盻拱 s盻嬶 nh蘯ｭn ra mﾃｬnh ﾄ疎ng sa l蘯ｧy.</p>", "42phﾃｺt/t蘯ｭp", 10, 10, "https://img.ophim.live/uploads/movies/vo-do-phan-2-thumb.jpg", false, "Vﾃｴ ﾄ黛ｻ� (Ph蘯ｧn 2)", "Insatiable (Season 2)", null, "vo-do-phan-2", true, "", new DateTime(2024, 10, 20, 17, 42, 0, 869, DateTimeKind.Utc), 2018 }
                });

            migrationBuilder.InsertData(
                table: "Episodes",
                columns: new[] { "EpisodeId", "CreatedDate", "EpisodeNumber", "IsFree", "MovieId", "Name", "Status", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc), 1, false, 1, "1", true, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc) },
                    { 2, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc), 2, false, 1, "2", true, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc) },
                    { 3, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc), 3, false, 1, "3", true, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc) },
                    { 4, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc), 4, false, 1, "4", true, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc) },
                    { 5, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc), 5, false, 1, "5", true, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc) },
                    { 6, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc), 6, false, 1, "6", true, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc) },
                    { 7, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc), 7, false, 1, "7", true, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc) },
                    { 8, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc), 8, false, 1, "8", true, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc) },
                    { 9, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc), 9, false, 1, "9", true, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc) },
                    { 10, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc), 10, false, 1, "10", true, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc) },
                    { 11, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc), 11, false, 1, "11", true, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc) },
                    { 12, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc), 12, false, 1, "12", true, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc) },
                    { 13, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc), 13, false, 1, "13", true, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc) },
                    { 14, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc), 14, false, 1, "14", true, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc) },
                    { 15, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc), 15, false, 1, "15", true, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc) },
                    { 16, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc), 16, false, 1, "16", true, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc) },
                    { 17, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc), 17, false, 1, "17", true, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc) },
                    { 18, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc), 18, false, 1, "18", true, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc) },
                    { 19, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc), 19, false, 1, "19", true, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc) },
                    { 20, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc), 20, false, 1, "20", true, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc) },
                    { 21, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc), 21, false, 1, "21", true, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc) },
                    { 22, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc), 22, false, 1, "22", true, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc) },
                    { 23, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc), 23, false, 1, "23", true, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc) },
                    { 24, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc), 24, false, 1, "24", true, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc) },
                    { 25, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc), 25, false, 1, "25", true, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc) },
                    { 26, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc), 26, false, 1, "26", true, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc) },
                    { 27, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc), 27, false, 1, "27", true, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc) },
                    { 28, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc), 28, false, 1, "28", true, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc) },
                    { 29, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc), 1, false, 2, "1", true, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc) },
                    { 30, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc), 2, false, 2, "2", true, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc) },
                    { 31, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc), 3, false, 2, "3", true, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc) },
                    { 32, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc), 4, false, 2, "4", true, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc) },
                    { 33, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc), 5, false, 2, "5", true, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc) },
                    { 34, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc), 6, false, 2, "6", true, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc) },
                    { 35, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc), 7, false, 2, "7", true, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc) },
                    { 36, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc), 8, false, 2, "8", true, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc) },
                    { 37, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc), 9, false, 2, "9", true, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc) },
                    { 38, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc), 10, false, 2, "10", true, new DateTime(2024, 10, 20, 17, 6, 28, 583, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "MovieGenres",
                columns: new[] { "Id", "GenreId", "MovieId" },
                values: new object[,]
                {
                    { 1, 10, 1 },
                    { 2, 1, 1 },
                    { 3, 2, 2 },
                    { 4, 3, 2 },
                    { 5, 5, 2 }
                });

            migrationBuilder.InsertData(
                table: "Servers",
                columns: new[] { "ServerId", "EpisodeId", "Link", "Name" },
                values: new object[,]
                {
                    { 1, 1, "https://vip.opstream11.com/share/fab4fd725511924faa4daab64a353415", "#1" },
                    { 2, 2, "https://vip.opstream11.com/share/48d740cb434f20ec8b9dd4007e798eb4", "#1" },
                    { 3, 3, "https://vip.opstream11.com/share/10ccc3d6b30e89b173594526e81df7b9", "#1" },
                    { 4, 4, "https://vip.opstream11.com/share/9ae82c26134ecd4981bda3c252fc1acf", "#1" },
                    { 5, 5, "https://vip.opstream11.com/share/4580593c772e8cda49fcf7668a785ac4", "#1" },
                    { 6, 6, "https://vip.opstream11.com/share/7276c0c3f88b2a852674bd7e1cda0cf6", "#1" },
                    { 7, 7, "https://vip.opstream11.com/share/822b440edda4c46a5e1dad463eaf8ebd", "#1" },
                    { 8, 8, "https://vip.opstream11.com/share/3a74864c593d71aa5eec4da27f11768f", "#1" },
                    { 9, 9, "https://vip.opstream11.com/share/5fa3410e6fd97e1c2ed4eadfb7eff53f", "#1" },
                    { 10, 10, "https://vip.opstream11.com/share/90c9acf337005013884727424781ca24", "#1" },
                    { 11, 11, "https://vip.opstream11.com/share/57f3c6d49921c666e47139e2ead526e3", "#1" },
                    { 12, 12, "https://vip.opstream11.com/share/df5bb8067b7b1d641275af93581dea28", "#1" },
                    { 13, 13, "https://vip.opstream11.com/share/1a81daeb419a28fc7a373eb87bc3ef65", "#1" },
                    { 14, 14, "https://vip.opstream11.com/share/5beab85f16162a828932242cbfc52994", "#1" },
                    { 15, 15, "https://vip.opstream11.com/share/1dcfe838513ec134aa9cd68df4b08ff2", "#1" },
                    { 16, 16, "https://vip.opstream11.com/share/3e658cb2a57f417dc712210ba28c8f4c", "#1" },
                    { 17, 17, "https://vip.opstream11.com/share/e02cf150a0e8155a1bf54e4d586dcb37", "#1" },
                    { 18, 18, "https://vip.opstream11.com/share/838b5a042f1761ef0b5981585a0a7f38", "#1" },
                    { 19, 19, "https://vip.opstream11.com/share/2167afc4a46ea99c6b39b178f4c8501d", "#1" },
                    { 20, 20, "https://vip.opstream11.com/share/b1606b159abc283677c84d68e91ec97a", "#1" },
                    { 21, 21, "https://vip.opstream15.com/share/95938bd687b287423c514deeae8bf4f1", "#1" },
                    { 22, 22, "https://vip.opstream15.com/share/ba8cf349f278c2ae7f53ba026389fabd", "#1" },
                    { 23, 23, "https://vip.opstream15.com/share/a1d2a5b05d09395f32299a4d270a9a32", "#1" },
                    { 24, 24, "https://vip.opstream15.com/share/4ff82c0b528f93716115986da0412623", "#1" },
                    { 25, 25, "https://vip.opstream15.com/share/f45451d5c725bd5795d432e970d3d978", "#1" },
                    { 26, 26, "https://vip.opstream15.com/share/3ae04cd9569705cc18fd51df7e870279", "#1" },
                    { 27, 27, "https://vip.opstream15.com/share/9f2dab581c42e1381065d4d6dbd75d1a", "#1" },
                    { 28, 28, "https://vip.opstream15.com/share/063f5947bbefa2dc4936b33f80cab66c", "#1" },
                    { 29, 29, "https://vip.opstream16.com/share/ea159dc9788ffac311592613b7f71fbb", "#1" },
                    { 30, 30, "https://vip.opstream16.com/share/1a68e5f4ade56ed1d4bf273e55510750", "#1" },
                    { 31, 31, "https://vip.opstream16.com/share/99f59c0842e83c808dd1813b48a37c6a", "#1" },
                    { 32, 32, "https://vip.opstream16.com/share/8b4224068a41c5d37f5e2d54f3995089", "#1" },
                    { 33, 33, "https://vip.opstream16.com/share/d04863f100d59b3eb688a11f95b0ae60", "#1" },
                    { 34, 34, "https://vip.opstream16.com/share/063e26c670d07bb7c4d30e6fc69fe056", "#1" },
                    { 35, 35, "https://vip.opstream16.com/share/878d5691c824ee2aaf770f7d36c151d6", "#1" },
                    { 36, 36, "https://vip.opstream16.com/share/565030e1fce4e481f9823a7de3b8a047", "#1" },
                    { 37, 37, "https://vip.opstream16.com/share/bc7f621451b4f5df308a8e098112185d", "#1" },
                    { 38, 38, "https://vip.opstream16.com/share/03cf87174debaccd689c90c34577b82f", "#1" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MovieGenres",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "MovieGenres",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "MovieGenres",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "MovieGenres",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "MovieGenres",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Servers",
                keyColumn: "ServerId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Servers",
                keyColumn: "ServerId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Servers",
                keyColumn: "ServerId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Servers",
                keyColumn: "ServerId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Servers",
                keyColumn: "ServerId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Servers",
                keyColumn: "ServerId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Servers",
                keyColumn: "ServerId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Servers",
                keyColumn: "ServerId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Servers",
                keyColumn: "ServerId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Servers",
                keyColumn: "ServerId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Servers",
                keyColumn: "ServerId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Servers",
                keyColumn: "ServerId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Servers",
                keyColumn: "ServerId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Servers",
                keyColumn: "ServerId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Servers",
                keyColumn: "ServerId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Servers",
                keyColumn: "ServerId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Servers",
                keyColumn: "ServerId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Servers",
                keyColumn: "ServerId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Servers",
                keyColumn: "ServerId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Servers",
                keyColumn: "ServerId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Servers",
                keyColumn: "ServerId",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Servers",
                keyColumn: "ServerId",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Servers",
                keyColumn: "ServerId",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Servers",
                keyColumn: "ServerId",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Servers",
                keyColumn: "ServerId",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Servers",
                keyColumn: "ServerId",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Servers",
                keyColumn: "ServerId",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Servers",
                keyColumn: "ServerId",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Servers",
                keyColumn: "ServerId",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Servers",
                keyColumn: "ServerId",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Servers",
                keyColumn: "ServerId",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Servers",
                keyColumn: "ServerId",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Servers",
                keyColumn: "ServerId",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Servers",
                keyColumn: "ServerId",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Servers",
                keyColumn: "ServerId",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Servers",
                keyColumn: "ServerId",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Servers",
                keyColumn: "ServerId",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Servers",
                keyColumn: "ServerId",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "EpisodeId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "EpisodeId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "EpisodeId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "EpisodeId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "EpisodeId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "EpisodeId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "EpisodeId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "EpisodeId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "EpisodeId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "EpisodeId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "EpisodeId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "EpisodeId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "EpisodeId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "EpisodeId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "EpisodeId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "EpisodeId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "EpisodeId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "EpisodeId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "EpisodeId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "EpisodeId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "EpisodeId",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "EpisodeId",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "EpisodeId",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "EpisodeId",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "EpisodeId",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "EpisodeId",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "EpisodeId",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "EpisodeId",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "EpisodeId",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "EpisodeId",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "EpisodeId",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "EpisodeId",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "EpisodeId",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "EpisodeId",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "EpisodeId",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "EpisodeId",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "EpisodeId",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "EpisodeId",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "MovieId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "MovieId",
                keyValue: 2);
        }
    }
}
