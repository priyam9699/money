using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddFirmNameToUpad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirmName",
                table: "Upads",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9b2ae22b-13d0-419a-be3e-e2cbca63b0cd", new DateTime(2024, 9, 6, 11, 36, 27, 510, DateTimeKind.Local).AddTicks(259), "AQAAAAIAAYagAAAAEEwGAuxqvedhEOnt1Bb1pzM6j/H92qvE4svwunKVoroT2JtmDrX3KzJhtQhnSVZZbA==", "230fc1aa-1860-4f29-89de-35bbd6eafbdf" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirmName",
                table: "Upads");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c2cc092b-7d48-43a2-9ef9-4f97897626d2", new DateTime(2024, 9, 4, 12, 28, 29, 17, DateTimeKind.Local).AddTicks(6592), "AQAAAAIAAYagAAAAEF1osQxgA2fOYQT7YQSsaoC7jZS9r3xkcQ4iTlfHAv0x1eOngIy9C2lLUa7d0eeDeg==", "e572ad0a-834a-4db7-8f2f-201f76876ff5" });
        }
    }
}
