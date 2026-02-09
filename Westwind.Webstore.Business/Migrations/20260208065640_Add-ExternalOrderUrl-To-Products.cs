using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Westwind.Webstore.Business.Migrations
{
    /// <inheritdoc />
    public partial class AddExternalOrderUrlToProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalOrderUrl",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalOrderUrl",
                table: "Products");
        }
    }
}
