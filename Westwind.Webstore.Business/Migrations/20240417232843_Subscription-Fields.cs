using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Westwind.Webstore.Business.Migrations
{
    /// <inheritdoc />
    public partial class SubscriptionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubscriptionRenewalDiscountPercent",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SubscriptionRenewalMonths",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SubscriptionRenewalRequestText",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SubscriptionAutoRenewal",
                table: "LineItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubscriptionExpires",
                table: "LineItems",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubscriptionRenewalDiscountPercent",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SubscriptionRenewalMonths",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SubscriptionRenewalRequestText",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SubscriptionAutoRenewal",
                table: "LineItems");

            migrationBuilder.DropColumn(
                name: "SubscriptionExpires",
                table: "LineItems");
        }
    }
}
