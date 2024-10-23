using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddDailyBelt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dailyBeltUpdates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProductSKU = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dailyBeltUpdates", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1ca30008-745c-4b5b-9db2-686636d9463c", new DateTime(2024, 10, 5, 11, 32, 35, 93, DateTimeKind.Local).AddTicks(7538), "AQAAAAIAAYagAAAAEIDbJf22WKn0OVmZ95a9A8eizOx4mWf+9hCnEDUwa/IpmlsfO0f6cCtqIVDh4ONIdA==", "669e005d-45e6-4a37-879a-354ff68acc33" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dailyBeltUpdates");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "95576250-d8cb-4631-8747-21322983b022", new DateTime(2024, 10, 5, 10, 56, 36, 177, DateTimeKind.Local).AddTicks(7894), "AQAAAAIAAYagAAAAEEHUBLOAKIRJXSEiLIqzkLS3EFTpkkDlJAEHio6MnnhFAywLy5mye6IlnV5IZ8xcVQ==", "4f68cc4c-6984-4b75-b912-dba40759c2b5" });
        }
    }
}
