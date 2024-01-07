using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceTrackerInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCol_TotalWorkingHours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "TotalWorkingHours",
                table: "WorkdayRecords",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalWorkingHours",
                table: "WorkdayRecords");
        }
    }
}
