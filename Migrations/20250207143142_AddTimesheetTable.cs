using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddTimesheetTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(150)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EmpId",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldNullable: true);

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
                    TaskId = table.Column<int>(type: "int", nullable: false),
                    EmpId = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalTimeSpent = table.Column<int>(type: "int", nullable: false)
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

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(150)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)");

            migrationBuilder.AlterColumn<string>(
                name: "EmpId",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");
        }
    }
}
