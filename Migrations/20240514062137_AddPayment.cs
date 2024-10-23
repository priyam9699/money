using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CashFlowId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_CashFlows_CashFlowId",
                        column: x => x.CashFlowId,
                        principalTable: "CashFlows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1caf0537-7802-4c6b-9a8b-49fc0eb6fd19", new DateTime(2024, 5, 14, 11, 51, 36, 707, DateTimeKind.Local).AddTicks(1156), "AQAAAAIAAYagAAAAEDVQGc8gR+KZLGtx1U3syItLian4T3VjxLy9fYdacvmhd5zhszDnFkCPXCjkqueZ1Q==", "42445752-f7fe-465a-a06f-45e6b6322bf2" });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CashFlowId",
                table: "Payments",
                column: "CashFlowId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "040a6c17-ebd6-4860-b26a-b72c9d88486a", new DateTime(2024, 5, 13, 16, 48, 45, 200, DateTimeKind.Local).AddTicks(4884), "AQAAAAIAAYagAAAAEAsCa30y/g1rUwmJAucY/cl6HEw+To6HkSf1kwde4/g4ds8VF46pcyMp0IwX86gezQ==", "17ff106b-8d87-4715-b2a7-2c4e743e4a34" });
        }
    }
}
