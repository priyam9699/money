using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceManagement.Migrations
{
    /// <inheritdoc />
    public partial class OtherCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "AccountMaster",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.CreateTable(
                name: "Others",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CashFlowId = table.Column<int>(type: "int", nullable: true),
                    FirmName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountMasterId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Others", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Others_AccountMaster_AccountMasterId",
                        column: x => x.AccountMasterId,
                        principalTable: "AccountMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Others_CashFlows_CashFlowId",
                        column: x => x.CashFlowId,
                        principalTable: "CashFlows",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b9cb179a-9f2e-4ba2-8215-0797eead9449", new DateTime(2024, 8, 29, 13, 24, 22, 36, DateTimeKind.Local).AddTicks(9313), "AQAAAAIAAYagAAAAEKFomkbq7/pETb7+HdZHpujxE0utMQUO81bs8wxrDs3+CnlFDOZ1wDMoWiOnHafBpQ==", "c7d16ffa-2373-4d31-9c59-4122eea51e63" });

            migrationBuilder.CreateIndex(
                name: "IX_Others_AccountMasterId",
                table: "Others",
                column: "AccountMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_Others_CashFlowId",
                table: "Others",
                column: "CashFlowId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Others");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "AccountMaster",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e49e13e8-aa13-4e6a-a622-8f2a957aa489", new DateTime(2024, 8, 6, 11, 10, 59, 234, DateTimeKind.Local).AddTicks(4657), "AQAAAAIAAYagAAAAEMim+mSlpiF26UveaelCEHs3IV/yJMeKIYYjnCQ5//z8jwo6Y0Hx5ZEQDmqqgwFlqw==", "52fbe7d0-7f1b-44f9-a75c-d3027fc38b36" });
        }
    }
}
