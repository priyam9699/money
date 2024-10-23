using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceManagement.Migrations
{
    /// <inheritdoc />
    public partial class DeleteBehaviour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Others_CashFlows_CashFlowId",
                table: "Others");

            migrationBuilder.DropForeignKey(
                name: "FK_Upads_CashFlows_CashFlowId",
                table: "Upads");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c2cc092b-7d48-43a2-9ef9-4f97897626d2", new DateTime(2024, 9, 4, 12, 28, 29, 17, DateTimeKind.Local).AddTicks(6592), "AQAAAAIAAYagAAAAEF1osQxgA2fOYQT7YQSsaoC7jZS9r3xkcQ4iTlfHAv0x1eOngIy9C2lLUa7d0eeDeg==", "e572ad0a-834a-4db7-8f2f-201f76876ff5" });

            migrationBuilder.AddForeignKey(
                name: "FK_Others_CashFlows_CashFlowId",
                table: "Others",
                column: "CashFlowId",
                principalTable: "CashFlows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Upads_CashFlows_CashFlowId",
                table: "Upads",
                column: "CashFlowId",
                principalTable: "CashFlows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Others_CashFlows_CashFlowId",
                table: "Others");

            migrationBuilder.DropForeignKey(
                name: "FK_Upads_CashFlows_CashFlowId",
                table: "Upads");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "dc6761fb-fbce-4b79-a4ae-ce11e6870f06", new DateTime(2024, 9, 3, 12, 50, 47, 716, DateTimeKind.Local).AddTicks(7301), "AQAAAAIAAYagAAAAEOPAiMZywNTv1cseb+gvGipVeAGNFxtnbparlTXEom5iKH6mfZWWaP2HVuNyHdDRUA==", "b1f4c013-6da6-4ab0-a5e6-8ad4aff1ab4c" });

            migrationBuilder.AddForeignKey(
                name: "FK_Others_CashFlows_CashFlowId",
                table: "Others",
                column: "CashFlowId",
                principalTable: "CashFlows",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Upads_CashFlows_CashFlowId",
                table: "Upads",
                column: "CashFlowId",
                principalTable: "CashFlows",
                principalColumn: "Id");
        }
    }
}
