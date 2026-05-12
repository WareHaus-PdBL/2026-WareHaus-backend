using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WareHaus.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAisleTypeAndAddCapacity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "QRCodePath",
                table: "Shelves",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<int>(
                name: "CurrentCapacity",
                table: "Shelves",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxCapacity",
                table: "Shelves",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentCapacity",
                table: "Shelves");

            migrationBuilder.DropColumn(
                name: "MaxCapacity",
                table: "Shelves");

            migrationBuilder.AlterColumn<string>(
                name: "QRCodePath",
                table: "Shelves",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);
        }
    }
}
