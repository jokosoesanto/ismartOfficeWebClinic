using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinic.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTreatmentCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TreatmentCatalogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TreatmentCode = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    TreatmentName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    CategoryId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SubCategoryId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ServiceTypeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DefaultPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DurationInMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    RequiresTooth = table.Column<bool>(type: "INTEGER", nullable: false),
                    RequiresSurface = table.Column<bool>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreatmentCatalogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreatmentCatalogs_MasterReferences_ServiceTypeId",
                        column: x => x.ServiceTypeId,
                        principalTable: "MasterReferences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TreatmentCatalogs_TreatmentCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "TreatmentCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TreatmentCatalogs_TreatmentSubCategories_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalTable: "TreatmentSubCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentCatalogs_CategoryId",
                table: "TreatmentCatalogs",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentCatalogs_ServiceTypeId",
                table: "TreatmentCatalogs",
                column: "ServiceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentCatalogs_SubCategoryId_TreatmentName",
                table: "TreatmentCatalogs",
                columns: new[] { "SubCategoryId", "TreatmentName" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentCatalogs_TreatmentCode",
                table: "TreatmentCatalogs",
                column: "TreatmentCode",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TreatmentCatalogs");
        }
    }
}
