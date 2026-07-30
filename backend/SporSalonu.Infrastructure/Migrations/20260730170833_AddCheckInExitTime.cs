using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SporSalonu.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckInExitTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CikisTarihi",
                table: "CheckInLogs",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CikisTarihi",
                table: "CheckInLogs");
        }
    }
}
