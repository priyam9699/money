using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SKU = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    DamageQuantity = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "95576250-d8cb-4631-8747-21322983b022", new DateTime(2024, 10, 5, 10, 56, 36, 177, DateTimeKind.Local).AddTicks(7894), "AQAAAAIAAYagAAAAEEHUBLOAKIRJXSEiLIqzkLS3EFTpkkDlJAEHio6MnnhFAywLy5mye6IlnV5IZ8xcVQ==", "4f68cc4c-6984-4b75-b912-dba40759c2b5" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9b2ae22b-13d0-419a-be3e-e2cbca63b0cd", new DateTime(2024, 9, 6, 11, 36, 27, 510, DateTimeKind.Local).AddTicks(259), "AQAAAAIAAYagAAAAEEwGAuxqvedhEOnt1Bb1pzM6j/H92qvE4svwunKVoroT2JtmDrX3KzJhtQhnSVZZbA==", "230fc1aa-1860-4f29-89de-35bbd6eafbdf" });
        }
    }
}
