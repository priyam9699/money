using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddDeleteBehaviour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_expenses_AccountMaster_AccountMasterId",
                table: "expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_AccountMaster_AccountMasterId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Upads_AccountMaster_AccountMasterId",
                table: "Upads");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "15540ab8-50a0-4f50-9b3b-b4e3cda7bec7", new DateTime(2024, 6, 14, 12, 27, 51, 301, DateTimeKind.Local).AddTicks(6435), "AQAAAAIAAYagAAAAEO6zjAebCTG0SmSJ66RrTBCjEjQCqcgP06Tr5otdi6ubBcju+FsQ4n4JWF7WeMKJ0w==", "473ebdde-daef-41e9-8100-31ba8bee0b64" });

            migrationBuilder.AddForeignKey(
                name: "FK_expenses_AccountMaster_AccountMasterId",
                table: "expenses",
                column: "AccountMasterId",
                principalTable: "AccountMaster",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_AccountMaster_AccountMasterId",
                table: "Payments",
                column: "AccountMasterId",
                principalTable: "AccountMaster",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Upads_AccountMaster_AccountMasterId",
                table: "Upads",
                column: "AccountMasterId",
                principalTable: "AccountMaster",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_expenses_AccountMaster_AccountMasterId",
                table: "expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_AccountMaster_AccountMasterId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Upads_AccountMaster_AccountMasterId",
                table: "Upads");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4a3cbd2e-d743-496e-a3b5-3459954cf7e4", new DateTime(2024, 6, 8, 11, 42, 34, 904, DateTimeKind.Local).AddTicks(1974), "AQAAAAIAAYagAAAAEGD2ohWNqjJUUdMAydFhQ2vOOCbO+sf7/7waSn5sZxTVuLNfRv5TWolKOLCHBJbjDw==", "951e8b97-1897-4c66-8acc-c5c4337df2f2" });

            migrationBuilder.AddForeignKey(
                name: "FK_expenses_AccountMaster_AccountMasterId",
                table: "expenses",
                column: "AccountMasterId",
                principalTable: "AccountMaster",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_AccountMaster_AccountMasterId",
                table: "Payments",
                column: "AccountMasterId",
                principalTable: "AccountMaster",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Upads_AccountMaster_AccountMasterId",
                table: "Upads",
                column: "AccountMasterId",
                principalTable: "AccountMaster",
                principalColumn: "Id");
        }
    }
}
