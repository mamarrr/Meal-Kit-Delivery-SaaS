using App.Contracts.BLL.Menu;
using App.Contracts.DAL.Menu;
using App.Domain.Menu;

namespace App.BLL.Menu;

public class MealSelectionService : BaseService<MealSelection, IMealSelectionRepository>, IMealSelectionService
{
    public MealSelectionService(IMealSelectionRepository repository) : base(repository)
    {
    }

    public async Task<ICollection<MealSelection>> GetAllByCompanyIdAsync(Guid companyId)
    {
        return await Repository.GetAllByCompanyIdAsync(companyId);
    }

    public async Task<MealSelection?> GetByIdAsync(Guid id, Guid companyId)
    {
        return await Repository.GetByIdAsync(id, companyId);
    }

    public async Task<MealSelection> AddAsync(MealSelection entity, Guid companyId)
    {
        var subscriptionInCompany = await Repository.IsMealSubscriptionInCompanyAsync(entity.MealSubscriptionId, companyId);
        var weeklyMenuInCompany = await Repository.IsWeeklyMenuInCompanyAsync(entity.WeeklyMenuId, companyId);
        var recipeInCompany = await Repository.IsRecipeInCompanyAsync(entity.RecipeId, companyId);

        if (!subscriptionInCompany || !weeklyMenuInCompany || !recipeInCompany)
        {
            throw new KeyNotFoundException("Meal selection references are outside company scope.");
        }

        return await base.AddAsync(entity);
    }

    public async Task<MealSelection> UpdateAsync(MealSelection entity, Guid companyId)
    {
        var existing = await Repository.GetByIdAsync(entity.Id, companyId);
        if (existing == null)
        {
            throw new KeyNotFoundException($"Meal selection {entity.Id} was not found in company scope {companyId}.");
        }

        var subscriptionInCompany = await Repository.IsMealSubscriptionInCompanyAsync(entity.MealSubscriptionId, companyId);
        var weeklyMenuInCompany = await Repository.IsWeeklyMenuInCompanyAsync(entity.WeeklyMenuId, companyId);
        var recipeInCompany = await Repository.IsRecipeInCompanyAsync(entity.RecipeId, companyId);

        if (!subscriptionInCompany || !weeklyMenuInCompany || !recipeInCompany)
        {
            throw new KeyNotFoundException("Meal selection references are outside company scope.");
        }

        return await base.UpdateAsync(entity);
    }

    public async Task<MealSelection> RemoveAsync(Guid id, Guid companyId)
    {
        var existing = await Repository.GetByIdAsync(id, companyId);
        if (existing == null)
        {
            throw new KeyNotFoundException($"Meal selection {id} was not found in company scope {companyId}.");
        }

        return await base.RemoveAsync(id);
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

