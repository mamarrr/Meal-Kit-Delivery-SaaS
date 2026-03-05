using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.DAL.EF.Migrations
{
    /// <inheritdoc />
    public partial class PricingProductsCorrections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoxPrices_PlatformSubscriptionTiers_PlatformSubscriptionTie~",
                table: "BoxPrices");

            migrationBuilder.DropIndex(
                name: "IX_BoxPrices_CompanyId_BoxId_PlatformSubscriptionTierId",
                table: "BoxPrices");

            migrationBuilder.DropIndex(
                name: "IX_BoxPrices_PlatformSubscriptionTierId",
                table: "BoxPrices");

            migrationBuilder.DropColumn(
                name: "PlatformSubscriptionTierId",
                table: "BoxPrices");

            migrationBuilder.AddColumn<string>(
                name: "PricingName",
                table: "BoxPrices",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "BoxDietaryCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BoxId = table.Column<Guid>(type: "uuid", nullable: false),
                    DietaryCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByAppUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoxDietaryCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BoxDietaryCategories_AspNetUsers_CreatedByAppUserId",
                        column: x => x.CreatedByAppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BoxDietaryCategories_Boxes_BoxId",
                        column: x => x.BoxId,
                        principalTable: "Boxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BoxDietaryCategories_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BoxDietaryCategories_DietaryCategories_DietaryCategoryId",
                        column: x => x.DietaryCategoryId,
                        principalTable: "DietaryCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BoxPrices_CompanyId_BoxId_PricingName",
                table: "BoxPrices",
                columns: new[] { "CompanyId", "BoxId", "PricingName" });

            migrationBuilder.CreateIndex(
                name: "IX_BoxDietaryCategories_BoxId",
                table: "BoxDietaryCategories",
                column: "BoxId");

            migrationBuilder.CreateIndex(
                name: "IX_BoxDietaryCategories_CompanyId_BoxId_DietaryCategoryId_Dele~",
                table: "BoxDietaryCategories",
                columns: new[] { "CompanyId", "BoxId", "DietaryCategoryId", "DeletedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_BoxDietaryCategories_CreatedByAppUserId",
                table: "BoxDietaryCategories",
                column: "CreatedByAppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BoxDietaryCategories_DietaryCategoryId",
                table: "BoxDietaryCategories",
                column: "DietaryCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BoxDietaryCategories");

            migrationBuilder.DropIndex(
                name: "IX_BoxPrices_CompanyId_BoxId_PricingName",
                table: "BoxPrices");

            migrationBuilder.DropColumn(
                name: "PricingName",
                table: "BoxPrices");

            migrationBuilder.AddColumn<Guid>(
                name: "PlatformSubscriptionTierId",
                table: "BoxPrices",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_BoxPrices_CompanyId_BoxId_PlatformSubscriptionTierId",
                table: "BoxPrices",
                columns: new[] { "CompanyId", "BoxId", "PlatformSubscriptionTierId" });

            migrationBuilder.CreateIndex(
                name: "IX_BoxPrices_PlatformSubscriptionTierId",
                table: "BoxPrices",
                column: "PlatformSubscriptionTierId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoxPrices_PlatformSubscriptionTiers_PlatformSubscriptionTie~",
                table: "BoxPrices",
                column: "PlatformSubscriptionTierId",
                principalTable: "PlatformSubscriptionTiers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
