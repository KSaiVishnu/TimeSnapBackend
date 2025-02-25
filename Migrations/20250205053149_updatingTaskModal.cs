using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class updatingTaskModal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EmpId",
                table: "UserEmployees",
                newName: "EmployeeId");

            migrationBuilder.AddColumn<string>(
                name: "EmpId",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmpId",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "UserEmployees",
                newName: "EmpId");
        }
    }
}
