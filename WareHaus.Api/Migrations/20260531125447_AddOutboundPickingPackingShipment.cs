using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WareHaus.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddOutboundPickingPackingShipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PackingTasks_SalesOrders_SOId",
                table: "PackingTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_POItems_Products_ProductId",
                table: "POItems");

            migrationBuilder.DropForeignKey(
                name: "FK_POItems_PurchaseOrders_PurchaseOrderId",
                table: "POItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceivingLogs_POItems_POItemId",
                table: "ReceivingLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceivingLogs_PurchaseOrders_PurchaseOrderId",
                table: "ReceivingLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Shelves_Zones_ZoneId",
                table: "Shelves");

            migrationBuilder.DropForeignKey(
                name: "FK_SOItems_SalesOrders_SOId",
                table: "SOItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_Products_ProductId",
                table: "Stocks");

            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_Shelves_ShelfId",
                table: "Stocks");

            migrationBuilder.DropIndex(
                name: "IX_Zones_ZoneCode",
                table: "Zones");

            migrationBuilder.DropIndex(
                name: "IX_Stocks_ProductId_ShelfId",
                table: "Stocks");

            migrationBuilder.DropIndex(
                name: "IX_Stocks_ShelfId",
                table: "Stocks");

            migrationBuilder.DropIndex(
                name: "IX_Shelves_ShelfCode",
                table: "Shelves");

            migrationBuilder.DropIndex(
                name: "IX_Shelves_ZoneId",
                table: "Shelves");

            migrationBuilder.DropIndex(
                name: "IX_ReceivingLogs_POItemId",
                table: "ReceivingLogs");

            migrationBuilder.DropIndex(
                name: "IX_ReceivingLogs_PurchaseOrderId",
                table: "ReceivingLogs");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrders_PONumber",
                table: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_Products_Barcode",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_SKU",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_POItems_ProductId",
                table: "POItems");

            migrationBuilder.DropIndex(
                name: "IX_POItems_PurchaseOrderId",
                table: "POItems");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Shelves",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Zones",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.RenameColumn(
                name: "SOId",
                table: "SOItems",
                newName: "SalesOrderId");

            migrationBuilder.RenameIndex(
                name: "IX_SOItems_SOId",
                table: "SOItems",
                newName: "IX_SOItems_SalesOrderId");

            migrationBuilder.RenameColumn(
                name: "SOId",
                table: "PackingTasks",
                newName: "SalesOrderId");

            migrationBuilder.RenameIndex(
                name: "IX_PackingTasks_SOId",
                table: "PackingTasks",
                newName: "IX_PackingTasks_SalesOrderId");

            migrationBuilder.AlterColumn<string>(
                name: "ZoneName",
                table: "Zones",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "ZoneCode",
                table: "Zones",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Zones",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Zones",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "ProductsId",
                table: "Stocks",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShelvesId",
                table: "Stocks",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductsId",
                table: "SOItems",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QtyVerified",
                table: "SOItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UnitOfMeasureSnapshot",
                table: "SOItems",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "ShippingLabelUrl",
                table: "Shipments",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "CourierName",
                table: "Shipments",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "CustomerNameSnapshot",
                table: "Shipments",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SalesOrderId",
                table: "Shipments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddressSnapshot",
                table: "Shipments",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShippingLabelNumber",
                table: "Shipments",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "ShelfCode",
                table: "Shelves",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "QRCodePath",
                table: "Shelves",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Aisle",
                table: "Shelves",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "ZonesId",
                table: "Shelves",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Courier",
                table: "SalesOrders",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RequiredDeliveryDate",
                table: "SalesOrders",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress",
                table: "SalesOrders",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TrackingNumber",
                table: "SalesOrders",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Condition",
                table: "ReceivingLogs",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "POItemsId",
                table: "ReceivingLogs",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PurchaseOrdersId",
                table: "ReceivingLogs",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SupplierName",
                table: "PurchaseOrders",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "PurchaseOrders",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "PONumber",
                table: "PurchaseOrders",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "UnitOfMeasure",
                table: "Products",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "SKU",
                table: "Products",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "ProductName",
                table: "Products",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "Barcode",
                table: "Products",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductsId",
                table: "POItems",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PurchaseOrdersId",
                table: "POItems",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PackingNumber",
                table: "PackingTasks",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExpectedBarcode",
                table: "PackingItems",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "PackingItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ProductsId",
                table: "PackingItems",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QtyExpected",
                table: "PackingItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ScannedBarcode",
                table: "PackingItems",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedAt",
                table: "PackingItems",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PickingTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PickingNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SalesOrderId = table.Column<int>(type: "integer", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TotalItems = table.Column<int>(type: "integer", nullable: false),
                    PickingStatus = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickingTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PickingTasks_SalesOrders_SalesOrderId",
                        column: x => x.SalesOrderId,
                        principalTable: "SalesOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PickingItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PickingTaskId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    ShelfId = table.Column<int>(type: "integer", nullable: false),
                    QtyToPick = table.Column<int>(type: "integer", nullable: false),
                    QtyPicked = table.Column<int>(type: "integer", nullable: false),
                    UnitOfMeasureSnapshot = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    LocationSuggestion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    IsShelfVerified = table.Column<bool>(type: "boolean", nullable: false),
                    ScannedShelfQrCode = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    VerifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickingItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PickingItems_PickingTasks_PickingTaskId",
                        column: x => x.PickingTaskId,
                        principalTable: "PickingTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PickingItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PickingItems_Shelves_ShelfId",
                        column: x => x.ShelfId,
                        principalTable: "Shelves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_ProductsId",
                table: "Stocks",
                column: "ProductsId");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_ShelvesId",
                table: "Stocks",
                column: "ShelvesId");

            migrationBuilder.CreateIndex(
                name: "IX_SOItems_ProductsId",
                table: "SOItems",
                column: "ProductsId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_SalesOrderId",
                table: "Shipments",
                column: "SalesOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_ShippingLabelNumber",
                table: "Shipments",
                column: "ShippingLabelNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shelves_ZonesId",
                table: "Shelves",
                column: "ZonesId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceivingLogs_POItemsId",
                table: "ReceivingLogs",
                column: "POItemsId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceivingLogs_PurchaseOrdersId",
                table: "ReceivingLogs",
                column: "PurchaseOrdersId");

            migrationBuilder.CreateIndex(
                name: "IX_POItems_ProductsId",
                table: "POItems",
                column: "ProductsId");

            migrationBuilder.CreateIndex(
                name: "IX_POItems_PurchaseOrdersId",
                table: "POItems",
                column: "PurchaseOrdersId");

            migrationBuilder.CreateIndex(
                name: "IX_PackingTasks_PackingNumber",
                table: "PackingTasks",
                column: "PackingNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PackingItems_ProductsId",
                table: "PackingItems",
                column: "ProductsId");

            migrationBuilder.CreateIndex(
                name: "IX_PickingItems_PickingTaskId",
                table: "PickingItems",
                column: "PickingTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_PickingItems_ProductId",
                table: "PickingItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PickingItems_ShelfId",
                table: "PickingItems",
                column: "ShelfId");

            migrationBuilder.CreateIndex(
                name: "IX_PickingTasks_PickingNumber",
                table: "PickingTasks",
                column: "PickingNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PickingTasks_SalesOrderId",
                table: "PickingTasks",
                column: "SalesOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_PackingItems_Products_ProductsId",
                table: "PackingItems",
                column: "ProductsId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PackingTasks_SalesOrders_SalesOrderId",
                table: "PackingTasks",
                column: "SalesOrderId",
                principalTable: "SalesOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_POItems_Products_ProductsId",
                table: "POItems",
                column: "ProductsId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_POItems_PurchaseOrders_PurchaseOrdersId",
                table: "POItems",
                column: "PurchaseOrdersId",
                principalTable: "PurchaseOrders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceivingLogs_POItems_POItemsId",
                table: "ReceivingLogs",
                column: "POItemsId",
                principalTable: "POItems",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceivingLogs_PurchaseOrders_PurchaseOrdersId",
                table: "ReceivingLogs",
                column: "PurchaseOrdersId",
                principalTable: "PurchaseOrders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Shelves_Zones_ZonesId",
                table: "Shelves",
                column: "ZonesId",
                principalTable: "Zones",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Shipments_SalesOrders_SalesOrderId",
                table: "Shipments",
                column: "SalesOrderId",
                principalTable: "SalesOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SOItems_Products_ProductsId",
                table: "SOItems",
                column: "ProductsId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SOItems_SalesOrders_SalesOrderId",
                table: "SOItems",
                column: "SalesOrderId",
                principalTable: "SalesOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_Products_ProductsId",
                table: "Stocks",
                column: "ProductsId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_Shelves_ShelvesId",
                table: "Stocks",
                column: "ShelvesId",
                principalTable: "Shelves",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PackingItems_Products_ProductsId",
                table: "PackingItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PackingTasks_SalesOrders_SalesOrderId",
                table: "PackingTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_POItems_Products_ProductsId",
                table: "POItems");

            migrationBuilder.DropForeignKey(
                name: "FK_POItems_PurchaseOrders_PurchaseOrdersId",
                table: "POItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceivingLogs_POItems_POItemsId",
                table: "ReceivingLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceivingLogs_PurchaseOrders_PurchaseOrdersId",
                table: "ReceivingLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Shelves_Zones_ZonesId",
                table: "Shelves");

            migrationBuilder.DropForeignKey(
                name: "FK_Shipments_SalesOrders_SalesOrderId",
                table: "Shipments");

            migrationBuilder.DropForeignKey(
                name: "FK_SOItems_Products_ProductsId",
                table: "SOItems");

            migrationBuilder.DropForeignKey(
                name: "FK_SOItems_SalesOrders_SalesOrderId",
                table: "SOItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_Products_ProductsId",
                table: "Stocks");

            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_Shelves_ShelvesId",
                table: "Stocks");

            migrationBuilder.DropTable(
                name: "PickingItems");

            migrationBuilder.DropTable(
                name: "PickingTasks");

            migrationBuilder.DropIndex(
                name: "IX_Stocks_ProductsId",
                table: "Stocks");

            migrationBuilder.DropIndex(
                name: "IX_Stocks_ShelvesId",
                table: "Stocks");

            migrationBuilder.DropIndex(
                name: "IX_SOItems_ProductsId",
                table: "SOItems");

            migrationBuilder.DropIndex(
                name: "IX_Shipments_SalesOrderId",
                table: "Shipments");

            migrationBuilder.DropIndex(
                name: "IX_Shipments_ShippingLabelNumber",
                table: "Shipments");

            migrationBuilder.DropIndex(
                name: "IX_Shelves_ZonesId",
                table: "Shelves");

            migrationBuilder.DropIndex(
                name: "IX_ReceivingLogs_POItemsId",
                table: "ReceivingLogs");

            migrationBuilder.DropIndex(
                name: "IX_ReceivingLogs_PurchaseOrdersId",
                table: "ReceivingLogs");

            migrationBuilder.DropIndex(
                name: "IX_POItems_ProductsId",
                table: "POItems");

            migrationBuilder.DropIndex(
                name: "IX_POItems_PurchaseOrdersId",
                table: "POItems");

            migrationBuilder.DropIndex(
                name: "IX_PackingTasks_PackingNumber",
                table: "PackingTasks");

            migrationBuilder.DropIndex(
                name: "IX_PackingItems_ProductsId",
                table: "PackingItems");

            migrationBuilder.DropColumn(
                name: "ProductsId",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "ShelvesId",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "ProductsId",
                table: "SOItems");

            migrationBuilder.DropColumn(
                name: "QtyVerified",
                table: "SOItems");

            migrationBuilder.DropColumn(
                name: "UnitOfMeasureSnapshot",
                table: "SOItems");

            migrationBuilder.DropColumn(
                name: "CustomerNameSnapshot",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "SalesOrderId",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "ShippingAddressSnapshot",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "ShippingLabelNumber",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "ZonesId",
                table: "Shelves");

            migrationBuilder.DropColumn(
                name: "Courier",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "RequiredDeliveryDate",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "ShippingAddress",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "TrackingNumber",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "POItemsId",
                table: "ReceivingLogs");

            migrationBuilder.DropColumn(
                name: "PurchaseOrdersId",
                table: "ReceivingLogs");

            migrationBuilder.DropColumn(
                name: "ProductsId",
                table: "POItems");

            migrationBuilder.DropColumn(
                name: "PurchaseOrdersId",
                table: "POItems");

            migrationBuilder.DropColumn(
                name: "PackingNumber",
                table: "PackingTasks");

            migrationBuilder.DropColumn(
                name: "ExpectedBarcode",
                table: "PackingItems");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "PackingItems");

            migrationBuilder.DropColumn(
                name: "ProductsId",
                table: "PackingItems");

            migrationBuilder.DropColumn(
                name: "QtyExpected",
                table: "PackingItems");

            migrationBuilder.DropColumn(
                name: "ScannedBarcode",
                table: "PackingItems");

            migrationBuilder.DropColumn(
                name: "VerifiedAt",
                table: "PackingItems");

            migrationBuilder.RenameColumn(
                name: "SalesOrderId",
                table: "SOItems",
                newName: "SOId");

            migrationBuilder.RenameIndex(
                name: "IX_SOItems_SalesOrderId",
                table: "SOItems",
                newName: "IX_SOItems_SOId");

            migrationBuilder.RenameColumn(
                name: "SalesOrderId",
                table: "PackingTasks",
                newName: "SOId");

            migrationBuilder.RenameIndex(
                name: "IX_PackingTasks_SalesOrderId",
                table: "PackingTasks",
                newName: "IX_PackingTasks_SOId");

            migrationBuilder.AlterColumn<string>(
                name: "ZoneName",
                table: "Zones",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ZoneCode",
                table: "Zones",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Zones",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Zones",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ShippingLabelUrl",
                table: "Shipments",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CourierName",
                table: "Shipments",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "ShelfCode",
                table: "Shelves",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "QRCodePath",
                table: "Shelves",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Aisle",
                table: "Shelves",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Condition",
                table: "ReceivingLogs",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "SupplierName",
                table: "PurchaseOrders",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "PurchaseOrders",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "PONumber",
                table: "PurchaseOrders",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "UnitOfMeasure",
                table: "Products",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "SKU",
                table: "Products",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ProductName",
                table: "Products",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Barcode",
                table: "Products",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Barcode", "CreatedAt", "DeletedAt", "ProductName", "SKU", "UnitOfMeasure", "UpdatedAt" },
                values: new object[] { 1, "899000000001", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Sample Product", "PRD-001", "pcs", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "Zones",
                columns: new[] { "Id", "Category", "CreatedAt", "DeletedAt", "Description", "LevelPerShelf", "ShelfPerAisle", "TotalAisle", "UpdatedAt", "ZoneCode", "ZoneName" },
                values: new object[] { 1, "General", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Sample zone untuk testing", 1, 1, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ZONE-A", "Zone A" });

            migrationBuilder.InsertData(
                table: "Shelves",
                columns: new[] { "Id", "Aisle", "Capacity", "CreatedAt", "CurrentVolume", "DeletedAt", "QRCodePath", "ShelfCode", "UpdatedAt", "ZoneId" },
                values: new object[] { 1, "A1", 100, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, null, null, "SH-A1-001", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1 });

            migrationBuilder.CreateIndex(
                name: "IX_Zones_ZoneCode",
                table: "Zones",
                column: "ZoneCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_ProductId_ShelfId",
                table: "Stocks",
                columns: new[] { "ProductId", "ShelfId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_ShelfId",
                table: "Stocks",
                column: "ShelfId");

            migrationBuilder.CreateIndex(
                name: "IX_Shelves_ShelfCode",
                table: "Shelves",
                column: "ShelfCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shelves_ZoneId",
                table: "Shelves",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceivingLogs_POItemId",
                table: "ReceivingLogs",
                column: "POItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceivingLogs_PurchaseOrderId",
                table: "ReceivingLogs",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_PONumber",
                table: "PurchaseOrders",
                column: "PONumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_Barcode",
                table: "Products",
                column: "Barcode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_SKU",
                table: "Products",
                column: "SKU",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_POItems_ProductId",
                table: "POItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_POItems_PurchaseOrderId",
                table: "POItems",
                column: "PurchaseOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_PackingTasks_SalesOrders_SOId",
                table: "PackingTasks",
                column: "SOId",
                principalTable: "SalesOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_POItems_Products_ProductId",
                table: "POItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_POItems_PurchaseOrders_PurchaseOrderId",
                table: "POItems",
                column: "PurchaseOrderId",
                principalTable: "PurchaseOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceivingLogs_POItems_POItemId",
                table: "ReceivingLogs",
                column: "POItemId",
                principalTable: "POItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceivingLogs_PurchaseOrders_PurchaseOrderId",
                table: "ReceivingLogs",
                column: "PurchaseOrderId",
                principalTable: "PurchaseOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Shelves_Zones_ZoneId",
                table: "Shelves",
                column: "ZoneId",
                principalTable: "Zones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SOItems_SalesOrders_SOId",
                table: "SOItems",
                column: "SOId",
                principalTable: "SalesOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_Products_ProductId",
                table: "Stocks",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_Shelves_ShelfId",
                table: "Stocks",
                column: "ShelfId",
                principalTable: "Shelves",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
