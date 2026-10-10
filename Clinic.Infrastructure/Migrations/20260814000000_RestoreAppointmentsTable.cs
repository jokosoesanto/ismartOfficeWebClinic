using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinic.Infrastructure.Migrations
{
    public partial class RestoreAppointmentsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Backfill migration to safely create the Appointments table if missing.
            // Uses IF NOT EXISTS to ensure it acts as a no-op on the existing BEFORE database.
            
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS ""Appointments"" (
                    ""Id"" TEXT NOT NULL CONSTRAINT ""PK_Appointments"" PRIMARY KEY,
                    ""PatientId"" TEXT NOT NULL,
                    ""DoctorId"" TEXT NOT NULL,
                    ""LocationId"" TEXT NOT NULL,
                    ""ChairId"" TEXT NOT NULL,
                    ""Date"" TEXT NOT NULL,
                    ""StartTime"" TEXT NOT NULL,
                    ""EndTime"" TEXT NOT NULL,
                    ""Status"" TEXT NOT NULL,
                    ""Notes"" TEXT NULL,
                    ""IsDeleted"" INTEGER NOT NULL,
                    ""CreatedAt"" TEXT NOT NULL,
                    ""CreatedBy"" TEXT NULL,
                    ""UpdatedAt"" TEXT NULL,
                    ""UpdatedBy"" TEXT NULL,
                    ""DeletedAt"" TEXT NULL,
                    ""DeletedBy"" TEXT NULL,
                    CONSTRAINT ""FK_Appointments_Chairs_ChairId"" FOREIGN KEY (""ChairId"") REFERENCES ""Chairs"" (""Id"") ON DELETE RESTRICT,
                    CONSTRAINT ""FK_Appointments_Doctors_DoctorId"" FOREIGN KEY (""DoctorId"") REFERENCES ""Doctors"" (""Id"") ON DELETE RESTRICT,
                    CONSTRAINT ""FK_Appointments_Locations_LocationId"" FOREIGN KEY (""LocationId"") REFERENCES ""Locations"" (""Id"") ON DELETE RESTRICT,
                    CONSTRAINT ""FK_Appointments_Patients_PatientId"" FOREIGN KEY (""PatientId"") REFERENCES ""Patients"" (""Id"") ON DELETE RESTRICT
                );
            ");

            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_Appointments_ChairId"" ON ""Appointments"" (""ChairId"");");
            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_Appointments_DoctorId"" ON ""Appointments"" (""DoctorId"");");
            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_Appointments_LocationId"" ON ""Appointments"" (""LocationId"");");
            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_Appointments_PatientId"" ON ""Appointments"" (""PatientId"");");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Empty down method. We do not drop the table here to preserve safety of existing data.
        }
    }
}
