using App.Contracts.DAL.Menu;
using App.Domain.Menu;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories.Menu;

/// <summary>
/// EF Core repository implementation for WeeklyMenu aggregate.
/// </summary>
public class WeeklyMenuRepository : BaseRepository<WeeklyMenu, AppDbContext>, IWeeklyMenuRepository
{
    public WeeklyMenuRepository(AppDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<ICollection<WeeklyMenu>> GetAllByCompanyIdAsync(Guid companyId)
    {
        return await RepositoryDbSet
            .Where(wm => wm.CompanyId == companyId)
            .ToListAsync();
    }

    public async Task<WeeklyMenuRuleConfig?> GetRuleConfigByCompanyIdAsync(Guid companyId)
    {
        return await RepositoryDbContext.WeeklyMenuRuleConfigs
            .FirstOrDefaultAsync(x => x.CompanyId == companyId);
    }

    public WeeklyMenuRuleConfig AddRuleConfig(WeeklyMenuRuleConfig config)
    {
        return RepositoryDbContext.WeeklyMenuRuleConfigs.Add(config).Entity;
    }

    public WeeklyMenuRuleConfig UpdateRuleConfig(WeeklyMenuRuleConfig config)
    {
        return RepositoryDbContext.WeeklyMenuRuleConfigs.Update(config).Entity;
    }

    public async Task<ICollection<WeeklyMenuRecipe>> GetWeeklyAssignmentsAsync(Guid companyId, DateTime weekStartDate)
    {
        return await RepositoryDbContext.WeeklyMenuRecipes
            .Include(x => x.Recipe)
            .Include(x => x.DietaryCategory)
            .Include(x => x.WeeklyMenu)
            .Where(x => x.WeeklyMenu != null
                        && x.WeeklyMenu.CompanyId == companyId
                        && x.WeeklyMenu.WeekStartDate == weekStartDate
                        && x.DeletedAt == null)
            .ToListAsync();
    }

    public WeeklyMenuRecipe AddWeeklyAssignment(WeeklyMenuRecipe assignment)
    {
        return RepositoryDbContext.WeeklyMenuRecipes.Add(assignment).Entity;
    }

    public async Task<bool> HasRecipeAssignedInPreviousWeeksAsync(Guid companyId, Guid recipeId, DateTime weekStartDate, int noRepeatWeeks)
    {
        var windowStart = weekStartDate.Date.AddDays(-(7 * Math.Max(0, noRepeatWeeks)));

        return await RepositoryDbContext.WeeklyMenuRecipes
            .Include(x => x.WeeklyMenu)
            .AnyAsync(x => x.RecipeId == recipeId
                           && x.DeletedAt == null
                           && x.WeeklyMenu != null
                           && x.WeeklyMenu.CompanyId == companyId
                           && x.WeeklyMenu.WeekStartDate >= windowStart
                           && x.WeeklyMenu.WeekStartDate < weekStartDate.Date);
    }

    public async Task<ICollection<RecipeSimulationCandidate>> GetSimulationCandidatesAsync(Guid companyId, DateTime weekStartDate)
    {
        var alreadyAssignedRecipeIds = await RepositoryDbContext.WeeklyMenuRecipes
            .Include(x => x.WeeklyMenu)
            .Where(x => x.WeeklyMenu != null
                        && x.WeeklyMenu.CompanyId == companyId
                        && x.WeeklyMenu.WeekStartDate == weekStartDate.Date
                        && x.DeletedAt == null)
            .Select(x => x.RecipeId)
            .ToListAsync();

        var baseQuery = RepositoryDbContext.Recipes
            .Where(r => r.CompanyId == companyId && r.IsActive && r.DeletedAt == null && !alreadyAssignedRecipeIds.Contains(r.Id));

        var withCategory = await (
            from r in baseQuery
            join rdc in RepositoryDbContext.RecipeDietaryCategories on r.Id equals rdc.RecipeId
            join dc in RepositoryDbContext.DietaryCategories on rdc.DietaryCategoryId equals dc.Id
            where rdc.DeletedAt == null && dc.IsActive && dc.DeletedAt == null
            select new RecipeSimulationCandidate
            {
                RecipeId = r.Id,
                RecipeName = r.Name,
                DietaryCategoryId = dc.Id,
                DietaryCategoryName = dc.Name
            })
            .ToListAsync();

        var withoutCategory = await (
            from r in baseQuery
            where !RepositoryDbContext.RecipeDietaryCategories.Any(rdc => rdc.RecipeId == r.Id && rdc.DeletedAt == null)
            select new RecipeSimulationCandidate
            {
                RecipeId = r.Id,
                RecipeName = r.Name,
                DietaryCategoryId = null,
                DietaryCategoryName = "Uncategorized"
            })
            .ToListAsync();

        return withCategory.Concat(withoutCategory).ToList();
    }
}
