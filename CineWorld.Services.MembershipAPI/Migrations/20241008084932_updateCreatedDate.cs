using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineWorld.Services.MembershipAPI.Migrations
{
    /// <inheritdoc />
    public partial class updateCreatedDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Packages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 8, 15, 49, 32, 128, DateTimeKind.Local).AddTicks(5015),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Packages",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 10, 8, 15, 49, 32, 128, DateTimeKind.Local).AddTicks(5015));
        }
    }
}
