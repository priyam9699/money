using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddDatatype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PaymentCategory",
                table: "CashFlows",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ffa2d68a-ba06-476f-bf96-167d45c76f16", new DateTime(2024, 5, 28, 10, 3, 17, 474, DateTimeKind.Local).AddTicks(2998), "AQAAAAIAAYagAAAAEACIU2mnJCT2PyP/3w8aQmWan8ntaBrS1TFieKCRyCBa8N/nPch74AMvAjEQr1GZHA==", "ffd1f54d-ccf3-4676-875a-de3657fc7828" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PaymentCategory",
                table: "CashFlows",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9f4951c3-96f5-498a-b307-503255371030", new DateTime(2024, 5, 26, 11, 4, 33, 992, DateTimeKind.Local).AddTicks(5967), "AQAAAAIAAYagAAAAEFvuRaouoaYYCLmszZ+1WxD0a5urvQjFefhFhLhXLBfW2bZ1iGTSq5Hf/GsKm3glIg==", "f453c5e1-91c0-46eb-894b-27f197a22d46" });
        }
    }
}
