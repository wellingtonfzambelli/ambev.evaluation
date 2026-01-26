using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ambev.DeveloperEvaluation.ORM.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerIdToSales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Users_UserId",
                table: "Sales");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Sales",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Sales_UserId",
                table: "Sales",
                newName: "IX_Sales_CustomerId");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("9f3c6ce6-4a4a-4f2e-86a6-2c92f39e5e58"),
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2026, 1, 23, 2, 15, 31, 27, DateTimeKind.Utc).AddTicks(7065), "$2a$11$2L5/nxUCsZ9pE0qwCnp8V.NlRoXlhYhF4U6NCOD0sF1fjd2mV2O.a" });

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Users_CustomerId",
                table: "Sales",
                column: "CustomerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Users_CustomerId",
                table: "Sales");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "Sales",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Sales_CustomerId",
                table: "Sales",
                newName: "IX_Sales_UserId");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("9f3c6ce6-4a4a-4f2e-86a6-2c92f39e5e58"),
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2026, 1, 22, 0, 0, 0, 0, DateTimeKind.Utc), "123" });

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Users_UserId",
                table: "Sales",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
