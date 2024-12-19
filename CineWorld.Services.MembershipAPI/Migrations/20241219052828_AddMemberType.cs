using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineWorld.Services.MembershipAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddMemberType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MemberType",
                table: "MemberShips",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MemberType",
                table: "MemberShips");
        }
    }
}
