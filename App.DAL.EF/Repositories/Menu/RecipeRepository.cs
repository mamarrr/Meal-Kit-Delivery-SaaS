using App.Contracts.DAL.Menu;
using App.Domain.Menu;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories.Menu;

/// <summary>
/// EF Core repository implementation for Recipe aggregate.
/// </summary>
public class RecipeRepository : BaseRepository<Recipe, AppDbContext>, IRecipeRepository
{
    public RecipeRepository(AppDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<ICollection<Recipe>> GetAllByCompanyIdAsync(Guid companyId)
    {
        return await RepositoryDbSet
            .Include(r => r.NutritionalInfo)
            .Include(r => r.RecipeIngredients)
            .Include(r => r.RecipeDietaryCategories)
            .Where(r => r.CompanyId == companyId)
            .ToListAsync();
    }

    public async Task<Recipe?> GetByIdWithDetailsAsync(Guid recipeId, Guid companyId)
    {
        return await RepositoryDbSet
            .Include(r => r.NutritionalInfo)
            .Include(r => r.RecipeIngredients)
            .Include(r => r.RecipeDietaryCategories)
            .FirstOrDefaultAsync(r => r.Id == recipeId && r.CompanyId == companyId);
    }

    public async Task<ICollection<Recipe>> GetAllByCompanyIdWithDetailsAsync(Guid companyId)
    {
        return await RepositoryDbSet
            .Include(r => r.NutritionalInfo)
            .Include(r => r.RecipeIngredients!)
                .ThenInclude(ri => ri.Ingredient)
            .Include(r => r.RecipeDietaryCategories!)
                .ThenInclude(rd => rd.DietaryCategory)
            .Where(r => r.CompanyId == companyId && r.DeletedAt == null)
            .OrderBy(r => r.Name)
            .ToListAsync();
    }

    public async Task<ICollection<RecipeIngredient>> GetRecipeIngredientsAsync(Guid recipeId)
    {
        return await RepositoryDbContext.RecipeIngredients
            .Where(x => x.RecipeId == recipeId && x.DeletedAt == null)
            .ToListAsync();
    }

    public async Task<ICollection<RecipeDietaryCategory>> GetRecipeDietaryCategoriesAsync(Guid recipeId)
    {
        return await RepositoryDbContext.RecipeDietaryCategories
            .Where(x => x.RecipeId == recipeId && x.DeletedAt == null)
            .ToListAsync();
    }

    public async Task AddRecipeIngredientsAsync(ICollection<RecipeIngredient> entities)
    {
        await RepositoryDbContext.RecipeIngredients.AddRangeAsync(entities);
    }

    public async Task AddRecipeDietaryCategoriesAsync(ICollection<RecipeDietaryCategory> entities)
    {
        await RepositoryDbContext.RecipeDietaryCategories.AddRangeAsync(entities);
    }

    public async Task<int> CountExistingIngredientIdsAsync(Guid companyId, ICollection<Guid> ingredientIds)
    {
        if (ingredientIds.Count == 0)
        {
            return 0;
        }

        return await RepositoryDbContext.Ingredients
            .Where(x => x.CompanyId == companyId && x.DeletedAt == null && ingredientIds.Contains(x.Id))
            .CountAsync();
    }

    public async Task<int> CountExistingDietaryCategoryIdsAsync(Guid companyId, ICollection<Guid> dietaryCategoryIds)
    {
        if (dietaryCategoryIds.Count == 0)
        {
            return 0;
        }

        return await RepositoryDbContext.DietaryCategories
            .Where(x => x.CompanyId == companyId && x.DeletedAt == null && dietaryCategoryIds.Contains(x.Id))
            .CountAsync();
    }

    public Task RemoveRecipeIngredientsAsync(ICollection<RecipeIngredient> entities)
    {
        foreach (var entity in entities)
        {
            entity.DeletedAt = DateTime.UtcNow;
            RepositoryDbContext.RecipeIngredients.Update(entity);
        }

        return Task.CompletedTask;
    }

    public Task RemoveRecipeDietaryCategoriesAsync(ICollection<RecipeDietaryCategory> entities)
    {
        foreach (var entity in entities)
        {
            entity.DeletedAt = DateTime.UtcNow;
            RepositoryDbContext.RecipeDietaryCategories.Update(entity);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<int> CountActiveByCompanyIdAsync(Guid companyId)
    {
        return await RepositoryDbSet
            .Where(r => r.CompanyId == companyId && r.DeletedAt == null)
            .CountAsync();
    }

    /// <inheritdoc />
    public async Task<NutritionalInfo?> GetNutritionalInfoByRecipeIdAsync(Guid recipeId)
    {
        return await RepositoryDbContext.NutritionalInfos
            .FirstOrDefaultAsync(n => n.RecipeId == recipeId);
    }

    /// <inheritdoc />
    public NutritionalInfo AddNutritionalInfo(NutritionalInfo nutritionalInfo)
    {
        return RepositoryDbContext.NutritionalInfos.Add(nutritionalInfo).Entity;
    }

    /// <inheritdoc />
    public NutritionalInfo UpdateNutritionalInfo(NutritionalInfo nutritionalInfo)
    {
        return RepositoryDbContext.NutritionalInfos.Update(nutritionalInfo).Entity;
    }
}
