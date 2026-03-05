namespace App.Contracts.BLL.Menu;

public sealed class RecipeNutritionDto
{
    public decimal CaloriesKcal { get; init; }
    public decimal ProteinG { get; init; }
    public decimal CarbsG { get; init; }
    public decimal FatG { get; init; }
    public decimal FiberG { get; init; }
    public decimal SodiumMg { get; init; }
}

/// <summary>
/// DTO for nutrition-based filtering with min/max range constraints.
/// </summary>
public sealed class NutritionFilterDto
{
    public decimal? MinCaloriesKcal { get; init; }
    public decimal? MaxCaloriesKcal { get; init; }
    public decimal? MinProteinG { get; init; }
    public decimal? MaxProteinG { get; init; }
    public decimal? MinCarbsG { get; init; }
    public decimal? MaxCarbsG { get; init; }
    public decimal? MinFatG { get; init; }
    public decimal? MaxFatG { get; init; }
    public decimal? MinFiberG { get; init; }
    public decimal? MaxFiberG { get; init; }
    public decimal? MinSodiumMg { get; init; }
    public decimal? MaxSodiumMg { get; init; }
}

public sealed class RecipeListItemDto
{
    public Guid RecipeId { get; init; }
    public string Name { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public IReadOnlyCollection<string> DietaryCategories { get; init; } = [];
    public IReadOnlyCollection<string> Tags { get; init; } = [];
    public RecipeNutritionDto? Nutrition { get; init; }
}

public sealed class RecipeEditorDto
{
    public Guid RecipeId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int DefaultServings { get; init; }
    public bool IsActive { get; init; }
    public IReadOnlyCollection<Guid> IngredientIds { get; init; } = [];
    public IReadOnlyCollection<Guid> DietaryCategoryIds { get; init; } = [];
    public RecipeNutritionDto? Nutrition { get; init; }
}

public sealed class RecipeEditorUpsertDto
{
    public Guid? RecipeId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int DefaultServings { get; init; }
    public bool IsActive { get; init; }
    public IReadOnlyCollection<Guid> IngredientIds { get; init; } = [];
    public IReadOnlyCollection<Guid> DietaryCategoryIds { get; init; } = [];
    public RecipeNutritionDto Nutrition { get; init; } = new();
}

public sealed class RecipeListFilterDto
{
    public string? Search { get; init; }
    public Guid? DietaryCategoryId { get; init; }
    public Guid? IngredientId { get; init; }
    public bool? ActiveOnly { get; init; }
}

public sealed class IngredientCatalogItemDto
{
    public Guid IngredientId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string NormalizedName { get; init; } = string.Empty;
    public bool IsAllergen { get; init; }
    public bool IsExclusionTag { get; init; }
    public string? ExclusionKey { get; init; }
}

public sealed class IngredientCatalogUpsertDto
{
    public Guid? IngredientId { get; init; }
    public string Name { get; init; } = string.Empty;
    public bool IsAllergen { get; init; }
    public bool IsExclusionTag { get; init; }
    public string? ExclusionKey { get; init; }
}

public sealed class DietaryCategoryCatalogItemDto
{
    public Guid DietaryCategoryId { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public bool IsActive { get; init; }
}

public sealed class DietaryCategoryCatalogUpsertDto
{
    public Guid? DietaryCategoryId { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public bool IsActive { get; init; } = true;
}
