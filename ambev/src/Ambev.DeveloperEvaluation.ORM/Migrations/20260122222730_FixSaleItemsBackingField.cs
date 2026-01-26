using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ambev.DeveloperEvaluation.ORM.Migrations
{
    /// <inheritdoc />
    public partial class FixSaleItemsBackingField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    SaleNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    SaleDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SaleStatus = table.Column<string>(type: "varchar(20)", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sales_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sales_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SaleItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PercentageDiscount = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    SaleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleItems_Sales_SaleId",
                        column: x => x.SaleId,
                        principalTable: "Sales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Branches",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("1f2e3d4c-5b6a-7c8d-9e0f-1a2b3c4d5e6f"), "South Side" },
                    { new Guid("2a3b4c5d-6e7f-8a9b-0c1d-2e3f4a5b6c7d"), "East End" },
                    { new Guid("3b4c5d6e-7f8a-9b0c-1d2e-3f4a5b6c7d8e"), "West End" },
                    { new Guid("4c5d6e7f-8a9b-0c1d-2e3f-4a5b6c7d8e9f"), "Central Plaza" },
                    { new Guid("5d6e7f8a-9b0c-1d2e-3f4a-5b6c7d8e9f0a"), "Harbor Point" },
                    { new Guid("6d7f5d7a-44d5-4c3f-8b4f-3a7b2a9f6f11"), "Downtown" },
                    { new Guid("6e7f8a9b-0c1d-2e3f-4a5b-6c7d8e9f0a1b"), "Riverside" },
                    { new Guid("7f8a9b0c-1d2e-3f4a-5b6c-7d8e9f0a1b2c"), "Airport Hub" },
                    { new Guid("9b8f1a2c-3d4e-4f5a-8b7c-6d5e4f3a2b10"), "North Side" },
                    { new Guid("b2c6a1e3-4f7d-4d8a-9a2f-3f1f7b6c5d21"), "Uptown" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Name", "Price" },
                values: new object[,]
                {
                    { new Guid("0f1b00d2-0ff3-4b2b-8b8c-1c6f1e9a1b16"), "Digital Piano", 2499.99m },
                    { new Guid("3c6e8c9b-7b61-4d6b-95f0-2c1f4dbbe6d2"), "Drum Kit", 2199.00m },
                    { new Guid("4b2b2f87-1e3e-4e1c-9fd3-6e9a7d6f1f5b"), "MIDI Controller", 499.90m },
                    { new Guid("5a10a8a4-42ef-4e9c-8e0e-2d6f0c0a2d9b"), "Electric Guitar", 1299.00m },
                    { new Guid("7f8d4b9b-1a6d-45a3-8e9c-8f0c4ddc0d1b"), "Studio Microphone", 699.00m },
                    { new Guid("9c0b75c6-9c3b-4d25-8a3a-2a91f0ec5c7a"), "Acoustic Guitar", 899.90m },
                    { new Guid("a3e7b0c7-8d2a-4e3a-b0df-9d3d5fd8d2dd"), "Bass Guitar", 1099.50m },
                    { new Guid("c9d2c7d4-1f53-4ef2-9b5a-0d9a7c4d2b29"), "Audio Interface", 899.00m },
                    { new Guid("d1a6b2f9-8e4b-4b74-9b8a-7c0d5f1e3a6c"), "Stage Monitor", 1399.00m },
                    { new Guid("f0b9d7a2-6b7c-4f9e-9b3c-5e1d2f7a9c10"), "Guitar Strings Set", 49.90m }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "Password", "Phone", "Role", "Status", "UpdatedAt", "Username" },
                values: new object[] { new Guid("9f3c6ce6-4a4a-4f2e-86a6-2c92f39e5e58"), new DateTime(2026, 1, 22, 0, 0, 0, 0, DateTimeKind.Utc), "wellington@test.com", "123", "(11) 99999-9999", "Admin", "Active", null, "Wellington" });

            migrationBuilder.CreateIndex(
                name: "IX_SaleItems_ProductId",
                table: "SaleItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleItems_SaleId",
                table: "SaleItems",
                column: "SaleId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_BranchId",
                table: "Sales",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_UserId",
                table: "Sales",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaleItems");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Sales");

            migrationBuilder.DropTable(
                name: "Branches");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("9f3c6ce6-4a4a-4f2e-86a6-2c92f39e5e58"));

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Users");
        }
    }
}
