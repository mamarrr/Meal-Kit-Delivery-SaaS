using App.Contracts.BLL.Menu;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels.Menu;

public class RecipesNutritionIndexViewModel
{
    public Guid CompanyId { get; set; }
    public string CompanySlug { get; set; } = string.Empty;

    public string? Search { get; set; }
    public Guid? FilterDietaryCategoryId { get; set; }
    public Guid? FilterIngredientId { get; set; }
    public bool ActiveOnly { get; set; } = true;

    public Guid? SelectedRecipeId { get; set; }
    public Guid? EditingIngredientId { get; set; }
    public Guid? EditingDietaryCategoryId { get; set; }

    public List<RecipeListItemDto> Recipes { get; set; } = [];
    public RecipeEditorFormViewModel RecipeForm { get; set; } = new();
    public IngredientCatalogFormViewModel IngredientForm { get; set; } = new();
    public DietaryCategoryCatalogFormViewModel DietaryCategoryForm { get; set; } = new();

    public List<IngredientCatalogItemDto> IngredientCatalog { get; set; } = [];
    public List<DietaryCategoryCatalogItemDto> DietaryCategoryCatalog { get; set; } = [];
    public List<SelectListItem> IngredientOptions { get; set; } = [];
    public List<SelectListItem> DietaryCategoryOptions { get; set; } = [];
}

public class RecipeEditorFormViewModel
{
    public Guid? RecipeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DefaultServings { get; set; } = 2;
    public bool IsActive { get; set; } = true;

    public List<Guid> SelectedIngredientIds { get; set; } = [];
    public List<Guid> SelectedDietaryCategoryIds { get; set; } = [];

    public decimal CaloriesKcal { get; set; }
    public decimal ProteinG { get; set; }
    public decimal CarbsG { get; set; }
    public decimal FatG { get; set; }
    public decimal FiberG { get; set; }
    public decimal SodiumMg { get; set; }
}

public class IngredientCatalogFormViewModel
{
    public Guid? IngredientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsAllergen { get; set; }
}

public class DietaryCategoryCatalogFormViewModel
{
    public Guid? DietaryCategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
