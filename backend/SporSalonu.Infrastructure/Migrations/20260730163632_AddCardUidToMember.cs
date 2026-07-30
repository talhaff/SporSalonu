using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SporSalonu.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCardUidToMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CardUid",
                table: "Members",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Members_CardUid",
                table: "Members",
                column: "CardUid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Members_CardUid",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "CardUid",
                table: "Members");
        }
    }
}
