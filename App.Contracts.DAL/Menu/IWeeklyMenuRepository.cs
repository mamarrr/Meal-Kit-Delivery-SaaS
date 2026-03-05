using App.Domain.Menu;
using Contracts.DAL;

namespace App.Contracts.DAL.Menu;

/// <summary>
/// Repository interface for WeeklyMenu aggregate.
/// </summary>
public interface IWeeklyMenuRepository : IRepository<WeeklyMenu>
{
    /// <summary>
    /// Gets all weekly menus for a specific company.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <returns>A collection of weekly menus belonging to the company.</returns>
    Task<ICollection<WeeklyMenu>> GetAllByCompanyIdAsync(Guid companyId);

    Task<WeeklyMenuRuleConfig?> GetRuleConfigByCompanyIdAsync(Guid companyId);
    WeeklyMenuRuleConfig AddRuleConfig(WeeklyMenuRuleConfig config);
    WeeklyMenuRuleConfig UpdateRuleConfig(WeeklyMenuRuleConfig config);

    Task<ICollection<WeeklyMenuRecipe>> GetWeeklyAssignmentsAsync(Guid companyId, DateTime weekStartDate);
    WeeklyMenuRecipe AddWeeklyAssignment(WeeklyMenuRecipe assignment);
    Task<bool> HasRecipeAssignedInPreviousWeeksAsync(Guid companyId, Guid recipeId, DateTime weekStartDate, int noRepeatWeeks);

    Task<ICollection<RecipeSimulationCandidate>> GetSimulationCandidatesAsync(Guid companyId, DateTime weekStartDate);
}

public sealed class RecipeSimulationCandidate
{
    public Guid RecipeId { get; init; }
    public string RecipeName { get; init; } = string.Empty;
    public Guid? DietaryCategoryId { get; init; }
    public string DietaryCategoryName { get; init; } = string.Empty;
}
