using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PONumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    PODate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    POCompletionDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CustomerName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    AssignedToUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    AssignedToName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ProductName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    UploadedImagesJson = table.Column<string>(type: "TEXT", nullable: false, defaultValue: "[]"),
                    OrderQuantityMeters = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    CostPerMeterPO = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    CostPerMeterProduction = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    TotalPOValue = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    TotalProductionValue = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    CurrentStatus = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    MetersDelivered = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    MetersToBeDelivered = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    AmountReceived = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    AmountToBeReceived = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    PendingPayments = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Comments = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductOrders_Users_AssignedToUserId",
                        column: x => x.AssignedToUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductOrders_AssignedToUserId",
                table: "ProductOrders",
                column: "AssignedToUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductOrders");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
