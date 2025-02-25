using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTimeSheet2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Timesheets");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "Timesheets",
                newName: "Date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Timesheets",
                newName: "StartTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "Timesheets",
                type: "datetime2",
                nullable: true);
        }
    }
}
