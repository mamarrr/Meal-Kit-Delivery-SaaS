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
