using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinic.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAssistantToAppointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AssistantId",
                table: "Appointments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_AssistantId",
                table: "Appointments",
                column: "AssistantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Users_AssistantId",
                table: "Appointments",
                column: "AssistantId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Users_AssistantId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_AssistantId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "AssistantId",
                table: "Appointments");
        }
    }
}
