using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.DAL.EF.Migrations
{
    /// <inheritdoc />
    public partial class RecipesNutritionCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Ingredients_CompanyId",
                table: "Ingredients");

            migrationBuilder.AddColumn<string>(
                name: "ExclusionKey",
                table: "Ingredients",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAllergen",
                table: "Ingredients",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsExclusionTag",
                table: "Ingredients",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                table: "Ingredients",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_CompanyId_ExclusionKey",
                table: "Ingredients",
                columns: new[] { "CompanyId", "ExclusionKey" });

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_CompanyId_Name",
                table: "Ingredients",
                columns: new[] { "CompanyId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Ingredients_CompanyId_ExclusionKey",
                table: "Ingredients");

            migrationBuilder.DropIndex(
                name: "IX_Ingredients_CompanyId_Name",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "ExclusionKey",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "IsAllergen",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "IsExclusionTag",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "NormalizedName",
                table: "Ingredients");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_CompanyId",
                table: "Ingredients",
                column: "CompanyId");
        }
    }
}
