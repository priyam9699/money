using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldToAccMaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PaymentCategory",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "PaymentCategory",
                table: "AccountMaster",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpadCategory",
                table: "AccountMaster",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5aa6a755-3a29-4962-917c-79863c00edc9", new DateTime(2024, 6, 6, 16, 59, 27, 27, DateTimeKind.Local).AddTicks(4271), "AQAAAAIAAYagAAAAEGQh9+P1If8RyQrpAPda+aqZlVsbAn9LbWwJ7lbN477YtKI6pg9bBOa0h3Wkb4ea/g==", "9e6a8eb1-0d70-4ba6-b357-d30cd22c3fe7" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentCategory",
                table: "AccountMaster");

            migrationBuilder.DropColumn(
                name: "UpadCategory",
                table: "AccountMaster");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentCategory",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ed1d8ae9-e873-47f3-974a-9b86c3cc2683", new DateTime(2024, 5, 31, 11, 4, 9, 999, DateTimeKind.Local).AddTicks(9475), "AQAAAAIAAYagAAAAEMnPoi7weX/TtBvG+NhkX90ZLPA3zlVT/u+BwdY/bK+Rrr10iC886mCtBfvhWD333w==", "4e3ec3b7-60b6-4bc3-8abf-0fadee3dceaf" });
        }
    }
}
