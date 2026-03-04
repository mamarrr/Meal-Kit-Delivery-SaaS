using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels.MealSelections;

public class MealSelectionRecommendationViewModel
{
    [Required]
    public Guid MealSubscriptionId { get; set; }

    [Required]
    public Guid WeeklyMenuId { get; set; }

    [Display(Name = "Lock selection at")]
    public DateTime? LockedAt { get; set; }

    [Display(Name = "Max calories (kcal)")]
    public decimal? MaxCaloriesKcal { get; set; }

    [Display(Name = "Min protein (g)")]
    public decimal? MinProteinG { get; set; }

    public Guid? RecommendedRecipeId { get; set; }

    public string? RecommendationSummary { get; set; }

    public IReadOnlyList<string> DecisionNotes { get; set; } = [];

    public IReadOnlyList<SelectListItem> MealSubscriptionOptions { get; set; } = [];

    public IReadOnlyList<SelectListItem> WeeklyMenuOptions { get; set; } = [];

    public IReadOnlyList<SelectListItem> RecommendedRecipeOptions { get; set; } = [];
}
