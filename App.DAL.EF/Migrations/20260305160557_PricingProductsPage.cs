using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.DAL.EF.Migrations
{
    /// <inheritdoc />
    public partial class PricingProductsPage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BoxPrices_CompanyId",
                table: "BoxPrices");

            migrationBuilder.DropIndex(
                name: "IX_Boxes_CompanyId",
                table: "Boxes");

            migrationBuilder.AddColumn<Guid>(
                name: "PlatformSubscriptionTierId",
                table: "BoxPrices",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "PricingAdjustments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AdjustmentType = table.Column<string>(type: "text", nullable: false),
                    Label = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    IsPercentage = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByAppUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricingAdjustments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PricingAdjustments_AspNetUsers_CreatedByAppUserId",
                        column: x => x.CreatedByAppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PricingAdjustments_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BoxPrices_CompanyId_BoxId_PlatformSubscriptionTierId",
                table: "BoxPrices",
                columns: new[] { "CompanyId", "BoxId", "PlatformSubscriptionTierId" });

            migrationBuilder.CreateIndex(
                name: "IX_BoxPrices_PlatformSubscriptionTierId",
                table: "BoxPrices",
                column: "PlatformSubscriptionTierId");

            migrationBuilder.CreateIndex(
                name: "IX_Boxes_CompanyId_MealsCount_PeopleCount",
                table: "Boxes",
                columns: new[] { "CompanyId", "MealsCount", "PeopleCount" });

            migrationBuilder.CreateIndex(
                name: "IX_PricingAdjustments_CompanyId_AdjustmentType_IsActive",
                table: "PricingAdjustments",
                columns: new[] { "CompanyId", "AdjustmentType", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_PricingAdjustments_CreatedByAppUserId",
                table: "PricingAdjustments",
                column: "CreatedByAppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoxPrices_PlatformSubscriptionTiers_PlatformSubscriptionTie~",
                table: "BoxPrices",
                column: "PlatformSubscriptionTierId",
                principalTable: "PlatformSubscriptionTiers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoxPrices_PlatformSubscriptionTiers_PlatformSubscriptionTie~",
                table: "BoxPrices");

            migrationBuilder.DropTable(
                name: "PricingAdjustments");

            migrationBuilder.DropIndex(
                name: "IX_BoxPrices_CompanyId_BoxId_PlatformSubscriptionTierId",
                table: "BoxPrices");

            migrationBuilder.DropIndex(
                name: "IX_BoxPrices_PlatformSubscriptionTierId",
                table: "BoxPrices");

            migrationBuilder.DropIndex(
                name: "IX_Boxes_CompanyId_MealsCount_PeopleCount",
                table: "Boxes");

            migrationBuilder.DropColumn(
                name: "PlatformSubscriptionTierId",
                table: "BoxPrices");

            migrationBuilder.CreateIndex(
                name: "IX_BoxPrices_CompanyId",
                table: "BoxPrices",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Boxes_CompanyId",
                table: "Boxes",
                column: "CompanyId");
        }
    }
}
