using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Westwind.Webstore.Business.Migrations
{
    /// <inheritdoc />
    public partial class AddressEmailAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Addresses");
        }
    }
}
