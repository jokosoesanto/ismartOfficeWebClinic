using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinic.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMinimalInformedConsent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TreatmentConsents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AppointmentTreatmentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsConsentGiven = table.Column<bool>(type: "INTEGER", nullable: false),
                    ConsentedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ConsentedByUserId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreatmentConsents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreatmentConsents_AppointmentTreatments_AppointmentTreatmentId",
                        column: x => x.AppointmentTreatmentId,
                        principalTable: "AppointmentTreatments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TreatmentConsents_Users_ConsentedByUserId",
                        column: x => x.ConsentedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentConsents_AppointmentTreatmentId",
                table: "TreatmentConsents",
                column: "AppointmentTreatmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentConsents_ConsentedByUserId",
                table: "TreatmentConsents",
                column: "ConsentedByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TreatmentConsents");
        }
    }
}
