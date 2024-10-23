using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddFirmName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirmName",
                table: "AccountMaster",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e79f074c-ffdf-4ca7-82d2-a69ae002d48d", new DateTime(2024, 5, 24, 12, 8, 29, 722, DateTimeKind.Local).AddTicks(6159), "AQAAAAIAAYagAAAAENBgOq7ylg6rprEtNfJm8PuIQkHR1ZowTKESMWb29y3n2NBZLrZD8jZ5S7FO+ptM8A==", "6626eb80-361c-49c3-aed0-7f6ea79f9628" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirmName",
                table: "AccountMaster");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "00e652f6-7b7f-43ab-8419-586649a25291", new DateTime(2024, 5, 23, 15, 38, 18, 50, DateTimeKind.Local).AddTicks(2285), "AQAAAAIAAYagAAAAELHAHFHAt9He0UOcMJqqzBqIqtt1J/OZoNVYfz/K+hROCAVnAppNOp+u2YypAnL0ig==", "7b51f58b-7e44-40a2-b144-516898c96185" });
        }
    }
}
