using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineWorld.Services.MembershipAPI.Migrations
{
    /// <inheritdoc />
    public partial class Update2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentSessionUrl",
                table: "Receipts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentSessionUrl",
                table: "Receipts");
        }
    }
}
