using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountMasterId",
                table: "CashFlows",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "dc6761fb-fbce-4b79-a4ae-ce11e6870f06", new DateTime(2024, 9, 3, 12, 50, 47, 716, DateTimeKind.Local).AddTicks(7301), "AQAAAAIAAYagAAAAEOPAiMZywNTv1cseb+gvGipVeAGNFxtnbparlTXEom5iKH6mfZWWaP2HVuNyHdDRUA==", "b1f4c013-6da6-4ab0-a5e6-8ad4aff1ab4c" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountMasterId",
                table: "CashFlows");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b9cb179a-9f2e-4ba2-8215-0797eead9449", new DateTime(2024, 8, 29, 13, 24, 22, 36, DateTimeKind.Local).AddTicks(9313), "AQAAAAIAAYagAAAAEKFomkbq7/pETb7+HdZHpujxE0utMQUO81bs8wxrDs3+CnlFDOZ1wDMoWiOnHafBpQ==", "c7d16ffa-2373-4d31-9c59-4122eea51e63" });
        }
    }
}
