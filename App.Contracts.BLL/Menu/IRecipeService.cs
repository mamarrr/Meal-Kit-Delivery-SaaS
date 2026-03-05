using App.Domain.Menu;

namespace App.Contracts.BLL.Menu;

public interface IRecipeService : ITenantEntityService<Recipe>
{
    /// <summary>
    /// Counts active (non-deleted) recipes for a company.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <returns>Number of active recipes.</returns>
    Task<int> CountActiveByCompanyIdAsync(Guid companyId);

    /// <summary>
    /// Creates or updates nutritional information for a recipe.
    /// </summary>
    /// <param name="recipeId">The recipe ID.</param>
    /// <param name="companyId">The company ID for tenancy validation.</param>
    /// <param name="caloriesKcal">Calories in kcal.</param>
    /// <param name="proteinG">Protein grams.</param>
    /// <param name="carbsG">Carbs grams.</param>
    /// <param name="fatG">Fat grams.</param>
    /// <param name="fiberG">Fiber grams.</param>
    /// <param name="sodiumMg">Sodium milligrams.</param>
    /// <param name="sugarG">Sugar grams.</param>
    /// <param name="saturatedFatG">Saturated fat grams.</param>
    Task UpsertNutritionalInfoAsync(
        Guid recipeId,
        Guid companyId,
        decimal caloriesKcal,
        decimal proteinG,
        decimal carbsG,
        decimal fatG,
        decimal fiberG,
        decimal sodiumMg,
        decimal sugarG,
        decimal saturatedFatG);

    Task<ICollection<RecipeListItemDto>> GetRecipeListAsync(Guid companyId, RecipeListFilterDto? filter = null);
    Task<RecipeEditorDto?> GetRecipeEditorAsync(Guid companyId, Guid recipeId);
    Task<RecipeEditorDto> UpsertRecipeEditorAsync(Guid companyId, Guid actorId, RecipeEditorUpsertDto dto);
}

