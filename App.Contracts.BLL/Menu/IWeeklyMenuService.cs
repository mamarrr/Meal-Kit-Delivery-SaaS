using App.Domain.Menu;

namespace App.Contracts.BLL.Menu;

public interface IWeeklyMenuService : ITenantEntityService<WeeklyMenu>
{
    Task<WeeklyMenuRuleConfigDto> GetRuleConfigAsync(Guid companyId);
    Task<WeeklyMenuRuleConfigDto> SaveRuleConfigAsync(Guid companyId, WeeklyMenuRuleConfigUpdateDto dto);

    Task<ICollection<WeeklyMenuAssignmentDto>> GetWeeklyAssignmentsAsync(Guid companyId, DateTime weekStartDate);
    Task<WeeklyMenuAssignmentResultDto> AssignRecipeToWeekAsync(Guid companyId, WeeklyMenuAssignmentCreateDto dto);
    Task<WeeklyMenuAssignmentResultDto> RemoveWeeklyAssignmentAsync(Guid companyId, Guid weeklyMenuRecipeId);

    Task<WeeklyMenuSimulationResultDto> SimulateAutoSelectionAsync(Guid companyId, WeeklyMenuSimulationRequestDto dto);

    Task<WeeklyMenuAutoSelectionResultDto> ApplyAutoSelectionAsync(Guid companyId, WeeklyMenuAutoSelectionRequestDto dto);
}

public sealed class WeeklyMenuRuleConfigDto
{
    public int RecipesPerCategory { get; init; }
    public int NoRepeatWeeks { get; init; }
    public int SelectionDeadlineDaysBeforeWeekStart { get; init; }
}

public sealed class WeeklyMenuRuleConfigUpdateDto
{
    public int RecipesPerCategory { get; init; }
    public int NoRepeatWeeks { get; init; }
    public int SelectionDeadlineDaysBeforeWeekStart { get; init; }
}

public sealed class WeeklyMenuAssignmentCreateDto
{
    public DateTime WeekStartDate { get; init; }
    public Guid RecipeId { get; init; }
    public Guid? DietaryCategoryId { get; init; }
    public Guid CreatedByAppUserId { get; init; }
}

public sealed class WeeklyMenuAssignmentDto
{
    public Guid WeeklyMenuRecipeId { get; init; }
    public Guid RecipeId { get; init; }
    public string RecipeName { get; init; } = string.Empty;
    public Guid? DietaryCategoryId { get; init; }
    public string DietaryCategoryName { get; init; } = string.Empty;
    public int? DisplayOrder { get; init; }
}

public sealed class WeeklyMenuAssignmentResultDto
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public WeeklyMenuAssignmentDto? Assignment { get; init; }
}

public sealed class WeeklyMenuSimulationRequestDto
{
    public DateTime WeekStartDate { get; init; }
}

public sealed class WeeklyMenuSimulationResultDto
{
    public DateTime WeekStartDate { get; init; }
    public ICollection<WeeklyMenuSimulationCategoryResultDto> Categories { get; init; } = new List<WeeklyMenuSimulationCategoryResultDto>();
    public ICollection<WeeklyMenuSimulationExclusionDto> Exclusions { get; init; } = new List<WeeklyMenuSimulationExclusionDto>();
}

public sealed class WeeklyMenuSimulationCategoryResultDto
{
    public Guid? DietaryCategoryId { get; init; }
    public string DietaryCategoryName { get; init; } = string.Empty;
    public int RequiredCount { get; init; }
    public int SelectedCount { get; init; }
    public ICollection<WeeklyMenuSimulationRecipeDto> SelectedRecipes { get; init; } = new List<WeeklyMenuSimulationRecipeDto>();
}

public sealed class WeeklyMenuSimulationRecipeDto
{
    public Guid RecipeId { get; init; }
    public string RecipeName { get; init; } = string.Empty;
}

public sealed class WeeklyMenuSimulationExclusionDto
{
    public Guid RecipeId { get; init; }
    public string RecipeName { get; init; } = string.Empty;
    public string Reason { get; init; } = string.Empty;
}

public sealed class WeeklyMenuAutoSelectionRequestDto
{
    public Guid MealSubscriptionId { get; init; }
    public DateTime WeekStartDate { get; init; }
    public DateTime? CurrentUtc { get; init; }
    public NutritionFilterDto? NutritionFilter { get; init; }
}

public sealed class WeeklyMenuAutoSelectionResultDto
{
    public Guid MealSubscriptionId { get; init; }
    public DateTime WeekStartDate { get; init; }
    public bool DeadlinePassed { get; init; }
    public bool AutoSelectionApplied { get; init; }
    public int RequiredMealCount { get; init; }
    public int SelectedMealCount { get; init; }
    public string Message { get; init; } = string.Empty;
    public IReadOnlyCollection<Guid> SelectedRecipeIds { get; init; } = [];
}

