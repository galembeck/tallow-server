using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpodatePaymentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorizationCode",
                table: "TBPayment",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrencyId",
                table: "TBPayment",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateLastUpdated",
                table: "TBPayment",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "LiveMode",
                table: "TBPayment",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ShippingAmount",
                table: "TBPayment",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatementDescriptor",
                table: "TBPayment",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorizationCode",
                table: "TBPayment");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "TBPayment");

            migrationBuilder.DropColumn(
                name: "DateLastUpdated",
                table: "TBPayment");

            migrationBuilder.DropColumn(
                name: "LiveMode",
                table: "TBPayment");

            migrationBuilder.DropColumn(
                name: "ShippingAmount",
                table: "TBPayment");

            migrationBuilder.DropColumn(
                name: "StatementDescriptor",
                table: "TBPayment");
        }
    }
}
