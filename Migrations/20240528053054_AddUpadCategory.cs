using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddUpadCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UpadOption",
                table: "Upads",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UpadCategory",
                table: "CashFlows",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ce5b4619-1b7b-4261-bdfa-05e59a5a383b", new DateTime(2024, 5, 28, 11, 0, 53, 234, DateTimeKind.Local).AddTicks(6678), "AQAAAAIAAYagAAAAEKqLNjFTjk09OzsavWgr5JlU3bYB/ev3fWIuyUM1tl5UFXiv+9OQ3rczNAr3XVXekg==", "dd6e7a51-5915-4ced-995c-a6b102f335b2" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpadOption",
                table: "Upads");

            migrationBuilder.DropColumn(
                name: "UpadCategory",
                table: "CashFlows");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ffa2d68a-ba06-476f-bf96-167d45c76f16", new DateTime(2024, 5, 28, 10, 3, 17, 474, DateTimeKind.Local).AddTicks(2998), "AQAAAAIAAYagAAAAEACIU2mnJCT2PyP/3w8aQmWan8ntaBrS1TFieKCRyCBa8N/nPch74AMvAjEQr1GZHA==", "ffd1f54d-ccf3-4676-875a-de3657fc7828" });
        }
    }
}
