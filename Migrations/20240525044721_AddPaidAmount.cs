using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddPaidAmount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Payments",
                newName: "TotalAmount");

            migrationBuilder.AddColumn<decimal>(
                name: "PaidAmount",
                table: "Payments",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "171010ea-7cbe-4b46-87ca-e97e1127aa20", new DateTime(2024, 5, 25, 10, 17, 20, 582, DateTimeKind.Local).AddTicks(6063), "AQAAAAIAAYagAAAAEIOknWQJToHsv99c7HPO7aDfezUDlLnFy62ySKi3VUEMWssssJRy8WRo3wdCzWetEw==", "eed147e6-d78a-4f36-8177-e8b73801859d" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaidAmount",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                table: "Payments",
                newName: "Amount");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5c4c717e-0edd-4cda-b974-5f285126d773", new DateTime(2024, 5, 24, 13, 43, 46, 461, DateTimeKind.Local).AddTicks(5515), "AQAAAAIAAYagAAAAEKq+gpu1fRHct5wLvU/t2xnl/ok8YlNd9hPMF+kU31SIDsYsqS7KqUQgzAwnTDBQXg==", "23f3203b-1e2e-42f5-a9eb-0c5ce38f144c" });
        }
    }
}
