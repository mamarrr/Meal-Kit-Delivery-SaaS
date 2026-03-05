using App.Domain.Menu;
using Contracts.DAL;

namespace App.Contracts.DAL.Menu;

/// <summary>
/// Repository interface for Recipe aggregate.
/// </summary>
public interface IRecipeRepository : IRepository<Recipe>
{
    /// <summary>
    /// Gets all recipes for a specific company.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <returns>A collection of recipes belonging to the company.</returns>
    Task<ICollection<Recipe>> GetAllByCompanyIdAsync(Guid companyId);
    Task<Recipe?> GetByIdWithDetailsAsync(Guid recipeId, Guid companyId);
    Task<ICollection<Recipe>> GetAllByCompanyIdWithDetailsAsync(Guid companyId);
    Task<ICollection<RecipeIngredient>> GetRecipeIngredientsAsync(Guid recipeId);
    Task<ICollection<RecipeDietaryCategory>> GetRecipeDietaryCategoriesAsync(Guid recipeId);
    Task AddRecipeIngredientsAsync(ICollection<RecipeIngredient> entities);
    Task AddRecipeDietaryCategoriesAsync(ICollection<RecipeDietaryCategory> entities);
    Task RemoveRecipeIngredientsAsync(ICollection<RecipeIngredient> entities);
    Task RemoveRecipeDietaryCategoriesAsync(ICollection<RecipeDietaryCategory> entities);
    Task<int> CountExistingIngredientIdsAsync(Guid companyId, ICollection<Guid> ingredientIds);
    Task<int> CountExistingDietaryCategoryIdsAsync(Guid companyId, ICollection<Guid> dietaryCategoryIds);

    /// <summary>
    /// Counts active (non-deleted) recipes for a company.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <returns>Number of active recipes in the company scope.</returns>
    Task<int> CountActiveByCompanyIdAsync(Guid companyId);

    /// <summary>
    /// Gets nutritional info by recipe id.
    /// </summary>
    /// <param name="recipeId">The recipe ID.</param>
    /// <returns>Nutritional info if found; otherwise null.</returns>
    Task<NutritionalInfo?> GetNutritionalInfoByRecipeIdAsync(Guid recipeId);

    /// <summary>
    /// Adds a nutritional info row.
    /// </summary>
    /// <param name="nutritionalInfo">Entity to add.</param>
    /// <returns>Added nutritional info entity.</returns>
    NutritionalInfo AddNutritionalInfo(NutritionalInfo nutritionalInfo);

    /// <summary>
    /// Updates a nutritional info row.
    /// </summary>
    /// <param name="nutritionalInfo">Entity to update.</param>
    /// <returns>Updated nutritional info entity.</returns>
    NutritionalInfo UpdateNutritionalInfo(NutritionalInfo nutritionalInfo);
}
