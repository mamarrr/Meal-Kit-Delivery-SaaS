using App.Contracts.BLL.Menu;
using App.Contracts.DAL.Menu;
using App.Domain.Menu;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace App.BLL.Menu;

public class RecipeService : BaseTenantService<Recipe, IRecipeRepository>, IRecipeService
{
    private readonly ILogger<RecipeService> _logger;

    public RecipeService(IRecipeRepository repository, ILogger<RecipeService>? logger = null) : base(repository)
    {
        _logger = logger ?? NullLogger<RecipeService>.Instance;
    }

    protected override async Task<ICollection<Recipe>> GetAllByCompanyIdCoreAsync(Guid companyId)
    {
        return await Repository.GetAllByCompanyIdAsync(companyId);
    }

    public async Task<int> CountActiveByCompanyIdAsync(Guid companyId)
    {
        return await Repository.CountActiveByCompanyIdAsync(companyId);
    }

    public async Task UpsertNutritionalInfoAsync(
        Guid recipeId,
        Guid companyId,
        decimal caloriesKcal,
        decimal proteinG,
        decimal carbsG,
        decimal fatG,
        decimal fiberG,
        decimal sodiumMg,
        decimal sugarG,
        decimal saturatedFatG)
    {
        var recipe = await Repository.GetByIdAsync(recipeId);
        if (recipe == null || recipe.CompanyId != companyId)
        {
            throw new KeyNotFoundException($"Recipe {recipeId} was not found in company scope {companyId}.");
        }

        var nutritionalInfo = await Repository.GetNutritionalInfoByRecipeIdAsync(recipeId);
        if (nutritionalInfo == null)
        {
            nutritionalInfo = new NutritionalInfo
            {
                RecipeId = recipeId,
                CreatedAt = DateTime.UtcNow,
                CaloriesKcal = caloriesKcal,
                ProteinG = proteinG,
                CarbsG = carbsG,
                FatG = fatG,
                FiberG = fiberG,
                SodiumMg = sodiumMg,
                SugarG = sugarG,
                SaturatedFatG = saturatedFatG
            };

            Repository.AddNutritionalInfo(nutritionalInfo);
            return;
        }

        nutritionalInfo.UpdatedAt = DateTime.UtcNow;
        nutritionalInfo.CaloriesKcal = caloriesKcal;
        nutritionalInfo.ProteinG = proteinG;
        nutritionalInfo.CarbsG = carbsG;
        nutritionalInfo.FatG = fatG;
        nutritionalInfo.FiberG = fiberG;
        nutritionalInfo.SodiumMg = sodiumMg;
        nutritionalInfo.SugarG = sugarG;
        nutritionalInfo.SaturatedFatG = saturatedFatG;

        Repository.UpdateNutritionalInfo(nutritionalInfo);
    }

