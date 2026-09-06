using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinic.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ApplyAppointmentChiefComplaintConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentChiefComplaint_Appointments_AppointmentId",
                table: "AppointmentChiefComplaint");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppointmentChiefComplaint",
                table: "AppointmentChiefComplaint");

            migrationBuilder.RenameTable(
                name: "AppointmentChiefComplaint",
                newName: "AppointmentChiefComplaints");

            migrationBuilder.RenameIndex(
                name: "IX_AppointmentChiefComplaint_AppointmentId",
                table: "AppointmentChiefComplaints",
                newName: "IX_AppointmentChiefComplaints_AppointmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppointmentChiefComplaints",
                table: "AppointmentChiefComplaints",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentChiefComplaints_Appointments_AppointmentId",
                table: "AppointmentChiefComplaints",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentChiefComplaints_Appointments_AppointmentId",
                table: "AppointmentChiefComplaints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppointmentChiefComplaints",
                table: "AppointmentChiefComplaints");

            migrationBuilder.RenameTable(
                name: "AppointmentChiefComplaints",
                newName: "AppointmentChiefComplaint");

            migrationBuilder.RenameIndex(
                name: "IX_AppointmentChiefComplaints_AppointmentId",
                table: "AppointmentChiefComplaint",
                newName: "IX_AppointmentChiefComplaint_AppointmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppointmentChiefComplaint",
                table: "AppointmentChiefComplaint",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentChiefComplaint_Appointments_AppointmentId",
                table: "AppointmentChiefComplaint",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
