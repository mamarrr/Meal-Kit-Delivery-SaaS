using App.Contracts.BLL.Menu;
using App.Contracts.DAL.Menu;
using App.Domain.Menu;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Menu;

public class WeeklyMenuService : BaseTenantService<WeeklyMenu, IWeeklyMenuRepository>, IWeeklyMenuService
{
    public WeeklyMenuService(IWeeklyMenuRepository repository) : base(repository)
    {
    }

    protected override async Task<ICollection<WeeklyMenu>> GetAllByCompanyIdCoreAsync(Guid companyId)
    {
        return await Repository.GetAllByCompanyIdAsync(companyId);
    }

    public async Task<WeeklyMenuRuleConfigDto> GetRuleConfigAsync(Guid companyId)
    {
        var config = await Repository.GetRuleConfigByCompanyIdAsync(companyId);
        if (config == null)
        {
            return new WeeklyMenuRuleConfigDto
            {
                RecipesPerCategory = 2,
                NoRepeatWeeks = 8,
                SelectionDeadlineDaysBeforeWeekStart = 2
            };
        }

        return new WeeklyMenuRuleConfigDto
        {
            RecipesPerCategory = config.RecipesPerCategory,
            NoRepeatWeeks = config.NoRepeatWeeks,
            SelectionDeadlineDaysBeforeWeekStart = config.SelectionDeadlineDaysBeforeWeekStart
        };
    }

    public async Task<WeeklyMenuRuleConfigDto> SaveRuleConfigAsync(Guid companyId, WeeklyMenuRuleConfigUpdateDto dto)
    {
        var sanitized = new WeeklyMenuRuleConfig
        {
            CompanyId = companyId,
            RecipesPerCategory = Math.Max(1, dto.RecipesPerCategory),
            NoRepeatWeeks = Math.Max(0, dto.NoRepeatWeeks),
            SelectionDeadlineDaysBeforeWeekStart = Math.Max(0, dto.SelectionDeadlineDaysBeforeWeekStart),
            UpdatedAt = DateTime.UtcNow
        };

        var existing = await Repository.GetRuleConfigByCompanyIdAsync(companyId);
        if (existing == null)
        {
            sanitized.CreatedAt = DateTime.UtcNow;
            Repository.AddRuleConfig(sanitized);
        }
        else
        {
            existing.RecipesPerCategory = sanitized.RecipesPerCategory;
            existing.NoRepeatWeeks = sanitized.NoRepeatWeeks;
            existing.SelectionDeadlineDaysBeforeWeekStart = sanitized.SelectionDeadlineDaysBeforeWeekStart;
            existing.UpdatedAt = sanitized.UpdatedAt;
            Repository.UpdateRuleConfig(existing);
        }

        return await GetRuleConfigAsync(companyId);
    }

    public async Task<ICollection<WeeklyMenuAssignmentDto>> GetWeeklyAssignmentsAsync(Guid companyId, DateTime weekStartDate)
    {
        var assignments = await Repository.GetWeeklyAssignmentsAsync(companyId, weekStartDate.Date);

        return assignments
            .OrderBy(x => x.DisplayOrder ?? int.MaxValue)
            .ThenBy(x => x.Recipe!.Name)
            .Select(x => new WeeklyMenuAssignmentDto
            {
                WeeklyMenuRecipeId = x.Id,
                RecipeId = x.RecipeId,
                RecipeName = x.Recipe?.Name ?? string.Empty,
                DietaryCategoryId = x.DietaryCategoryId,
                DietaryCategoryName = x.DietaryCategory?.Name ?? "Uncategorized",
                DisplayOrder = x.DisplayOrder
            })
            .ToList();
    }

