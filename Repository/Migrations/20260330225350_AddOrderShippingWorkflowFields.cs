using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderShippingWorkflowFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PreparingAt",
                table: "TBOrder",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessingAt",
                table: "TBOrder",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingLabelUrl",
                table: "TBOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SuperFreteOrderId",
                table: "TBOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrackingUrl",
                table: "TBOrder",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreparingAt",
                table: "TBOrder");

            migrationBuilder.DropColumn(
                name: "ProcessingAt",
                table: "TBOrder");

            migrationBuilder.DropColumn(
                name: "ShippingLabelUrl",
                table: "TBOrder");

            migrationBuilder.DropColumn(
                name: "SuperFreteOrderId",
                table: "TBOrder");

            migrationBuilder.DropColumn(
                name: "TrackingUrl",
                table: "TBOrder");
        }
    }
}
