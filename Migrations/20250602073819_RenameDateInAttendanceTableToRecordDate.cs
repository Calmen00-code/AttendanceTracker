using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceTrackerWeb.Migrations
{
    /// <inheritdoc />
    public partial class RenameDateInAttendanceTableToRecordDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Attendances",
                newName: "RecordDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RecordDate",
                table: "Attendances",
                newName: "Date");
        }
    }
}
