using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WareHaus.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddOutboundMergeDevelop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PurchaseOrdersId",
                table: "ReceivingLogs",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OrderDate",
                table: "PurchaseOrders",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "SalesOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SONumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CustomerName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ShippingAddress = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Courier = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OrderDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RequiredDeliveryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TrackingNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PackingTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PackingNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SalesOrderId = table.Column<int>(type: "integer", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TotalPackage = table.Column<int>(type: "integer", nullable: false),
                    PackingStatus = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackingTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PackingTasks_SalesOrders_SalesOrderId",
                        column: x => x.SalesOrderId,
                        principalTable: "SalesOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                name: "SOItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SalesOrderId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    QtyOrdered = table.Column<int>(type: "integer", nullable: false),
                    QtyPicked = table.Column<int>(type: "integer", nullable: false),
                    QtyVerified = table.Column<int>(type: "integer", nullable: false),
                    UnitOfMeasureSnapshot = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SOItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SOItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SOItems_SalesOrders_SalesOrderId",
                        column: x => x.SalesOrderId,
                        principalTable: "SalesOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PackingItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PackingTaskId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    QtyExpected = table.Column<int>(type: "integer", nullable: false),
                    QtyVerified = table.Column<int>(type: "integer", nullable: false),
                    ExpectedBarcode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ScannedBarcode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackingItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PackingItems_PackingTasks_PackingTaskId",
                        column: x => x.PackingTaskId,
                        principalTable: "PackingTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PackingItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Shipments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PackingTaskId = table.Column<int>(type: "integer", nullable: false),
                    SalesOrderId = table.Column<int>(type: "integer", nullable: false),
                    ShippingLabelNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CourierName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TrackingNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ShippingLabelUrl = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CustomerNameSnapshot = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ShippingAddressSnapshot = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ManifestDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shipments_PackingTasks_PackingTaskId",
                        column: x => x.PackingTaskId,
                        principalTable: "PackingTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Shipments_SalesOrders_SalesOrderId",
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
                name: "IX_ReceivingLogs_PurchaseOrdersId",
                table: "ReceivingLogs",
                column: "PurchaseOrdersId");

            migrationBuilder.CreateIndex(
                name: "IX_PackingItems_PackingTaskId",
                table: "PackingItems",
                column: "PackingTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_PackingItems_ProductId",
                table: "PackingItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PackingTasks_PackingNumber",
                table: "PackingTasks",
                column: "PackingNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PackingTasks_SalesOrderId",
                table: "PackingTasks",
                column: "SalesOrderId");

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

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_SONumber",
                table: "SalesOrders",
                column: "SONumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_PackingTaskId",
                table: "Shipments",
                column: "PackingTaskId");

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
                name: "IX_SOItems_ProductId",
                table: "SOItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SOItems_SalesOrderId",
                table: "SOItems",
                column: "SalesOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceivingLogs_PurchaseOrders_PurchaseOrdersId",
                table: "ReceivingLogs",
                column: "PurchaseOrdersId",
                principalTable: "PurchaseOrders",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReceivingLogs_PurchaseOrders_PurchaseOrdersId",
                table: "ReceivingLogs");

            migrationBuilder.DropTable(
                name: "PackingItems");

            migrationBuilder.DropTable(
                name: "PickingItems");

            migrationBuilder.DropTable(
                name: "Shipments");

            migrationBuilder.DropTable(
                name: "SOItems");

            migrationBuilder.DropTable(
                name: "PickingTasks");

            migrationBuilder.DropTable(
                name: "PackingTasks");

            migrationBuilder.DropTable(
                name: "SalesOrders");

            migrationBuilder.DropIndex(
                name: "IX_ReceivingLogs_PurchaseOrdersId",
                table: "ReceivingLogs");

            migrationBuilder.DropColumn(
                name: "PurchaseOrdersId",
                table: "ReceivingLogs");

            migrationBuilder.DropColumn(
                name: "OrderDate",
                table: "PurchaseOrders");
        }
    }
}
