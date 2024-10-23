using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirmName",
                table: "expenses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ed1d8ae9-e873-47f3-974a-9b86c3cc2683", new DateTime(2024, 5, 31, 11, 4, 9, 999, DateTimeKind.Local).AddTicks(9475), "AQAAAAIAAYagAAAAEMnPoi7weX/TtBvG+NhkX90ZLPA3zlVT/u+BwdY/bK+Rrr10iC886mCtBfvhWD333w==", "4e3ec3b7-60b6-4bc3-8abf-0fadee3dceaf" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirmName",
                table: "expenses");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                columns: new[] { "ConcurrencyStamp", "CreateDateTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ce5b4619-1b7b-4261-bdfa-05e59a5a383b", new DateTime(2024, 5, 28, 11, 0, 53, 234, DateTimeKind.Local).AddTicks(6678), "AQAAAAIAAYagAAAAEKqLNjFTjk09OzsavWgr5JlU3bYB/ev3fWIuyUM1tl5UFXiv+9OQ3rczNAr3XVXekg==", "dd6e7a51-5915-4ced-995c-a6b102f335b2" });
        }
    }
}
