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
            .Include(r => r.RecipeIngredients!)
                .ThenInclude(ri => ri.Ingredient)
            .Include(r => r.RecipeDietaryCategories!)
                .ThenInclude(rd => rd.DietaryCategory)
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
            var tracked = RepositoryDbContext.RecipeIngredients.Local
                .FirstOrDefault(x => x.Id == entity.Id);

            if (tracked != null)
            {
                if (!ReferenceEquals(tracked, entity))
                {
                    RepositoryDbContext.Entry(tracked).CurrentValues.SetValues(entity);
                }
            }
            else
            {
                RepositoryDbContext.RecipeIngredients.Update(entity);
            }
        }

        return Task.CompletedTask;
    }

    public Task RemoveRecipeDietaryCategoriesAsync(ICollection<RecipeDietaryCategory> entities)
    {
        foreach (var entity in entities)
        {
            entity.DeletedAt = DateTime.UtcNow;
            var tracked = RepositoryDbContext.RecipeDietaryCategories.Local
                .FirstOrDefault(x => x.Id == entity.Id);

            if (tracked != null)
            {
                if (!ReferenceEquals(tracked, entity))
                {
                    RepositoryDbContext.Entry(tracked).CurrentValues.SetValues(entity);
                }
            }
            else
            {
                RepositoryDbContext.RecipeDietaryCategories.Update(entity);
            }
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
        var tracked = RepositoryDbContext.NutritionalInfos.Local
            .FirstOrDefault(x => x.Id == nutritionalInfo.Id);

        if (tracked != null)
        {
            if (!ReferenceEquals(tracked, nutritionalInfo))
            {
                RepositoryDbContext.Entry(tracked).CurrentValues.SetValues(nutritionalInfo);
            }

            return tracked;
        }

        return RepositoryDbContext.NutritionalInfos.Update(nutritionalInfo).Entity;
    }

    /// <inheritdoc />
    public async Task<ICollection<Recipe>> GetAllByCompanyIdWithNutritionFilterAsync(
        Guid companyId,
        decimal? minCaloriesKcal = null,
        decimal? maxCaloriesKcal = null,
        decimal? minProteinG = null,
        decimal? maxProteinG = null,
        decimal? minCarbsG = null,
        decimal? maxCarbsG = null,
        decimal? minFatG = null,
        decimal? maxFatG = null,
        decimal? minFiberG = null,
        decimal? maxFiberG = null,
        decimal? minSodiumMg = null,
        decimal? maxSodiumMg = null)
    {
        var query = RepositoryDbSet
            .Include(r => r.NutritionalInfo)
            .Include(r => r.RecipeIngredients!)
                .ThenInclude(ri => ri.Ingredient)
            .Include(r => r.RecipeDietaryCategories!)
                .ThenInclude(rd => rd.DietaryCategory)
            .Where(r => r.CompanyId == companyId && r.DeletedAt == null);

        // Apply nutrition filters using LEFT JOIN to NutritionalInfo
        if (minCaloriesKcal.HasValue || maxCaloriesKcal.HasValue ||
            minProteinG.HasValue || maxProteinG.HasValue ||
            minCarbsG.HasValue || maxCarbsG.HasValue ||
            minFatG.HasValue || maxFatG.HasValue ||
            minFiberG.HasValue || maxFiberG.HasValue ||
            minSodiumMg.HasValue || maxSodiumMg.HasValue)
        {
            query = query.Where(r => r.NutritionalInfo != null);

            if (minCaloriesKcal.HasValue)
                query = query.Where(r => r.NutritionalInfo!.CaloriesKcal >= minCaloriesKcal.Value);
            if (maxCaloriesKcal.HasValue)
                query = query.Where(r => r.NutritionalInfo!.CaloriesKcal <= maxCaloriesKcal.Value);
            if (minProteinG.HasValue)
                query = query.Where(r => r.NutritionalInfo!.ProteinG >= minProteinG.Value);
            if (maxProteinG.HasValue)
                query = query.Where(r => r.NutritionalInfo!.ProteinG <= maxProteinG.Value);
            if (minCarbsG.HasValue)
                query = query.Where(r => r.NutritionalInfo!.CarbsG >= minCarbsG.Value);
            if (maxCarbsG.HasValue)
                query = query.Where(r => r.NutritionalInfo!.CarbsG <= maxCarbsG.Value);
            if (minFatG.HasValue)
                query = query.Where(r => r.NutritionalInfo!.FatG >= minFatG.Value);
            if (maxFatG.HasValue)
                query = query.Where(r => r.NutritionalInfo!.FatG <= maxFatG.Value);
            if (minFiberG.HasValue)
                query = query.Where(r => r.NutritionalInfo!.FiberG >= minFiberG.Value);
            if (maxFiberG.HasValue)
                query = query.Where(r => r.NutritionalInfo!.FiberG <= maxFiberG.Value);
            if (minSodiumMg.HasValue)
                query = query.Where(r => r.NutritionalInfo!.SodiumMg >= minSodiumMg.Value);
            if (maxSodiumMg.HasValue)
                query = query.Where(r => r.NutritionalInfo!.SodiumMg <= maxSodiumMg.Value);
        }

        return await query.OrderBy(r => r.Name).ToListAsync();
    }
}
