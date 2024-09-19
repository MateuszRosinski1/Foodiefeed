using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Foodiefeed_api.Migrations
{
    /// <inheritdoc />
    public partial class followerhotfix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FriendUserId",
                table: "Followers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FriendUserId",
                table: "Followers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
