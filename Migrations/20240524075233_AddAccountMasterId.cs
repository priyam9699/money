using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountMasterId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_expenses_CashFlows_CashFlowId",
                table: "expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_CashFlows_CashFlowId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Upads_CashFlows_CashFlowId",
                table: "Upads");

            migrationBuilder.AlterColumn<int>(
                name: "CashFlowId",
                table: "Upads",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "AccountMasterId",
                table: "Upads",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CashFlowId",
                table: "Payments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "AccountMasterId",
                table: "Payments",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CashFlowId",
                table: "expenses",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "AccountMasterId",
                table: "expenses",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e8c1ad37-11a7-4d9d-a6f5-31b941c53272", new DateTime(2024, 5, 24, 13, 22, 32, 493, DateTimeKind.Local).AddTicks(4887), "AQAAAAIAAYagAAAAEJIjzGRjcT8jDwI7iuc/1YZv6+S5HxR3pTcvl+DJBwUuR29nGNyG5MIODQHcvP2RiQ==", "5de978ff-2116-416d-9c24-8f5244b6dee7" });

            migrationBuilder.CreateIndex(
                name: "IX_Upads_AccountMasterId",
                table: "Upads",
                column: "AccountMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_AccountMasterId",
                table: "Payments",
                column: "AccountMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_expenses_AccountMasterId",
                table: "expenses",
                column: "AccountMasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_expenses_AccountMaster_AccountMasterId",
                table: "expenses",
                column: "AccountMasterId",
                principalTable: "AccountMaster",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_expenses_CashFlows_CashFlowId",
                table: "expenses",
                column: "CashFlowId",
                principalTable: "CashFlows",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_AccountMaster_AccountMasterId",
                table: "Payments",
                column: "AccountMasterId",
                principalTable: "AccountMaster",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_CashFlows_CashFlowId",
                table: "Payments",
                column: "CashFlowId",
                principalTable: "CashFlows",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Upads_AccountMaster_AccountMasterId",
                table: "Upads",
                column: "AccountMasterId",
                principalTable: "AccountMaster",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Upads_CashFlows_CashFlowId",
                table: "Upads",
                column: "CashFlowId",
                principalTable: "CashFlows",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_expenses_AccountMaster_AccountMasterId",
                table: "expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_expenses_CashFlows_CashFlowId",
                table: "expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_AccountMaster_AccountMasterId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_CashFlows_CashFlowId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Upads_AccountMaster_AccountMasterId",
                table: "Upads");

            migrationBuilder.DropForeignKey(
                name: "FK_Upads_CashFlows_CashFlowId",
                table: "Upads");

            migrationBuilder.DropIndex(
                name: "IX_Upads_AccountMasterId",
                table: "Upads");

            migrationBuilder.DropIndex(
                name: "IX_Payments_AccountMasterId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_expenses_AccountMasterId",
                table: "expenses");

            migrationBuilder.DropColumn(
                name: "AccountMasterId",
                table: "Upads");

            migrationBuilder.DropColumn(
                name: "AccountMasterId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "AccountMasterId",
                table: "expenses");

            migrationBuilder.AlterColumn<int>(
                name: "CashFlowId",
                table: "Upads",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CashFlowId",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CashFlowId",
                table: "expenses",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e79f074c-ffdf-4ca7-82d2-a69ae002d48d", new DateTime(2024, 5, 24, 12, 8, 29, 722, DateTimeKind.Local).AddTicks(6159), "AQAAAAIAAYagAAAAENBgOq7ylg6rprEtNfJm8PuIQkHR1ZowTKESMWb29y3n2NBZLrZD8jZ5S7FO+ptM8A==", "6626eb80-361c-49c3-aed0-7f6ea79f9628" });

            migrationBuilder.AddForeignKey(
                name: "FK_expenses_CashFlows_CashFlowId",
                table: "expenses",
                column: "CashFlowId",
                principalTable: "CashFlows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_CashFlows_CashFlowId",
                table: "Payments",
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
    }
}
