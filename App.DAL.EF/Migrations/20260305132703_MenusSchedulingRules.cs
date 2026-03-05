using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.DAL.EF.Migrations
{
    /// <inheritdoc />
    public partial class MenusSchedulingRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DietaryCategoryId",
                table: "WeeklyMenuRecipes",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WeeklyMenuRuleConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RecipesPerCategory = table.Column<int>(type: "integer", nullable: false),
                    NoRepeatWeeks = table.Column<int>(type: "integer", nullable: false),
                    SelectionDeadlineDaysBeforeWeekStart = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyMenuRuleConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeeklyMenuRuleConfigs_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyMenuRecipes_DietaryCategoryId",
                table: "WeeklyMenuRecipes",
                column: "DietaryCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyMenuRuleConfigs_CompanyId",
                table: "WeeklyMenuRuleConfigs",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_WeeklyMenuRecipes_DietaryCategories_DietaryCategoryId",
                table: "WeeklyMenuRecipes",
                column: "DietaryCategoryId",
                principalTable: "DietaryCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WeeklyMenuRecipes_DietaryCategories_DietaryCategoryId",
                table: "WeeklyMenuRecipes");

            migrationBuilder.DropTable(
                name: "WeeklyMenuRuleConfigs");

            migrationBuilder.DropIndex(
                name: "IX_WeeklyMenuRecipes_DietaryCategoryId",
                table: "WeeklyMenuRecipes");

            migrationBuilder.DropColumn(
                name: "DietaryCategoryId",
                table: "WeeklyMenuRecipes");
        }
    }
}
