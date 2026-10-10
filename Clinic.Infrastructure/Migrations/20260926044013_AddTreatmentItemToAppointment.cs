using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinic.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTreatmentItemToAppointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TreatmentItemId",
                table: "Appointments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_TreatmentItemId",
                table: "Appointments",
                column: "TreatmentItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_TreatmentCatalogs_TreatmentItemId",
                table: "Appointments",
                column: "TreatmentItemId",
                principalTable: "TreatmentCatalogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_TreatmentCatalogs_TreatmentItemId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_TreatmentItemId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "TreatmentItemId",
                table: "Appointments");
        }
    }
}
