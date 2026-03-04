using App.Contracts.DAL.Menu;
using App.Domain.Menu;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories.Menu;

/// <summary>
/// EF Core repository implementation for MealSelection aggregate.
/// </summary>
public class MealSelectionRepository : BaseRepository<MealSelection, AppDbContext>, IMealSelectionRepository
{
    public MealSelectionRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<ICollection<MealSelection>> GetAllByCompanyIdAsync(Guid companyId)
    {
        return await RepositoryDbSet
            .Include(ms => ms.MealSubscription)
            .Include(ms => ms.WeeklyMenu)
            .Include(ms => ms.Recipe)
            .Where(ms => ms.WeeklyMenu != null && ms.WeeklyMenu.CompanyId == companyId)
            .ToListAsync();
    }

    public async Task<MealSelection?> GetByIdAsync(Guid id, Guid companyId)
    {
        return await RepositoryDbSet
            .Include(ms => ms.MealSubscription)
            .Include(ms => ms.WeeklyMenu)
            .Include(ms => ms.Recipe)
            .FirstOrDefaultAsync(ms => ms.Id == id && ms.WeeklyMenu != null && ms.WeeklyMenu.CompanyId == companyId);
    }

    public async Task<bool> IsMealSubscriptionInCompanyAsync(Guid mealSubscriptionId, Guid companyId)
    {
        return await RepositoryDbContext.MealSubscriptions.AnyAsync(ms => ms.Id == mealSubscriptionId && ms.CompanyId == companyId);
    }

    public async Task<bool> IsWeeklyMenuInCompanyAsync(Guid weeklyMenuId, Guid companyId)
    {
        return await RepositoryDbContext.WeeklyMenus.AnyAsync(wm => wm.Id == weeklyMenuId && wm.CompanyId == companyId);
    }

    public async Task<bool> IsRecipeInCompanyAsync(Guid recipeId, Guid companyId)
    {
        return await RepositoryDbContext.Recipes.AnyAsync(r => r.Id == recipeId && r.CompanyId == companyId);
    }

    /// <inheritdoc />
    public async Task<ICollection<MealSelection>> GetAllByMealSubscriptionIdAsync(Guid mealSubscriptionId)
    {
        return await RepositoryDbSet
            .Where(ms => ms.MealSubscriptionId == mealSubscriptionId)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<ICollection<MealSelection>> GetAllByWeeklyMenuIdAsync(Guid weeklyMenuId)
    {
        return await RepositoryDbSet
            .Where(ms => ms.WeeklyMenuId == weeklyMenuId)
            .ToListAsync();
    }
}
