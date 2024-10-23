using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CashFlowId",
                table: "expenses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6b9f3ec6-8c50-427c-8d07-2f16e1a40332", new DateTime(2024, 5, 13, 15, 58, 35, 387, DateTimeKind.Local).AddTicks(756), "AQAAAAIAAYagAAAAEC2c5Qfa1LLsuIQHst3Iy1QItB90gyzq09a7lGz10Xoqx/h7cPkVAd2FUELe3X94cw==", "ee6ab02c-30f4-45b1-b077-24be9464ec58" });

            migrationBuilder.CreateIndex(
                name: "IX_expenses_CashFlowId",
                table: "expenses",
                column: "CashFlowId");

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

            migrationBuilder.DropIndex(
                name: "IX_expenses_CashFlowId",
                table: "expenses");

            migrationBuilder.DropColumn(
                name: "CashFlowId",
                table: "expenses");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5c428517-7c3d-482b-8c10-ff619cefda19", new DateTime(2024, 5, 10, 15, 6, 27, 624, DateTimeKind.Local).AddTicks(3555), "AQAAAAIAAYagAAAAENF31VnLR6c1UcGUj0IhaLANWIRmjHVWBbZH4JRHLrd4LzIh9DcHtsAnhZcdCOcjkA==", "f133c1dd-524c-4711-b7b5-9e19c3c9067c" });
        }
    }
}
