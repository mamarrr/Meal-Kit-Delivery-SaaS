using App.Domain.Menu;

namespace App.Contracts.BLL.Menu;

public interface IMealSelectionService : IBaseEntityService<MealSelection>
{
    Task<ICollection<MealSelection>> GetAllByCompanyIdAsync(Guid companyId);
    Task<MealSelection?> GetByIdAsync(Guid id, Guid companyId);
    Task<MealSelection> AddAsync(MealSelection entity, Guid companyId);
    Task<MealSelection> UpdateAsync(MealSelection entity, Guid companyId);
    Task<MealSelection> RemoveAsync(Guid id, Guid companyId);
    Task<ICollection<MealSelection>> GetAllByMealSubscriptionIdAsync(Guid mealSubscriptionId);
    Task<ICollection<MealSelection>> GetAllByWeeklyMenuIdAsync(Guid weeklyMenuId);
}

