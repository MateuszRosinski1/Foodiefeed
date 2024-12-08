using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Foodiefeed_api.Migrations
{
    /// <inheritdoc />
    public partial class removeddescpostTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "PostTags");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "PostTags",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
