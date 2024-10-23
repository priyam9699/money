using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddUpad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Upads",
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
                    table.PrimaryKey("PK_Upads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Upads_CashFlows_CashFlowId",
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
                values: new object[] { "52f05c08-f917-4765-b26c-7e3620d92a2c", new DateTime(2024, 5, 14, 12, 27, 8, 767, DateTimeKind.Local).AddTicks(67), "AQAAAAIAAYagAAAAEOKRFQe5Ka+wX30CsTTNlUYFQODLgxhyqFW8T28oquZiuHkIcUZX5QIUE9MiMy+fpQ==", "15f1e334-4297-46c2-8f20-1fafed439695" });

            migrationBuilder.CreateIndex(
                name: "IX_Upads_CashFlowId",
                table: "Upads",
                column: "CashFlowId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Upads");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1caf0537-7802-4c6b-9a8b-49fc0eb6fd19", new DateTime(2024, 5, 14, 11, 51, 36, 707, DateTimeKind.Local).AddTicks(1156), "AQAAAAIAAYagAAAAEDVQGc8gR+KZLGtx1U3syItLian4T3VjxLy9fYdacvmhd5zhszDnFkCPXCjkqueZ1Q==", "42445752-f7fe-465a-a06f-45e6b6322bf2" });
        }
    }
}
