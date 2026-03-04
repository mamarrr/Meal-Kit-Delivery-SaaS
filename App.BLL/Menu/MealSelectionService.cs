using App.Contracts.BLL.Menu;
using App.Contracts.DAL.Menu;
using App.Domain.Menu;

namespace App.BLL.Menu;

public class MealSelectionService : BaseService<MealSelection, IMealSelectionRepository>, IMealSelectionService
{
    public MealSelectionService(IMealSelectionRepository repository) : base(repository)
    {
    }

    public async Task<ICollection<MealSelection>> GetAllByMealSubscriptionIdAsync(Guid mealSubscriptionId)
    {
        return await Repository.GetAllByMealSubscriptionIdAsync(mealSubscriptionId);
    }

    public async Task<ICollection<MealSelection>> GetAllByWeeklyMenuIdAsync(Guid weeklyMenuId)
    {
        return await Repository.GetAllByWeeklyMenuIdAsync(weeklyMenuId);
    }
}

