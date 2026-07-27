using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SporSalonu.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGunSayisiToPackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GunSayisi",
                table: "MembershipPackages",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "MembershipPackages",
                keyColumn: "Id",
                keyValue: 1,
                column: "GunSayisi",
                value: 0);

            migrationBuilder.UpdateData(
                table: "MembershipPackages",
                keyColumn: "Id",
                keyValue: 2,
                column: "GunSayisi",
                value: 0);

            migrationBuilder.UpdateData(
                table: "MembershipPackages",
                keyColumn: "Id",
                keyValue: 3,
                column: "GunSayisi",
                value: 0);

            migrationBuilder.UpdateData(
                table: "MembershipPackages",
                keyColumn: "Id",
                keyValue: 4,
                column: "GunSayisi",
                value: 0);

            migrationBuilder.InsertData(
                table: "MembershipPackages",
                columns: new[] { "Id", "Ad", "AySayisi", "Fiyat", "GunSayisi", "IsActive", "OlusturmaTarihi" },
                values: new object[,]
                {
                    { 5, "Günlük Giriş", 0, 100m, 1, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, "Haftalık Üyelik", 0, 400m, 7, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MembershipPackages",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "MembershipPackages",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DropColumn(
                name: "GunSayisi",
                table: "MembershipPackages");
        }
    }
}
