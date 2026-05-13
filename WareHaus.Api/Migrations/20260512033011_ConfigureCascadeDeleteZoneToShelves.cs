using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WareHaus.Api.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureCascadeDeleteZoneToShelves : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shelves_Zones_ZoneId",
                table: "Shelves");

            migrationBuilder.AddForeignKey(
                name: "FK_Shelves_Zones_ZoneId",
                table: "Shelves",
                column: "ZoneId",
                principalTable: "Zones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shelves_Zones_ZoneId",
                table: "Shelves");

            migrationBuilder.AddForeignKey(
                name: "FK_Shelves_Zones_ZoneId",
                table: "Shelves",
                column: "ZoneId",
                principalTable: "Zones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