    public async Task<ICollection<RecipeListItemDto>> GetRecipeListAsync(Guid companyId, RecipeListFilterDto? filter = null)
    {
        var rows = await Repository.GetAllByCompanyIdWithDetailsAsync(companyId);

        IEnumerable<Recipe> query = rows.Where(r => r.DeletedAt == null);
        if (filter != null)
        {
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.Trim();
                query = query.Where(x => x.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            if (filter.ActiveOnly == true)
            {
                query = query.Where(x => x.IsActive);
            }

            if (filter.DietaryCategoryId.HasValue)
            {
                query = query.Where(x => (x.RecipeDietaryCategories ?? [])
                    .Any(rd => rd.DeletedAt == null
                               && rd.DietaryCategoryId == filter.DietaryCategoryId.Value
                               && rd.DietaryCategory != null
                               && rd.DietaryCategory.DeletedAt == null));
            }

            if (filter.IngredientId.HasValue)
            {
                query = query.Where(x => (x.RecipeIngredients ?? [])
                    .Any(ri => ri.DeletedAt == null
                               && ri.IngredientId == filter.IngredientId.Value
                               && ri.Ingredient != null
                               && ri.Ingredient.DeletedAt == null));
            }
        }

        return query
            .OrderBy(x => x.Name)
            .Select(MapToListItem)
            .ToList();
    }

    public async Task<RecipeEditorDto?> GetRecipeEditorAsync(Guid companyId, Guid recipeId)
    {
        var recipe = await Repository.GetByIdWithDetailsAsync(recipeId, companyId);
        if (recipe == null || recipe.DeletedAt != null)
        {
            return null;
        }

        return MapToEditor(recipe);
    }

    public async Task<RecipeEditorDto> UpsertRecipeEditorAsync(Guid companyId, Guid actorId, RecipeEditorUpsertDto dto)
    {
        ValidateNutrition(dto.Nutrition);

        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            throw new ArgumentException("Recipe name is required.");
        }

        if (dto.DefaultServings < 1)
        {
            throw new ArgumentException("Default servings must be at least 1.");
        }

        var distinctIngredientIds = (dto.IngredientIds ?? Array.Empty<Guid>())
            .Distinct()
            .ToList();
        var distinctDietaryCategoryIds = (dto.DietaryCategoryIds ?? Array.Empty<Guid>())
            .Distinct()
            .ToList();

        var existingIngredientCount = await Repository.CountExistingIngredientIdsAsync(companyId, distinctIngredientIds);
        if (existingIngredientCount != distinctIngredientIds.Count)
        {
            throw new ArgumentException("One or more selected ingredients are invalid for the active company scope.");
        }

        var existingCategoryCount = await Repository.CountExistingDietaryCategoryIdsAsync(companyId, distinctDietaryCategoryIds);
        if (existingCategoryCount != distinctDietaryCategoryIds.Count)
        {
            throw new ArgumentException("One or more selected dietary categories are invalid for the active company scope.");
        }

        Recipe recipe;
        if (dto.RecipeId.HasValue)
        {
            recipe = await Repository.GetByIdWithDetailsAsync(dto.RecipeId.Value, companyId)
                     ?? throw new KeyNotFoundException($"Recipe {dto.RecipeId.Value} was not found in company scope {companyId}.");

            recipe.Name = dto.Name.Trim();
            recipe.Description = dto.Description?.Trim();
            recipe.DefaultServings = dto.DefaultServings;
            recipe.IsActive = dto.IsActive;
            recipe.UpdatedAt = DateTime.UtcNow;

            recipe = await UpdateAsync(recipe, companyId);
        }
        else
        {
            recipe = await AddAsync(new Recipe
            {
                Id = Guid.NewGuid(),
                CompanyId = companyId,
                CreatedByAppUserId = actorId,
                CreatedAt = DateTime.UtcNow,
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim(),
                DefaultServings = dto.DefaultServings,
                IsActive = dto.IsActive
            }, companyId);
        }

        await UpsertNutritionalInfoAsync(
            recipe.Id,
            companyId,
            dto.Nutrition.CaloriesKcal,
            dto.Nutrition.ProteinG,
            dto.Nutrition.CarbsG,
            dto.Nutrition.FatG,
            dto.Nutrition.FiberG,
            dto.Nutrition.SodiumMg,
            0,
            0);

        await ReplaceIngredientsAsync(recipe.Id, actorId, distinctIngredientIds);
        await ReplaceDietaryCategoriesAsync(recipe.Id, actorId, distinctDietaryCategoryIds);

        if (!dto.RecipeId.HasValue)
        {
            return new RecipeEditorDto
            {
                RecipeId = recipe.Id,
                Name = recipe.Name,
                Description = recipe.Description,
                DefaultServings = recipe.DefaultServings,
                IsActive = recipe.IsActive,
                IngredientIds = distinctIngredientIds,
                DietaryCategoryIds = distinctDietaryCategoryIds,
                Nutrition = new RecipeNutritionDto
                {
                    CaloriesKcal = dto.Nutrition.CaloriesKcal,
                    ProteinG = dto.Nutrition.ProteinG,
                    CarbsG = dto.Nutrition.CarbsG,
                    FatG = dto.Nutrition.FatG,
                    FiberG = dto.Nutrition.FiberG,
                    SodiumMg = dto.Nutrition.SodiumMg
                }
            };
        }

        var updated = await Repository.GetByIdWithDetailsAsync(recipe.Id, companyId)
                      ?? throw new KeyNotFoundException($"Recipe {recipe.Id} was not found in company scope {companyId}.");
        return MapToEditor(updated);
    }

