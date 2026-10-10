using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinic.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryBatchFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InventoryBatchId",
                table: "InventoryStockTransactions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InventoryBatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    InventoryItemId = table.Column<Guid>(type: "TEXT", nullable: false),
                    BatchNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryBatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryBatches_InventoryItems_InventoryItemId",
                        column: x => x.InventoryItemId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryStockTransactions_InventoryBatchId",
                table: "InventoryStockTransactions",
                column: "InventoryBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryBatches_InventoryItemId_BatchNumber",
                table: "InventoryBatches",
                columns: new[] { "InventoryItemId", "BatchNumber" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryStockTransactions_InventoryBatches_InventoryBatchId",
                table: "InventoryStockTransactions",
                column: "InventoryBatchId",
                principalTable: "InventoryBatches",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryStockTransactions_InventoryBatches_InventoryBatchId",
                table: "InventoryStockTransactions");

            migrationBuilder.DropTable(
                name: "InventoryBatches");

            migrationBuilder.DropIndex(
                name: "IX_InventoryStockTransactions_InventoryBatchId",
                table: "InventoryStockTransactions");

            migrationBuilder.DropColumn(
                name: "InventoryBatchId",
                table: "InventoryStockTransactions");
        }
    }
}
