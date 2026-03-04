using App.Domain.Menu;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels.CustomerExclusions;

public class CustomerExclusionEditViewModel
{
    public CustomerExclusion CustomerExclusion { get; set; } = default!;

    public IReadOnlyList<SelectListItem> CustomerOptions { get; set; } = [];

    public IReadOnlyList<SelectListItem> IngredientOptions { get; set; } = [];
}

