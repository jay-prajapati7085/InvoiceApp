using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvoiceApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnitsPerCrate",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "UnitsPerCrate",
                table: "OrderItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnitsPerCrate",
                table: "OrderItems");

            migrationBuilder.AddColumn<int>(
                name: "UnitsPerCrate",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
