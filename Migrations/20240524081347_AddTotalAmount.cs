using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddTotalAmount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "AccountMaster",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5c4c717e-0edd-4cda-b974-5f285126d773", new DateTime(2024, 5, 24, 13, 43, 46, 461, DateTimeKind.Local).AddTicks(5515), "AQAAAAIAAYagAAAAEKq+gpu1fRHct5wLvU/t2xnl/ok8YlNd9hPMF+kU31SIDsYsqS7KqUQgzAwnTDBQXg==", "23f3203b-1e2e-42f5-a9eb-0c5ce38f144c" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "AccountMaster");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e8c1ad37-11a7-4d9d-a6f5-31b941c53272", new DateTime(2024, 5, 24, 13, 22, 32, 493, DateTimeKind.Local).AddTicks(4887), "AQAAAAIAAYagAAAAEJIjzGRjcT8jDwI7iuc/1YZv6+S5HxR3pTcvl+DJBwUuR29nGNyG5MIODQHcvP2RiQ==", "5de978ff-2116-416d-9c24-8f5244b6dee7" });
        }
    }
}
