using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "040a6c17-ebd6-4860-b26a-b72c9d88486a", new DateTime(2024, 5, 13, 16, 48, 45, 200, DateTimeKind.Local).AddTicks(4884), "AQAAAAIAAYagAAAAEAsCa30y/g1rUwmJAucY/cl6HEw+To6HkSf1kwde4/g4ds8VF46pcyMp0IwX86gezQ==", "17ff106b-8d87-4715-b2a7-2c4e743e4a34" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6b9f3ec6-8c50-427c-8d07-2f16e1a40332", new DateTime(2024, 5, 13, 15, 58, 35, 387, DateTimeKind.Local).AddTicks(756), "AQAAAAIAAYagAAAAEC2c5Qfa1LLsuIQHst3Iy1QItB90gyzq09a7lGz10Xoqx/h7cPkVAd2FUELe3X94cw==", "ee6ab02c-30f4-45b1-b077-24be9464ec58" });
        }
    }
}
