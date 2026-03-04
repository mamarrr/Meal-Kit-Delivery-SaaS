using App.Domain.Menu;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels.MealSelections;

public class MealSelectionEditViewModel
{
    public MealSelection MealSelection { get; set; } = default!;

    public IReadOnlyList<SelectListItem> MealSubscriptionOptions { get; set; } = [];

    public IReadOnlyList<SelectListItem> WeeklyMenuOptions { get; set; } = [];

    public IReadOnlyList<SelectListItem> RecipeOptions { get; set; } = [];
}

