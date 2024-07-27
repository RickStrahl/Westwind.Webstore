using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Westwind.Webstore.Business.Migrations
{
    /// <inheritdoc />
    public partial class Addshippingweighttolineitems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsStockItem",
                table: "LineItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Weight",
                table: "LineItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsStockItem",
                table: "LineItems");`

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "LineItems");
        }
    }
}
