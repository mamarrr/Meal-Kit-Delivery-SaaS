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
}
