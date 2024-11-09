using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Foodiefeed_api.Migrations
{
    /// <inheritdoc />
    public partial class products : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Product",
                table: "PostProducts");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "PostProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostProducts_ProductId",
                table: "PostProducts",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_PostProducts_Products_ProductId",
                table: "PostProducts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostProducts_Products_ProductId",
                table: "PostProducts");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropIndex(
                name: "IX_PostProducts_ProductId",
                table: "PostProducts");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "PostProducts");

            migrationBuilder.AddColumn<string>(
                name: "Product",
                table: "PostProducts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
