using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderAndPaymentTablesV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShippingAddressCity",
                table: "TBOrder");

            migrationBuilder.DropColumn(
                name: "ShippingAddressComplement",
                table: "TBOrder");

            migrationBuilder.DropColumn(
                name: "ShippingAddressNeighborhood",
                table: "TBOrder");

            migrationBuilder.DropColumn(
                name: "ShippingAddressNumber",
                table: "TBOrder");

            migrationBuilder.DropColumn(
                name: "ShippingAddressState",
                table: "TBOrder");

            migrationBuilder.DropColumn(
                name: "ShippingAddressStreet",
                table: "TBOrder");

            migrationBuilder.RenameColumn(
                name: "ShippingAddressZipcode",
                table: "TBOrder",
                newName: "ShippingComplement");

            migrationBuilder.AlterColumn<string>(
                name: "ShippingService",
                table: "TBOrder",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BuyerName",
                table: "TBOrder",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BuyerEmail",
                table: "TBOrder",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BuyerDocument",
                table: "TBOrder",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BuyerCellphone",
                table: "TBOrder",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress",
                table: "TBOrder",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShippingCity",
                table: "TBOrder",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShippingDeliveryTime",
                table: "TBOrder",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShippingNeighborhood",
                table: "TBOrder",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShippingNumber",
                table: "TBOrder",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShippingState",
                table: "TBOrder",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShippingZipcode",
                table: "TBOrder",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShippingAddress",
                table: "TBOrder");

            migrationBuilder.DropColumn(
                name: "ShippingCity",
                table: "TBOrder");

            migrationBuilder.DropColumn(
                name: "ShippingDeliveryTime",
                table: "TBOrder");

            migrationBuilder.DropColumn(
                name: "ShippingNeighborhood",
                table: "TBOrder");

            migrationBuilder.DropColumn(
                name: "ShippingNumber",
                table: "TBOrder");

            migrationBuilder.DropColumn(
                name: "ShippingState",
                table: "TBOrder");

            migrationBuilder.DropColumn(
                name: "ShippingZipcode",
                table: "TBOrder");

            migrationBuilder.RenameColumn(
                name: "ShippingComplement",
                table: "TBOrder",
                newName: "ShippingAddressZipcode");

            migrationBuilder.AlterColumn<string>(
                name: "ShippingService",
                table: "TBOrder",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "BuyerName",
                table: "TBOrder",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "BuyerEmail",
                table: "TBOrder",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "BuyerDocument",
                table: "TBOrder",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "BuyerCellphone",
                table: "TBOrder",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddressCity",
                table: "TBOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddressComplement",
                table: "TBOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddressNeighborhood",
                table: "TBOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddressNumber",
                table: "TBOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddressState",
                table: "TBOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddressStreet",
                table: "TBOrder",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
