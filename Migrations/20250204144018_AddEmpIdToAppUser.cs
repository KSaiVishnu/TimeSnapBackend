using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddEmpIdToAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "EmpId",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserEmployees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEmployees", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserEmployees");

            migrationBuilder.DropColumn(
                name: "EmpId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "Assignee",
                table: "Tasks",
                newName: "AssigneeNames");
        }
    }
}
