using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Westwind.Webstore.Business.Migrations
{
    /// <inheritdoc />
    public partial class ProductsRemoveIsPhysical : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPhysical",
                table: "Products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPhysical",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