    public async Task RemoveRecipeAsync(Guid companyId, Guid recipeId)
    {
        var recipe = await Repository.GetByIdAsync(recipeId);
        if (recipe == null || recipe.CompanyId != companyId || recipe.DeletedAt != null)
        {
            throw new KeyNotFoundException($"Recipe {recipeId} was not found in company scope {companyId}.");
        }

        recipe.DeletedAt = DateTime.UtcNow;
        recipe.UpdatedAt = DateTime.UtcNow;
        await UpdateAsync(recipe, companyId);
    }

    private static RecipeListItemDto MapToListItem(Recipe recipe)
    {
        return new RecipeListItemDto
        {
            RecipeId = recipe.Id,
            Name = recipe.Name,
            IsActive = recipe.IsActive,
            DietaryCategories = (recipe.RecipeDietaryCategories ?? [])
                .Where(x => x.DeletedAt == null && x.DietaryCategory != null && x.DietaryCategory.DeletedAt == null)
                .Select(x => x.DietaryCategory?.Name ?? string.Empty)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x)
                .ToList(),
            Tags = (recipe.RecipeIngredients ?? [])
                .Where(x => x.DeletedAt == null && x.Ingredient != null && x.Ingredient.DeletedAt == null)
                .Select(x => x.Ingredient?.Name ?? string.Empty)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x)
                .ToList(),
            Nutrition = recipe.NutritionalInfo == null
                ? null
                : new RecipeNutritionDto
                {
                    CaloriesKcal = recipe.NutritionalInfo.CaloriesKcal,
                    ProteinG = recipe.NutritionalInfo.ProteinG,
                    CarbsG = recipe.NutritionalInfo.CarbsG,
                    FatG = recipe.NutritionalInfo.FatG,
                    FiberG = recipe.NutritionalInfo.FiberG,
                    SodiumMg = recipe.NutritionalInfo.SodiumMg
                }
        };
    }

    private static RecipeEditorDto MapToEditor(Recipe recipe)
    {
        return new RecipeEditorDto
        {
            RecipeId = recipe.Id,
            Name = recipe.Name,
            Description = recipe.Description,
            DefaultServings = recipe.DefaultServings,
            IsActive = recipe.IsActive,
            IngredientIds = (recipe.RecipeIngredients ?? [])
                .Where(x => x.DeletedAt == null)
                .Select(x => x.IngredientId)
                .Distinct()
                .ToList(),
            DietaryCategoryIds = (recipe.RecipeDietaryCategories ?? [])
                .Where(x => x.DeletedAt == null)
                .Select(x => x.DietaryCategoryId)
                .Distinct()
                .ToList(),
            Nutrition = recipe.NutritionalInfo == null
                ? null
                : new RecipeNutritionDto
                {
                    CaloriesKcal = recipe.NutritionalInfo.CaloriesKcal,
                    ProteinG = recipe.NutritionalInfo.ProteinG,
                    CarbsG = recipe.NutritionalInfo.CarbsG,
                    FatG = recipe.NutritionalInfo.FatG,
                    FiberG = recipe.NutritionalInfo.FiberG,
                    SodiumMg = recipe.NutritionalInfo.SodiumMg
                }
        };
    }

    private async Task ReplaceIngredientsAsync(Guid recipeId, Guid actorId, IReadOnlyCollection<Guid> ingredientIds)
    {
        var existing = await Repository.GetRecipeIngredientsAsync(recipeId);
        var selected = ingredientIds.Distinct().ToHashSet();

        var toRemove = existing
            .Where(x => !selected.Contains(x.IngredientId))
            .ToList();

        _logger.LogInformation(
            "ReplaceIngredients: recipeId={RecipeId}, selectedIngredientIds={SelectedIngredientIds}, existingRelationCount={ExistingRelationCount}, toRemoveRelationIds={ToRemoveRelationIds}",
            recipeId,
            string.Join(",", selected.OrderBy(x => x)),
            existing.Count,
            string.Join(",", toRemove.Select(x => x.Id).OrderBy(x => x)));

        if (toRemove.Count > 0)
        {
            await Repository.RemoveRecipeIngredientsAsync(toRemove);
        }

        var existingIds = existing
            .Select(x => x.IngredientId)
            .ToHashSet();

        var items = selected
            .Where(x => !existingIds.Contains(x))
            .Select(x => new RecipeIngredient
            {
                Id = Guid.NewGuid(),
                RecipeId = recipeId,
                IngredientId = x,
                CreatedByAppUserId = actorId,
                CreatedAt = DateTime.UtcNow
            })
            .ToList();

        _logger.LogInformation(
            "ReplaceIngredients add: recipeId={RecipeId}, existingIngredientIds={ExistingIngredientIds}, newIngredientIds={NewIngredientIds}",
            recipeId,
            string.Join(",", existingIds.OrderBy(x => x)),
            string.Join(",", items.Select(x => x.IngredientId).OrderBy(x => x)));

        if (items.Count > 0)
        {
            await Repository.AddRecipeIngredientsAsync(items);
        }
    }

    private async Task ReplaceDietaryCategoriesAsync(Guid recipeId, Guid actorId, IReadOnlyCollection<Guid> dietaryCategoryIds)
    {
        var existing = await Repository.GetRecipeDietaryCategoriesAsync(recipeId);
        var selected = dietaryCategoryIds.Distinct().ToHashSet();

        var toRemove = existing
            .Where(x => !selected.Contains(x.DietaryCategoryId))
            .ToList();

        _logger.LogInformation(
            "ReplaceDietaryCategories: recipeId={RecipeId}, selectedCategoryIds={SelectedCategoryIds}, existingRelationCount={ExistingRelationCount}, toRemoveRelationIds={ToRemoveRelationIds}",
            recipeId,
            string.Join(",", selected.OrderBy(x => x)),
            existing.Count,
            string.Join(",", toRemove.Select(x => x.Id).OrderBy(x => x)));

        if (toRemove.Count > 0)
        {
            await Repository.RemoveRecipeDietaryCategoriesAsync(toRemove);
        }

        var existingIds = existing
            .Select(x => x.DietaryCategoryId)
            .ToHashSet();

        var items = selected
            .Where(x => !existingIds.Contains(x))
            .Select(x => new RecipeDietaryCategory
            {
                Id = Guid.NewGuid(),
                RecipeId = recipeId,
                DietaryCategoryId = x,
                CreatedByAppUserId = actorId,
                CreatedAt = DateTime.UtcNow
            })
            .ToList();

        _logger.LogInformation(
            "ReplaceDietaryCategories add: recipeId={RecipeId}, existingCategoryIds={ExistingCategoryIds}, newCategoryIds={NewCategoryIds}",
            recipeId,
            string.Join(",", existingIds.OrderBy(x => x)),
            string.Join(",", items.Select(x => x.DietaryCategoryId).OrderBy(x => x)));

        if (items.Count > 0)
        {
            await Repository.AddRecipeDietaryCategoriesAsync(items);
        }
    }

    private static void ValidateNutrition(RecipeNutritionDto nutrition)
    {
        if (nutrition.CaloriesKcal <= 0
            || nutrition.ProteinG < 0
            || nutrition.CarbsG < 0
            || nutrition.FatG < 0
            || nutrition.FiberG < 0
            || nutrition.SodiumMg < 0)
        {
            throw new ArgumentException("Nutrition values are invalid. Calories must be > 0 and other values must be >= 0.");
        }
    }
}

