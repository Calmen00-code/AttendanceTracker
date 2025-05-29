using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceTrackerWeb.Migrations
{
    /// <inheritdoc />
    public partial class ModifyDailyRecordTableToIncludeEmployeeId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmployeeId",
                table: "DailyAttendanceRecords",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_DailyAttendanceRecords_EmployeeId",
                table: "DailyAttendanceRecords",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_DailyAttendanceRecords_AspNetUsers_EmployeeId",
                table: "DailyAttendanceRecords",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyAttendanceRecords_AspNetUsers_EmployeeId",
                table: "DailyAttendanceRecords");

            migrationBuilder.DropIndex(
                name: "IX_DailyAttendanceRecords_EmployeeId",
                table: "DailyAttendanceRecords");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "DailyAttendanceRecords");
        }
    }
}
