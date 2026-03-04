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
