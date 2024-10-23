using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceManagement.Migrations
{
    public partial class AddPaymentCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_CashFlows_CashFlowId",
                table: "Payments");

            migrationBuilder.AddColumn<string>(
                name: "PaymentCategory",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true); // Change to nullable: true

            migrationBuilder.AddColumn<string>(
                name: "PaymentCategory",
                table: "CashFlows",
                type: "nvarchar(max)",
                nullable: true); // Change to nullable: true

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9f4951c3-96f5-498a-b307-503255371030", new DateTime(2024, 5, 26, 11, 4, 33, 992, DateTimeKind.Local).AddTicks(5967), "AQAAAAIAAYagAAAAEFvuRaouoaYYCLmszZ+1WxD0a5urvQjFefhFhLhXLBfW2bZ1iGTSq5Hf/GsKm3glIg==", "f453c5e1-91c0-46eb-894b-27f197a22d46" });

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_CashFlows_CashFlowId",
                table: "Payments",
                column: "CashFlowId",
                principalTable: "CashFlows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_CashFlows_CashFlowId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymentCategory",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymentCategory",
                table: "CashFlows");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "171010ea-7cbe-4b46-87ca-e97e1127aa20", new DateTime(2024, 5, 25, 10, 17, 20, 582, DateTimeKind.Local).AddTicks(6063), "AQAAAAIAAYagAAAAEIOknWQJToHsv99c7HPO7aDfezUDlLnFy62ySKi3VUEMWssssJRy8WRo3wdCzWetEw==", "eed147e6-d78a-4f36-8177-e8b73801859d" });

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_CashFlows_CashFlowId",
                table: "Payments",
                column: "CashFlowId",
                principalTable: "CashFlows",
                principalColumn: "Id");
        }
    }
}
