using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddLabelName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "AccountMaster",
                newName: "PaidAmount");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a6f0e356-2902-4280-afd6-1ce3e1bc20bf", new DateTime(2024, 8, 4, 13, 45, 14, 531, DateTimeKind.Local).AddTicks(2704), "AQAAAAIAAYagAAAAENtjm530BwQzb9bW8TRITNQCtTXJXEMbNnuO0d5UH+ekdsmRQaYXWYwxeXkIsqSxvA==", "969e2410-8566-47c2-8275-c94af11b009e" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaidAmount",
                table: "AccountMaster",
                newName: "Amount");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "15540ab8-50a0-4f50-9b3b-b4e3cda7bec7", new DateTime(2024, 6, 14, 12, 27, 51, 301, DateTimeKind.Local).AddTicks(6435), "AQAAAAIAAYagAAAAEO6zjAebCTG0SmSJ66RrTBCjEjQCqcgP06Tr5otdi6ubBcju+FsQ4n4JWF7WeMKJ0w==", "473ebdde-daef-41e9-8100-31ba8bee0b64" });
        }
    }
}
