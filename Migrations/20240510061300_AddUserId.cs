using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "CashFlows",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "fe49da9e-2e62-4d59-b3b8-8f6ae4afb78f", new DateTime(2024, 5, 10, 11, 42, 59, 539, DateTimeKind.Local).AddTicks(7101), "AQAAAAIAAYagAAAAEMQ46Wid/tLuq/0onxlihqNXLdai8ak3uIKutLb4EKSruIk4IwPZQ+mPazyuiF0jTQ==", "3e646554-1b69-4ce8-b97a-bfe87631d32e" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CashFlows");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c300b859-9a26-40c5-aed3-88bcbdbe5f21", new DateTime(2024, 5, 10, 10, 30, 7, 652, DateTimeKind.Local).AddTicks(2518), "AQAAAAIAAYagAAAAEAAEILaMauRWGTQ1ZUmupHSBxGuliYDR5TWZdtrv7aZr7ZA5ZonjjjmIVo4Fhn7UFQ==", "2ee8fa16-050f-4399-b958-bc530a43ccbc" });
        }
    }
}
