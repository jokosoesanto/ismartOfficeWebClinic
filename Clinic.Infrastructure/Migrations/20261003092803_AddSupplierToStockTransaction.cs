using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinic.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplierToStockTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SupplierId",
                table: "InventoryStockTransactions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryStockTransactions_SupplierId",
                table: "InventoryStockTransactions",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryStockTransactions_Suppliers_SupplierId",
                table: "InventoryStockTransactions",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryStockTransactions_Suppliers_SupplierId",
                table: "InventoryStockTransactions");

            migrationBuilder.DropIndex(
                name: "IX_InventoryStockTransactions_SupplierId",
                table: "InventoryStockTransactions");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "InventoryStockTransactions");
        }
    }
}
