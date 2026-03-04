using App.Domain.Menu;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels.CustomerPreferences;

public class CustomerPreferenceEditViewModel
{
    public CustomerPreference CustomerPreference { get; set; } = default!;

    public IReadOnlyList<SelectListItem> CustomerOptions { get; set; } = [];

    public IReadOnlyList<SelectListItem> DietaryCategoryOptions { get; set; } = [];
}

