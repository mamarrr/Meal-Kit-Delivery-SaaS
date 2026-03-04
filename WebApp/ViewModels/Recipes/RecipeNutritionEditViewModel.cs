using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels.Recipes;

public class RecipeNutritionEditViewModel
{
    public Guid RecipeId { get; set; }

    public string RecipeName { get; set; } = string.Empty;

    [Range(0, 5000)]
    public decimal CaloriesKcal { get; set; }

    [Range(0, 500)]
    public decimal ProteinG { get; set; }

    [Range(0, 500)]
    public decimal CarbsG { get; set; }

    [Range(0, 300)]
    public decimal FatG { get; set; }

    [Range(0, 200)]
    public decimal FiberG { get; set; }

    [Range(0, 10000)]
    public decimal SodiumMg { get; set; }

    [Range(0, 500)]
    public decimal SugarG { get; set; }

    [Range(0, 200)]
    public decimal SaturatedFatG { get; set; }
}
