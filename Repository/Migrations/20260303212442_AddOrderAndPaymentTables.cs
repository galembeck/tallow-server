using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderAndPaymentTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TBOrder",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SubTotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ShippingAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ShippingAddressZipcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShippingAddressState = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShippingAddressCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShippingAddressNeighborhood = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShippingAddressStreet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShippingAddressNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShippingAddressComplement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BuyerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BuyerEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BuyerCellphone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BuyerDocument = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrackingCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShippingService = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ShippedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CustomerNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdminNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBOrder_TBUser_UserId",
                        column: x => x.UserId,
                        principalTable: "TBUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TBOrderItem",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrderId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBOrderItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBOrderItem_TBOrder_OrderId",
                        column: x => x.OrderId,
                        principalTable: "TBOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TBOrderItem_TBProduct_Prod~",
                        column: x => x.ProductId,
                        principalTable: "TBProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TBPayment",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrderId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MercadoPagoPaymentId = table.Column<long>(type: "bigint", nullable: true),
                    MercadoPagoPaymentMethodId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentTypeId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentMethod = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TransactionAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Installments = table.Column<int>(type: "int", nullable: true),
                    ExternalReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusDetail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateApproved = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateOfExpiration = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PixQrCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PixQrCodeBase64 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PixCopyPaste = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BoletoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BoletoBarcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RawMercadoPagoResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBPayment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBPayment_TBOrder_OrderId",
                        column: x => x.OrderId,
                        principalTable: "TBOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TBPayment_TBUser_UserId",
                        column: x => x.UserId,
                        principalTable: "TBUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBOrder_UserId",
                table: "TBOrder",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TBOrderItem_OrderId",
                table: "TBOrderItem",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_TBOrderItem_ProductId",
                table: "TBOrderItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_TBPayment_MercadoPagoPayme~",
                table: "TBPayment",
                column: "MercadoPagoPaymentId",
                unique: true,
                filter: "[MercadoPagoPaymentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TBPayment_OrderId",
                table: "TBPayment",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_TBPayment_UserId",
                table: "TBPayment",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBOrderItem");

            migrationBuilder.DropTable(
                name: "TBPayment");

            migrationBuilder.DropTable(
                name: "TBOrder");
        }
    }
}
