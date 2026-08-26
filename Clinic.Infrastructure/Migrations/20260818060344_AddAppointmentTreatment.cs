using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinic.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAppointmentTreatment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppointmentTreatments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AppointmentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TreatmentItemId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SiteNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    SiteDetail = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    ActualPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Remark = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentTreatments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppointmentTreatments_Appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppointmentTreatments_TreatmentCatalogs_TreatmentItemId",
                        column: x => x.TreatmentItemId,
                        principalTable: "TreatmentCatalogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentTreatments_AppointmentId",
                table: "AppointmentTreatments",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentTreatments_TreatmentItemId",
                table: "AppointmentTreatments",
                column: "TreatmentItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentTreatments");
        }
    }
}
