using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddTimesheetTableWithEmpIdFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_AspNetUsers_EmpId",
                table: "AspNetUsers",
                column: "EmpId");

            migrationBuilder.CreateTable(
                name: "Timesheets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpId = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    TaskId = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalHours = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Timesheets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Timesheets_AspNetUsers_EmpId",
                        column: x => x.EmpId,
                        principalTable: "AspNetUsers",
                        principalColumn: "EmpId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Timesheets_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_EmpId",
                table: "AspNetUsers",
                column: "EmpId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Timesheets_EmpId",
                table: "Timesheets",
                column: "EmpId");

            migrationBuilder.CreateIndex(
                name: "IX_Timesheets_TaskId",
                table: "Timesheets",
                column: "TaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Timesheets");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_AspNetUsers_EmpId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_EmpId",
                table: "AspNetUsers");
        }
    }
}
