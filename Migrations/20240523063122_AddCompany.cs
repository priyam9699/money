using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirmName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Owner = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "358252a3-464b-40c6-b7ee-f8982b2f2366", new DateTime(2024, 5, 23, 12, 1, 21, 803, DateTimeKind.Local).AddTicks(8523), "AQAAAAIAAYagAAAAEKdk0LVpeVtU5HqgPIeXIlId3f0/SXQlOL09ICeK8oE0nhtl/echdsgV/kqN/8NBNw==", "eaded322-aa7c-4bd9-9011-8abcbc7d988f" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "52f05c08-f917-4765-b26c-7e3620d92a2c", new DateTime(2024, 5, 14, 12, 27, 8, 767, DateTimeKind.Local).AddTicks(67), "AQAAAAIAAYagAAAAEOKRFQe5Ka+wX30CsTTNlUYFQODLgxhyqFW8T28oquZiuHkIcUZX5QIUE9MiMy+fpQ==", "15f1e334-4297-46c2-8f20-1fafed439695" });
        }
    }
}
