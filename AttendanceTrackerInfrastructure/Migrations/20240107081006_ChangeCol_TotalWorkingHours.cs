using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceTrackerInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCol_TotalWorkingHours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TotalWorkingHours",
                table: "WorkdayRecords",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "TotalWorkingHours",
                table: "WorkdayRecords",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
