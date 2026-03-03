using App.Domain.Menu;
using Contracts.DAL;

namespace App.Contracts.DAL.Menu;

/// <summary>
/// Repository interface for MealSelection aggregate.
/// </summary>
public interface IMealSelectionRepository : IRepository<MealSelection>
{
    /// <summary>
    /// Gets all meal selections for a specific meal subscription.
    /// </summary>
    /// <param name="mealSubscriptionId">The meal subscription ID.</param>
    /// <returns>A collection of meal selections.</returns>
    Task<ICollection<MealSelection>> GetAllByMealSubscriptionIdAsync(Guid mealSubscriptionId);
    
    /// <summary>
    /// Gets all meal selections for a specific weekly menu.
    /// </summary>
    /// <param name="weeklyMenuId">The weekly menu ID.</param>
    /// <returns>A collection of meal selections.</returns>
    Task<ICollection<MealSelection>> GetAllByWeeklyMenuIdAsync(Guid weeklyMenuId);
}
