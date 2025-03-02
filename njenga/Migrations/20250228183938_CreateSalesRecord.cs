using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace njenga.Migrations
{
    /// <inheritdoc />
    public partial class CreateSalesRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Institutions",
                table: "Institutions");

            migrationBuilder.DropColumn(
                name: "Products",
                table: "Institutions");

            migrationBuilder.RenameTable(
                name: "Institutions",
                newName: "institutions");

            migrationBuilder.RenameColumn(
                name: "Institution_id",
                table: "products",
                newName: "InstitutionId");

            migrationBuilder.AddColumn<string>(
                name: "About",
                table: "institutions",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_institutions",
                table: "institutions",
                column: "InstitutionId");

            migrationBuilder.CreateTable(
                name: "account",
                columns: table => new
                {
                    User_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InstitutionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account", x => x.User_id);
                    table.ForeignKey(
                        name: "FK_account_institutions_InstitutionId",
                        column: x => x.InstitutionId,
                        principalTable: "institutions",
                        principalColumn: "InstitutionId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PurchaseRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Amount = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    InstitutionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseRecords_institutions_InstitutionId",
                        column: x => x.InstitutionId,
                        principalTable: "institutions",
                        principalColumn: "InstitutionId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SalesRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    InstitutionId = table.Column<int>(type: "int", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    User = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Count = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesRecords_institutions_InstitutionId",
                        column: x => x.InstitutionId,
                        principalTable: "institutions",
                        principalColumn: "InstitutionId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "stocktaking",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    InstitutionId = table.Column<int>(type: "int", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    User = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stocktaking", x => x.Id);
                    table.ForeignKey(
                        name: "FK_stocktaking_institutions_InstitutionId",
                        column: x => x.InstitutionId,
                        principalTable: "institutions",
                        principalColumn: "InstitutionId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_products_InstitutionId",
                table: "products",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_account_InstitutionId",
                table: "account",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRecords_InstitutionId",
                table: "PurchaseRecords",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesRecords_InstitutionId",
                table: "SalesRecords",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_stocktaking_InstitutionId",
                table: "stocktaking",
                column: "InstitutionId");

            migrationBuilder.AddForeignKey(
                name: "FK_products_institutions_InstitutionId",
                table: "products",
                column: "InstitutionId",
                principalTable: "institutions",
                principalColumn: "InstitutionId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_products_institutions_InstitutionId",
                table: "products");

            migrationBuilder.DropTable(
                name: "account");

            migrationBuilder.DropTable(
                name: "PurchaseRecords");

            migrationBuilder.DropTable(
                name: "SalesRecords");

            migrationBuilder.DropTable(
                name: "stocktaking");

            migrationBuilder.DropIndex(
                name: "IX_products_InstitutionId",
                table: "products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_institutions",
                table: "institutions");

            migrationBuilder.DropColumn(
                name: "About",
                table: "institutions");

            migrationBuilder.RenameTable(
                name: "institutions",
                newName: "Institutions");

            migrationBuilder.RenameColumn(
                name: "InstitutionId",
                table: "products",
                newName: "Institution_id");

            migrationBuilder.AddColumn<string>(
                name: "Products",
                table: "Institutions",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Institutions",
                table: "Institutions",
                column: "InstitutionId");
        }
    }
}
