using App.Contracts.BLL.Menu;
using App.Contracts.DAL.Menu;
using App.DAL.EF;
using App.Domain.Menu;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Menu;

public class WeeklyMenuService : BaseTenantService<WeeklyMenu, IWeeklyMenuRepository>, IWeeklyMenuService
{
    private readonly AppDbContext? _context;

    public WeeklyMenuService(IWeeklyMenuRepository repository) : base(repository)
    {
    }

    public WeeklyMenuService(
        IWeeklyMenuRepository repository,
        AppDbContext context) : base(repository)
    {
        _context = context;
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

        var recipeContext = await Repository.GetRecipeAssignmentContextAsync(companyId, dto.RecipeId);
        if (recipeContext == null)
        {
            return new WeeklyMenuAssignmentResultDto
            {
                Success = false,
                Message = "Selected recipe was not found in company scope or is inactive."
            };
        }

        var resolvedCategoryId = recipeContext.DietaryCategoryId;

        var categoryCount = weeklyAssignments.Count(x => x.DietaryCategoryId == resolvedCategoryId);
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
            DietaryCategoryId = resolvedCategoryId,
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

        return new WeeklyMenuAssignmentResultDto
        {
            Success = true,
            Message = "Recipe assignment created.",
            Assignment = new WeeklyMenuAssignmentDto
            {
                WeeklyMenuRecipeId = assignment.Id,
                RecipeId = assignment.RecipeId,
                DietaryCategoryId = assignment.DietaryCategoryId,
                DisplayOrder = assignment.DisplayOrder
            }
        };
    }

