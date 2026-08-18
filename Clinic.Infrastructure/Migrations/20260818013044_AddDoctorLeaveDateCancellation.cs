using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinic.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDoctorLeaveDateCancellation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "DoctorLeaveDates",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledAt",
                table: "DoctorLeaveDates",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CancelledBy",
                table: "DoctorLeaveDates",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCancelled",
                table: "DoctorLeaveDates",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "DoctorLeaveDates");

            migrationBuilder.DropColumn(
                name: "CancelledAt",
                table: "DoctorLeaveDates");

            migrationBuilder.DropColumn(
                name: "CancelledBy",
                table: "DoctorLeaveDates");

            migrationBuilder.DropColumn(
                name: "IsCancelled",
                table: "DoctorLeaveDates");
        }
    }
}
