using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddExpenses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "expenses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_expenses", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5c428517-7c3d-482b-8c10-ff619cefda19", new DateTime(2024, 5, 10, 15, 6, 27, 624, DateTimeKind.Local).AddTicks(3555), "AQAAAAIAAYagAAAAENF31VnLR6c1UcGUj0IhaLANWIRmjHVWBbZH4JRHLrd4LzIh9DcHtsAnhZcdCOcjkA==", "f133c1dd-524c-4711-b7b5-9e19c3c9067c" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "expenses");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "fe49da9e-2e62-4d59-b3b8-8f6ae4afb78f", new DateTime(2024, 5, 10, 11, 42, 59, 539, DateTimeKind.Local).AddTicks(7101), "AQAAAAIAAYagAAAAEMQ46Wid/tLuq/0onxlihqNXLdai8ak3uIKutLb4EKSruIk4IwPZQ+mPazyuiF0jTQ==", "3e646554-1b69-4ce8-b97a-bfe87631d32e" });
        }
    }
}