    public async Task<WeeklyMenuAssignmentResultDto> RemoveWeeklyAssignmentAsync(Guid companyId, Guid weeklyMenuRecipeId)
    {
        var assignment = await Repository.GetWeeklyAssignmentByIdAsync(companyId, weeklyMenuRecipeId);
        if (assignment == null)
        {
            return new WeeklyMenuAssignmentResultDto
            {
                Success = false,
                Message = "Weekly assignment was not found in company scope."
            };
        }

        assignment.DeletedAt = DateTime.UtcNow;
        Repository.UpdateWeeklyAssignment(assignment);

        if (assignment.WeeklyMenuId != Guid.Empty)
        {
            var weeklyMenu = await Repository.GetByIdAsync(assignment.WeeklyMenuId);
            if (weeklyMenu != null && weeklyMenu.CompanyId == companyId)
            {
                weeklyMenu.TotalRecipes = Math.Max(0, weeklyMenu.TotalRecipes - 1);
                weeklyMenu.UpdatedAt = DateTime.UtcNow;
                Repository.Update(weeklyMenu);
            }
        }

        return new WeeklyMenuAssignmentResultDto
        {
            Success = true,
            Message = "Recipe assignment removed."
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

    public async Task<WeeklyMenuAutoSelectionResultDto> ApplyAutoSelectionAsync(Guid companyId, WeeklyMenuAutoSelectionRequestDto dto)
    {
        if (_context == null)
        {
            throw new InvalidOperationException("Auto-selection requires database context.");
        }

        var weekStart = dto.WeekStartDate.Date;
        var now = dto.CurrentUtc ?? DateTime.UtcNow;

        var subscription = await _context.MealSubscriptions
            .Include(ms => ms.Box)
            .FirstOrDefaultAsync(ms => ms.Id == dto.MealSubscriptionId && ms.DeletedAt == null);

        if (subscription == null || subscription.CompanyId != companyId)
        {
            return new WeeklyMenuAutoSelectionResultDto
            {
                MealSubscriptionId = dto.MealSubscriptionId,
                WeekStartDate = weekStart,
                DeadlinePassed = false,
                AutoSelectionApplied = false,
                RequiredMealCount = 0,
                SelectedMealCount = 0,
                Message = "Meal subscription is not available in company scope."
            };
        }

        if (!subscription.AutoSelectEnabled)
        {
            return new WeeklyMenuAutoSelectionResultDto
            {
                MealSubscriptionId = subscription.Id,
                WeekStartDate = weekStart,
                DeadlinePassed = false,
                AutoSelectionApplied = false,
                RequiredMealCount = 0,
                SelectedMealCount = 0,
                Message = "Auto-selection is disabled for this subscription."
            };
        }

        var weeklyMenu = await _context.WeeklyMenus
            .Include(wm => wm.WeeklyMenuRecipes!)
                .ThenInclude(wmr => wmr.Recipe)
            .FirstOrDefaultAsync(wm => wm.CompanyId == companyId && wm.WeekStartDate == weekStart && wm.DeletedAt == null);

        if (weeklyMenu == null)
        {
            return new WeeklyMenuAutoSelectionResultDto
            {
                MealSubscriptionId = subscription.Id,
                WeekStartDate = weekStart,
                DeadlinePassed = false,
                AutoSelectionApplied = false,
                RequiredMealCount = 0,
                SelectedMealCount = 0,
                Message = "Weekly menu is not available for the requested week."
            };
        }

        var deadline = weeklyMenu.SelectionDeadlineAt == default
            ? weekStart.AddDays(-(await GetRuleConfigAsync(companyId)).SelectionDeadlineDaysBeforeWeekStart)
            : weeklyMenu.SelectionDeadlineAt;

        var deadlinePassed = now > deadline;
        if (!deadlinePassed)
        {
            return new WeeklyMenuAutoSelectionResultDto
            {
                MealSubscriptionId = subscription.Id,
                WeekStartDate = weekStart,
                DeadlinePassed = false,
                AutoSelectionApplied = false,
                RequiredMealCount = 0,
                SelectedMealCount = 0,
                Message = "Selection deadline has not passed."
            };
        }

        var existingSelections = await _context.MealSelections
            .Where(ms => ms.MealSubscriptionId == subscription.Id
                         && ms.WeeklyMenuId == weeklyMenu.Id
                         && ms.DeletedAt == null)
            .ToListAsync();

        if (existingSelections.Count > 0)
        {
            return new WeeklyMenuAutoSelectionResultDto
            {
                MealSubscriptionId = subscription.Id,
                WeekStartDate = weekStart,
                DeadlinePassed = true,
                AutoSelectionApplied = false,
                RequiredMealCount = existingSelections.Count,
                SelectedMealCount = existingSelections.Count,
                SelectedRecipeIds = existingSelections.Select(x => x.RecipeId).ToList(),
                Message = "Meal selections already exist for this week."
            };
        }

        var requiredMealCount = subscription.Box?.MealsCount ?? weeklyMenu.TotalRecipes;
        if (requiredMealCount <= 0)
        {
            requiredMealCount = weeklyMenu.WeeklyMenuRecipes?.Count ?? 0;
        }

        var baseCandidates = (weeklyMenu.WeeklyMenuRecipes ?? [])
            .Where(wmr => wmr.DeletedAt == null
                          && wmr.Recipe != null
                          && wmr.Recipe.IsActive
                          && wmr.Recipe.DeletedAt == null)
            .GroupBy(wmr => wmr.RecipeId)
            .Select(group => new RecipeCandidate
            {
                RecipeId = group.Key,
                RecipeName = group.First().Recipe?.Name ?? string.Empty
            })
            .ToList();

        if (baseCandidates.Count == 0)
        {
            return new WeeklyMenuAutoSelectionResultDto
            {
                MealSubscriptionId = subscription.Id,
                WeekStartDate = weekStart,
                DeadlinePassed = true,
                AutoSelectionApplied = false,
                RequiredMealCount = requiredMealCount,
                SelectedMealCount = 0,
                Message = "No recipes are available for auto-selection."
            };
        }

        var recipeIds = baseCandidates.Select(x => x.RecipeId).ToList();
        var categoryLinks = await _context.RecipeDietaryCategories
            .Where(link => recipeIds.Contains(link.RecipeId) && link.DeletedAt == null)
            .ToListAsync();
        var ingredientLinks = await _context.RecipeIngredients
            .Where(link => recipeIds.Contains(link.RecipeId) && link.DeletedAt == null)
            .ToListAsync();
        var nutritionRows = await _context.NutritionalInfos
            .Where(info => recipeIds.Contains(info.RecipeId))
            .ToListAsync();

        var categoryLookup = categoryLinks
            .GroupBy(link => link.RecipeId)
            .ToDictionary(group => group.Key, group => (IReadOnlyCollection<Guid>)group.Select(x => x.DietaryCategoryId).Distinct().ToList());
        var ingredientLookup = ingredientLinks
            .GroupBy(link => link.RecipeId)
            .ToDictionary(group => group.Key, group => (IReadOnlyCollection<Guid>)group.Select(x => x.IngredientId).Distinct().ToList());
        var nutritionLookup = nutritionRows
            .ToDictionary(info => info.RecipeId, info => info);

        var enrichedCandidates = baseCandidates
            .Select(candidate => candidate with
            {
                DietaryCategoryIds = categoryLookup.TryGetValue(candidate.RecipeId, out var categories)
                    ? categories
                    : [],
                IngredientIds = ingredientLookup.TryGetValue(candidate.RecipeId, out var ingredients)
                    ? ingredients
                    : [],
                Nutrition = nutritionLookup.TryGetValue(candidate.RecipeId, out var nutrition)
                    ? nutrition
                    : null
            })
            .ToList();

        var excludedIngredientIds = await _context.CustomerExclusions
            .Where(ce => ce.CustomerId == subscription.CustomerId && ce.DeletedAt == null)
            .Select(ce => ce.IngredientId)
            .Distinct()
            .ToListAsync();

        var afterExclusions = excludedIngredientIds.Count == 0
            ? enrichedCandidates
            : enrichedCandidates
                .Where(candidate => !candidate.IngredientIds.Any(id => excludedIngredientIds.Contains(id)))
                .ToList();

        var preferenceCategoryIds = await _context.CustomerPreferences
            .Where(cp => cp.CustomerId == subscription.CustomerId && cp.DeletedAt == null)
            .Select(cp => cp.DietaryCategoryId)
            .Distinct()
            .ToListAsync();

        var afterPreferences = preferenceCategoryIds.Count == 0
            ? afterExclusions
            : afterExclusions
                .Where(candidate => candidate.DietaryCategoryIds.Any(id => preferenceCategoryIds.Contains(id)))
                .ToList();

        var afterNutrition = ApplyNutritionFilter(afterPreferences, dto.NutritionFilter);

        var candidatesForRepeatCheck = afterNutrition;
        var message = string.Empty;
        var reason = "deadline_missed";

        if (candidatesForRepeatCheck.Count == 0)
        {
            if (afterExclusions.Count == 0)
            {
                return new WeeklyMenuAutoSelectionResultDto
                {
                    MealSubscriptionId = subscription.Id,
                    WeekStartDate = weekStart,
                    DeadlinePassed = true,
                    AutoSelectionApplied = false,
                    RequiredMealCount = requiredMealCount,
                    SelectedMealCount = 0,
                    Message = "No recipes remain after applying exclusions."
                };
            }

            candidatesForRepeatCheck = afterExclusions;
            message = "No recipes matched your preferences or nutrition filters, selecting from all available recipes.";
            reason = "filters_relaxed";
        }

        var config = await GetRuleConfigAsync(companyId);
        var lookbackWeeks = Math.Max(0, subscription.NoRepeatWeeksOverride ?? config.NoRepeatWeeks);
        var candidatesBeforeNoRepeat = candidatesForRepeatCheck;

        if (lookbackWeeks > 0)
        {
            var windowStart = weekStart.AddDays(-(7 * lookbackWeeks));
            var recentRecipeIds = await (
                    from selection in _context.MealSelections
                    join menu in _context.WeeklyMenus on selection.WeeklyMenuId equals menu.Id
                    where selection.MealSubscriptionId == subscription.Id
                          && selection.DeletedAt == null
                          && menu.CompanyId == companyId
                          && menu.WeekStartDate >= windowStart
                          && menu.WeekStartDate < weekStart
                    select selection.RecipeId)
                .Distinct()
                .ToListAsync();

            if (recentRecipeIds.Count > 0)
            {
                candidatesForRepeatCheck = candidatesForRepeatCheck
                    .Where(candidate => !recentRecipeIds.Contains(candidate.RecipeId))
                    .ToList();
            }

            if (candidatesForRepeatCheck.Count == 0)
            {
                candidatesForRepeatCheck = candidatesBeforeNoRepeat;
                message = string.IsNullOrWhiteSpace(message)
                    ? $"No recipes met the no-repeat rule ({lookbackWeeks} weeks), selecting from all available recipes."
                    : message;
                reason = string.IsNullOrWhiteSpace(reason) ? "no_repeat_relaxed" : reason;
            }
        }

        var ratingScores = await _context.Ratings
            .Where(rating => rating.CustomerId == subscription.CustomerId && rating.DeletedAt == null)
            .GroupBy(rating => rating.RecipeId)
            .Select(group => new { group.Key, Score = group.Average(r => r.Score) })
            .ToListAsync();

        var ratingLookup = ratingScores.ToDictionary(x => x.Key, x => x.Score);

        var orderedCandidates = candidatesForRepeatCheck
            .OrderByDescending(candidate => ratingLookup.TryGetValue(candidate.RecipeId, out var score) ? score : 0)
            .ThenBy(candidate => candidate.RecipeName)
            .ThenBy(candidate => candidate.RecipeId)
            .ToList();

        if (requiredMealCount <= 0)
        {
            requiredMealCount = orderedCandidates.Count;
        }

        var selected = orderedCandidates
            .Take(requiredMealCount)
            .ToList();

        if (selected.Count == 0)
        {
            return new WeeklyMenuAutoSelectionResultDto
            {
                MealSubscriptionId = subscription.Id,
                WeekStartDate = weekStart,
                DeadlinePassed = true,
                AutoSelectionApplied = false,
                RequiredMealCount = requiredMealCount,
                SelectedMealCount = 0,
                Message = "No recipes are eligible for auto-selection."
            };
        }

        var selections = selected.Select(candidate => new MealSelection
        {
            MealSubscriptionId = subscription.Id,
            WeeklyMenuId = weeklyMenu.Id,
            RecipeId = candidate.RecipeId,
            SelectedAutomatically = true,
            SelectedAt = now,
            AutoSelectionReason = reason,
            AutoSelectionNotes = string.IsNullOrWhiteSpace(message) ? null : message,
            CreatedAt = now
        }).ToList();

        _context.MealSelections.AddRange(selections);

        return new WeeklyMenuAutoSelectionResultDto
        {
            MealSubscriptionId = subscription.Id,
            WeekStartDate = weekStart,
            DeadlinePassed = true,
            AutoSelectionApplied = true,
            RequiredMealCount = requiredMealCount,
            SelectedMealCount = selections.Count,
            SelectedRecipeIds = selections.Select(x => x.RecipeId).ToList(),
            Message = string.IsNullOrWhiteSpace(message) ? "Auto-selection applied." : message
        };
    }

    private static List<RecipeCandidate> ApplyNutritionFilter(ICollection<RecipeCandidate> candidates, NutritionFilterDto? filter)
    {
        if (filter == null || !HasNutritionCriteria(filter))
        {
            return candidates.ToList();
        }

        return candidates
            .Where(candidate => MeetsNutritionFilter(candidate.Nutrition, filter))
            .ToList();
    }

    private static bool HasNutritionCriteria(NutritionFilterDto filter)
    {
        return filter.MinCaloriesKcal.HasValue || filter.MaxCaloriesKcal.HasValue
               || filter.MinProteinG.HasValue || filter.MaxProteinG.HasValue
               || filter.MinCarbsG.HasValue || filter.MaxCarbsG.HasValue
               || filter.MinFatG.HasValue || filter.MaxFatG.HasValue
               || filter.MinFiberG.HasValue || filter.MaxFiberG.HasValue
               || filter.MinSodiumMg.HasValue || filter.MaxSodiumMg.HasValue;
    }

    private static bool MeetsNutritionFilter(NutritionalInfo? nutrition, NutritionFilterDto filter)
    {
        if (nutrition == null)
        {
            return false;
        }

        return (!filter.MinCaloriesKcal.HasValue || nutrition.CaloriesKcal >= filter.MinCaloriesKcal.Value)
               && (!filter.MaxCaloriesKcal.HasValue || nutrition.CaloriesKcal <= filter.MaxCaloriesKcal.Value)
               && (!filter.MinProteinG.HasValue || nutrition.ProteinG >= filter.MinProteinG.Value)
               && (!filter.MaxProteinG.HasValue || nutrition.ProteinG <= filter.MaxProteinG.Value)
               && (!filter.MinCarbsG.HasValue || nutrition.CarbsG >= filter.MinCarbsG.Value)
               && (!filter.MaxCarbsG.HasValue || nutrition.CarbsG <= filter.MaxCarbsG.Value)
               && (!filter.MinFatG.HasValue || nutrition.FatG >= filter.MinFatG.Value)
               && (!filter.MaxFatG.HasValue || nutrition.FatG <= filter.MaxFatG.Value)
               && (!filter.MinFiberG.HasValue || nutrition.FiberG >= filter.MinFiberG.Value)
               && (!filter.MaxFiberG.HasValue || nutrition.FiberG <= filter.MaxFiberG.Value)
               && (!filter.MinSodiumMg.HasValue || nutrition.SodiumMg >= filter.MinSodiumMg.Value)
               && (!filter.MaxSodiumMg.HasValue || nutrition.SodiumMg <= filter.MaxSodiumMg.Value);
    }

    private sealed record RecipeCandidate
    {
        public Guid RecipeId { get; init; }
        public string RecipeName { get; init; } = string.Empty;
        public IReadOnlyCollection<Guid> DietaryCategoryIds { get; init; } = [];
        public IReadOnlyCollection<Guid> IngredientIds { get; init; } = [];
        public NutritionalInfo? Nutrition { get; init; }
    }
}

