using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "AccountMaster",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "00e652f6-7b7f-43ab-8419-586649a25291", new DateTime(2024, 5, 23, 15, 38, 18, 50, DateTimeKind.Local).AddTicks(2285), "AQAAAAIAAYagAAAAELHAHFHAt9He0UOcMJqqzBqIqtt1J/OZoNVYfz/K+hROCAVnAppNOp+u2YypAnL0ig==", "7b51f58b-7e44-40a2-b144-516898c96185" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "AccountMaster");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c4222077-5b3a-4cfe-866c-efd45dc6ac03", new DateTime(2024, 5, 23, 14, 4, 25, 235, DateTimeKind.Local).AddTicks(5688), "AQAAAAIAAYagAAAAEPo0LJ7Qoku/jNkuxrXHnvAEE8MWenVGiuOOrYH3wYsx6WWOzmEtTwLP1ABEIZPNjw==", "8a6920f1-d956-416f-b89b-d8e5276b1629" });
        }
    }
}
