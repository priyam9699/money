using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddFirmInPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirmName",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4a3cbd2e-d743-496e-a3b5-3459954cf7e4", new DateTime(2024, 6, 8, 11, 42, 34, 904, DateTimeKind.Local).AddTicks(1974), "AQAAAAIAAYagAAAAEGD2ohWNqjJUUdMAydFhQ2vOOCbO+sf7/7waSn5sZxTVuLNfRv5TWolKOLCHBJbjDw==", "951e8b97-1897-4c66-8acc-c5c4337df2f2" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirmName",
                table: "Payments");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5aa6a755-3a29-4962-917c-79863c00edc9", new DateTime(2024, 6, 6, 16, 59, 27, 27, DateTimeKind.Local).AddTicks(4271), "AQAAAAIAAYagAAAAEGQh9+P1If8RyQrpAPda+aqZlVsbAn9LbWwJ7lbN477YtKI6pg9bBOa0h3Wkb4ea/g==", "9e6a8eb1-0d70-4ba6-b357-d30cd22c3fe7" });
        }
    }
}
