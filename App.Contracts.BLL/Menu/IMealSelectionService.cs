using App.Domain.Menu;

namespace App.Contracts.BLL.Menu;

public interface IMealSelectionService : IBaseEntityService<MealSelection>
{
    Task<ICollection<MealSelection>> GetAllByMealSubscriptionIdAsync(Guid mealSubscriptionId);
    Task<ICollection<MealSelection>> GetAllByWeeklyMenuIdAsync(Guid weeklyMenuId);
}

