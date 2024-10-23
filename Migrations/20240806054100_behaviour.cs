using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceManagement.Migrations
{
    /// <inheritdoc />
    public partial class behaviour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_expenses_CashFlows_CashFlowId",
                table: "expenses");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e49e13e8-aa13-4e6a-a622-8f2a957aa489", new DateTime(2024, 8, 6, 11, 10, 59, 234, DateTimeKind.Local).AddTicks(4657), "AQAAAAIAAYagAAAAEMim+mSlpiF26UveaelCEHs3IV/yJMeKIYYjnCQ5//z8jwo6Y0Hx5ZEQDmqqgwFlqw==", "52fbe7d0-7f1b-44f9-a75c-d3027fc38b36" });

            migrationBuilder.AddForeignKey(
                name: "FK_expenses_CashFlows_CashFlowId",
                table: "expenses",
                column: "CashFlowId",
                principalTable: "CashFlows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_expenses_CashFlows_CashFlowId",
                table: "expenses");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a6f0e356-2902-4280-afd6-1ce3e1bc20bf", new DateTime(2024, 8, 4, 13, 45, 14, 531, DateTimeKind.Local).AddTicks(2704), "AQAAAAIAAYagAAAAENtjm530BwQzb9bW8TRITNQCtTXJXEMbNnuO0d5UH+ekdsmRQaYXWYwxeXkIsqSxvA==", "969e2410-8566-47c2-8275-c94af11b009e" });

            migrationBuilder.AddForeignKey(
                name: "FK_expenses_CashFlows_CashFlowId",
                table: "expenses",
                column: "CashFlowId",
                principalTable: "CashFlows",
                principalColumn: "Id");
        }
    }
}
