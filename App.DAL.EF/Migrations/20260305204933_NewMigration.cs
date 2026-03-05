using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.DAL.EF.Migrations
{
    /// <inheritdoc />
    public partial class NewMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AutoSelectionNotes",
                table: "MealSelections",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AutoSelectionReason",
                table: "MealSelections",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoSelectionNotes",
                table: "MealSelections");

            migrationBuilder.DropColumn(
                name: "AutoSelectionReason",
                table: "MealSelections");
        }
    }
}
