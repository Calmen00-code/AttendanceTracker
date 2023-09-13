using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceTrackerInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update_ForeignKey_In_WorkdayRecord_8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkdayRecords",
                table: "WorkdayRecords");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "WorkdayRecords",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("Relational:ColumnOrder", 2);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkdayRecords",
                table: "WorkdayRecords",
                columns: new[] { "Date", "StaffName" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkdayRecords_Id",
                table: "WorkdayRecords",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkdayRecords",
                table: "WorkdayRecords");

            migrationBuilder.DropIndex(
                name: "IX_WorkdayRecords_Id",
                table: "WorkdayRecords");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "WorkdayRecords");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkdayRecords",
                table: "WorkdayRecords",
                column: "Date");
        }
    }
}
