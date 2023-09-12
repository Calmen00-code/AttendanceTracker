using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceTrackerInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update_ForeignKey_In_WorkdayRecord_4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkdayRecords",
                table: "WorkdayRecords");

            migrationBuilder.DropIndex(
                name: "IX_WorkdayRecords_StaffName",
                table: "WorkdayRecords");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkdayRecords",
                table: "WorkdayRecords",
                column: "StaffName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkdayRecords",
                table: "WorkdayRecords");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkdayRecords",
                table: "WorkdayRecords",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_WorkdayRecords_StaffName",
                table: "WorkdayRecords",
                column: "StaffName");
        }
    }
}
