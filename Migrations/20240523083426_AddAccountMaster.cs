using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountMaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountMaster",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountMaster", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c4222077-5b3a-4cfe-866c-efd45dc6ac03", new DateTime(2024, 5, 23, 14, 4, 25, 235, DateTimeKind.Local).AddTicks(5688), "AQAAAAIAAYagAAAAEPo0LJ7Qoku/jNkuxrXHnvAEE8MWenVGiuOOrYH3wYsx6WWOzmEtTwLP1ABEIZPNjw==", "8a6920f1-d956-416f-b89b-d8e5276b1629" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountMaster");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "358252a3-464b-40c6-b7ee-f8982b2f2366", new DateTime(2024, 5, 23, 12, 1, 21, 803, DateTimeKind.Local).AddTicks(8523), "AQAAAAIAAYagAAAAEKdk0LVpeVtU5HqgPIeXIlId3f0/SXQlOL09ICeK8oE0nhtl/echdsgV/kqN/8NBNw==", "eaded322-aa7c-4bd9-9011-8abcbc7d988f" });
        }
    }
}