    public async Task<WeeklyMenuAssignmentResultDto> AssignRecipeToWeekAsync(Guid companyId, WeeklyMenuAssignmentCreateDto dto)
    {
        var weekStart = dto.WeekStartDate.Date;
        var config = await GetRuleConfigAsync(companyId);
        var weeklyAssignments = await Repository.GetWeeklyAssignmentsAsync(companyId, weekStart);

        var categoryCount = weeklyAssignments.Count(x => x.DietaryCategoryId == dto.DietaryCategoryId);
        if (categoryCount >= config.RecipesPerCategory)
        {
            return new WeeklyMenuAssignmentResultDto
            {
                Success = false,
                Message = "Category quota exceeded for selected week."
            };
        }

        var alreadyAssigned = weeklyAssignments.Any(x => x.RecipeId == dto.RecipeId);
        if (alreadyAssigned)
        {
            return new WeeklyMenuAssignmentResultDto
            {
                Success = false,
                Message = "Recipe is already assigned to the selected week."
            };
        }

        var repeated = await Repository.HasRecipeAssignedInPreviousWeeksAsync(companyId, dto.RecipeId, weekStart, config.NoRepeatWeeks);
        if (repeated)
        {
            return new WeeklyMenuAssignmentResultDto
            {
                Success = false,
                Message = $"Recipe violates no-repeat rule ({config.NoRepeatWeeks} weeks)."
            };
        }

        var weeklyMenu = (await Repository.GetAllByCompanyIdAsync(companyId))
            .FirstOrDefault(x => x.WeekStartDate.Date == weekStart);
        var isNewWeeklyMenu = weeklyMenu == null;

        if (isNewWeeklyMenu)
        {
            var deadline = weekStart.AddDays(-config.SelectionDeadlineDaysBeforeWeekStart);
            weeklyMenu = await AddAsync(new WeeklyMenu
            {
                CompanyId = companyId,
                WeekStartDate = weekStart,
                SelectionDeadlineAt = deadline,
                TotalRecipes = 1,
                IsPublished = false,
                CreatedAt = DateTime.UtcNow,
                CreatedByAppUserId = dto.CreatedByAppUserId
            }, companyId);
        }

        if (weeklyMenu == null)
        {
            throw new InvalidOperationException("Unable to resolve weekly menu for assignment.");
        }

        var assignment = new WeeklyMenuRecipe
        {
            Id = Guid.NewGuid(),
            WeeklyMenuId = weeklyMenu.Id,
            RecipeId = dto.RecipeId,
            DietaryCategoryId = dto.DietaryCategoryId,
            DisplayOrder = weeklyAssignments.Count + 1,
            IsFeatured = false,
            CreatedByAppUserId = dto.CreatedByAppUserId,
            CreatedAt = DateTime.UtcNow
        };

        Repository.AddWeeklyAssignment(assignment);

        if (!isNewWeeklyMenu)
        {
            weeklyMenu.TotalRecipes = weeklyAssignments.Count + 1;
            weeklyMenu.UpdatedAt = DateTime.UtcNow;
            Repository.Update(weeklyMenu);
        }

        var latestAssignments = await GetWeeklyAssignmentsAsync(companyId, weekStart);
        var created = latestAssignments.FirstOrDefault(x => x.RecipeId == dto.RecipeId && x.DietaryCategoryId == dto.DietaryCategoryId);

        return new WeeklyMenuAssignmentResultDto
        {
            Success = true,
            Message = "Recipe assignment created.",
            Assignment = created
        };
    }

    public async Task<WeeklyMenuSimulationResultDto> SimulateAutoSelectionAsync(Guid companyId, WeeklyMenuSimulationRequestDto dto)
    {
        var weekStart = dto.WeekStartDate.Date;
        var config = await GetRuleConfigAsync(companyId);
        var candidates = await Repository.GetSimulationCandidatesAsync(companyId, weekStart);

        var grouped = candidates
            .GroupBy(x => new { x.DietaryCategoryId, x.DietaryCategoryName })
            .OrderBy(x => x.Key.DietaryCategoryName)
            .ToList();

        var result = new WeeklyMenuSimulationResultDto
        {
            WeekStartDate = weekStart
        };

        foreach (var category in grouped)
        {
            var selected = new List<WeeklyMenuSimulationRecipeDto>();
            var excluded = new List<WeeklyMenuSimulationExclusionDto>();

            foreach (var candidate in category)
            {
                var repeated = await Repository.HasRecipeAssignedInPreviousWeeksAsync(companyId, candidate.RecipeId, weekStart, config.NoRepeatWeeks);
                if (repeated)
                {
                    excluded.Add(new WeeklyMenuSimulationExclusionDto
                    {
                        RecipeId = candidate.RecipeId,
                        RecipeName = candidate.RecipeName,
                        Reason = $"Excluded by no-repeat rule ({config.NoRepeatWeeks} weeks)."
                    });
                    continue;
                }

                if (selected.Count >= config.RecipesPerCategory)
                {
                    excluded.Add(new WeeklyMenuSimulationExclusionDto
                    {
                        RecipeId = candidate.RecipeId,
                        RecipeName = candidate.RecipeName,
                        Reason = "Excluded because category quota was already reached."
                    });
                    continue;
                }

                selected.Add(new WeeklyMenuSimulationRecipeDto
                {
                    RecipeId = candidate.RecipeId,
                    RecipeName = candidate.RecipeName
                });
            }

            result.Categories.Add(new WeeklyMenuSimulationCategoryResultDto
            {
                DietaryCategoryId = category.Key.DietaryCategoryId,
                DietaryCategoryName = category.Key.DietaryCategoryName,
                RequiredCount = config.RecipesPerCategory,
                SelectedCount = selected.Count,
                SelectedRecipes = selected
            });

            foreach (var exclusion in excluded)
            {
                result.Exclusions.Add(exclusion);
            }

            if (selected.Count < config.RecipesPerCategory)
            {
                result.Exclusions.Add(new WeeklyMenuSimulationExclusionDto
                {
                    RecipeId = Guid.Empty,
                    RecipeName = category.Key.DietaryCategoryName,
                    Reason = $"Unmet category quota: required {config.RecipesPerCategory}, selected {selected.Count}."
                });
            }
        }

        return result;
    }
}

