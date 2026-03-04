using App.Contracts.BLL.Menu;
using App.Contracts.DAL.Menu;
using App.Domain.Menu;

namespace App.BLL.Menu;

public class RecipeService : BaseTenantService<Recipe, IRecipeRepository>, IRecipeService
{
    public RecipeService(IRecipeRepository repository) : base(repository)
    {
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
}

