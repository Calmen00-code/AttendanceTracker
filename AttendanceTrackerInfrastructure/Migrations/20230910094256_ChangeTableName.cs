using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceTrackerInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkdayRecord_Staff_StaffName",
                table: "WorkdayRecord");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkdayRecord",
                table: "WorkdayRecord");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Staff",
                table: "Staff");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Admin",
                table: "Admin");

            migrationBuilder.RenameTable(
                name: "WorkdayRecord",
                newName: "WorkdayRecords");

            migrationBuilder.RenameTable(
                name: "Staff",
                newName: "Staffs");

            migrationBuilder.RenameTable(
                name: "Admin",
                newName: "Admins");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkdayRecords",
                table: "WorkdayRecords",
                column: "StaffName");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Staffs",
                table: "Staffs",
                column: "Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Admins",
                table: "Admins",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkdayRecords_Staffs_StaffName",
                table: "WorkdayRecords",
                column: "StaffName",
                principalTable: "Staffs",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkdayRecords_Staffs_StaffName",
                table: "WorkdayRecords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkdayRecords",
                table: "WorkdayRecords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Staffs",
                table: "Staffs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Admins",
                table: "Admins");

            migrationBuilder.RenameTable(
                name: "WorkdayRecords",
                newName: "WorkdayRecord");

            migrationBuilder.RenameTable(
                name: "Staffs",
                newName: "Staff");

            migrationBuilder.RenameTable(
                name: "Admins",
                newName: "Admin");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkdayRecord",
                table: "WorkdayRecord",
                column: "StaffName");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Staff",
                table: "Staff",
                column: "Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Admin",
                table: "Admin",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkdayRecord_Staff_StaffName",
                table: "WorkdayRecord",
                column: "StaffName",
                principalTable: "Staff",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
