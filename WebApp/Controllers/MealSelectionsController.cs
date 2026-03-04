using App.Contracts.BLL.Menu;
using App.Contracts.BLL.Core;
using App.Contracts.BLL.Subscription;
using App.Domain.Menu;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.ViewModels.MealSelections;

namespace WebApp.Controllers
{
    [Authorize(Roles = "user")]
    public class MealSelectionsController : Controller
    {
        private readonly IMealSelectionService _mealSelectionService;
        private readonly IMealSubscriptionService _mealSubscriptionService;
        private readonly IWeeklyMenuService _weeklyMenuService;
        private readonly IRecipeService _recipeService;
        private readonly ICustomerPreferenceService _customerPreferenceService;
        private readonly ICustomerExclusionService _customerExclusionService;
        private readonly ICompanySettingsService _companySettingsService;

        public MealSelectionsController(
            IMealSelectionService mealSelectionService,
            IMealSubscriptionService mealSubscriptionService,
            IWeeklyMenuService weeklyMenuService,
            IRecipeService recipeService,
            ICustomerPreferenceService customerPreferenceService,
            ICustomerExclusionService customerExclusionService,
            ICompanySettingsService companySettingsService)
        {
            _mealSelectionService = mealSelectionService;
            _mealSubscriptionService = mealSubscriptionService;
            _weeklyMenuService = weeklyMenuService;
            _recipeService = recipeService;
            _customerPreferenceService = customerPreferenceService;
            _customerExclusionService = customerExclusionService;
            _companySettingsService = companySettingsService;
        }

        // GET: MealSelections/Recommend
        [HttpGet]
        public async Task<IActionResult> Recommend()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            var model = await BuildRecommendationViewModelAsync(new MealSelectionRecommendationViewModel(), companyId.Value);
            return View(model);
        }

        // POST: MealSelections/Recommend
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Recommend(MealSelectionRecommendationViewModel viewModel)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return View(await BuildRecommendationViewModelAsync(viewModel, companyId.Value));
            }

            var mealSubscription = await _mealSubscriptionService.GetByIdAsync(viewModel.MealSubscriptionId, companyId.Value);
            if (mealSubscription == null)
            {
                ModelState.AddModelError(string.Empty, "Meal subscription was not found in company scope.");
                return View(await BuildRecommendationViewModelAsync(viewModel, companyId.Value));
            }

            var weeklyMenu = await _weeklyMenuService.GetByIdAsync(viewModel.WeeklyMenuId, companyId.Value);
            if (weeklyMenu == null)
            {
                ModelState.AddModelError(string.Empty, "Weekly menu was not found in company scope.");
                return View(await BuildRecommendationViewModelAsync(viewModel, companyId.Value));
            }

            var settings = await _companySettingsService.GetByCompanyIdAsync(companyId.Value);
            if (settings?.AllowAutoSelection == false)
            {
                ModelState.AddModelError(string.Empty, "Automatic selection is disabled by company settings.");
                return View(await BuildRecommendationViewModelAsync(viewModel, companyId.Value));
            }

            var allRecipes = await _recipeService.GetAllByCompanyIdAsync(companyId.Value);
            var preferences = await _customerPreferenceService.GetAllByCompanyIdAsync(companyId.Value);
            var exclusions = await _customerExclusionService.GetAllByCompanyIdAsync(companyId.Value);
            var previousSelections = await _mealSelectionService.GetAllByMealSubscriptionIdAsync(viewModel.MealSubscriptionId);

            var customerPreferences = preferences.Where(p => p.CustomerId == mealSubscription.CustomerId).ToList();
            var customerExclusions = exclusions.Where(e => e.CustomerId == mealSubscription.CustomerId).ToList();

            var noRepeatWeeks = settings?.DefaultNoRepeatWeeks ?? 0;
            var nonRepeatBoundary = weeklyMenu.WeekStartDate.AddDays(-(7 * Math.Max(0, noRepeatWeeks)));
            var recentRecipeIds = previousSelections
                .Where(s => s.SelectedAt >= nonRepeatBoundary)
                .Select(s => s.RecipeId)
                .ToHashSet();

            var filtered = allRecipes
                .Where(r => r.IsActive)
                .Where(r => !recentRecipeIds.Contains(r.Id))
                .Where(r => MeetsNutritionFilter(r, viewModel.MaxCaloriesKcal, viewModel.MinProteinG))
                .Where(r => MatchesPreferenceFilter(r, customerPreferences))
                .Where(r => PassesExclusionFilter(r, customerExclusions))
                .ToList();

            if (filtered.Count == 0)
            {
                viewModel.RecommendationSummary = "No recipes matched the fallback filters. Consider relaxing nutrition targets or non-repetition window constraints.";
                viewModel.DecisionNotes = [
                    $"No-repeat weeks applied: {noRepeatWeeks}",
                    customerPreferences.Count > 0
                        ? "Dietary preference filtering was applied."
                        : "No dietary preferences were found for this customer.",
                    customerExclusions.Count > 0
                        ? "Ingredient exclusion filtering was applied."
                        : "No ingredient exclusions were found for this customer."
                ];

                return View(await BuildRecommendationViewModelAsync(viewModel, companyId.Value));
            }

            var recommended = filtered
                .OrderByDescending(r => r.NutritionalInfo?.ProteinG ?? 0)
                .ThenBy(r => r.NutritionalInfo?.CaloriesKcal ?? decimal.MaxValue)
                .ThenBy(r => r.Name)
                .First();

            viewModel.RecommendedRecipeId = recommended.Id;
            viewModel.RecommendationSummary = $"Recommended fallback recipe: {recommended.Name}.";
            viewModel.DecisionNotes = [
                $"No-repeat weeks applied: {noRepeatWeeks}",
                $"Nutrition filter: max kcal {(viewModel.MaxCaloriesKcal?.ToString() ?? "(none)")}, min protein {(viewModel.MinProteinG?.ToString() ?? "(none)")}.",
                $"Candidate recipes after filtering: {filtered.Count}"
            ];

            return View(await BuildRecommendationViewModelAsync(viewModel, companyId.Value));
        }

        // GET: MealSelections
        public async Task<IActionResult> Index()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            return View(await _mealSelectionService.GetAllByCompanyIdAsync(companyId.Value));
        }

        // GET: MealSelections/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            var mealSelection = await _mealSelectionService.GetByIdAsync(id.Value, companyId.Value);
            if (mealSelection == null)
            {
                return NotFound();
            }

            return View(mealSelection);
        }

        // GET: MealSelections/Create
        public async Task<IActionResult> Create()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            return View(await BuildEditViewModelAsync(new MealSelection(), companyId.Value));
        }

        // POST: MealSelections/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MealSelectionEditViewModel viewModel)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            var mealSelection = viewModel.MealSelection;
            if (ModelState.IsValid)
            {
                mealSelection.CreatedAt = DateTime.UtcNow;
                mealSelection.UpdatedAt = null;
                mealSelection.DeletedAt = null;
                await _mealSelectionService.AddAsync(mealSelection, companyId.Value);
                return RedirectToAction(nameof(Index));
            }

            return View(await BuildEditViewModelAsync(mealSelection, companyId.Value));
        }

        // GET: MealSelections/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            var mealSelection = await _mealSelectionService.GetByIdAsync(id.Value, companyId.Value);
            if (mealSelection == null)
            {
                return NotFound();
            }

            return View(await BuildEditViewModelAsync(mealSelection, companyId.Value));
        }

        // POST: MealSelections/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, MealSelectionEditViewModel viewModel)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            var mealSelection = viewModel.MealSelection;
            if (id != mealSelection.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existing = await _mealSelectionService.GetByIdAsync(id, companyId.Value);
                if (existing == null)
                {
                    return NotFound();
                }

                mealSelection.CreatedAt = existing.CreatedAt;
                mealSelection.UpdatedAt = DateTime.UtcNow;
                mealSelection.DeletedAt = existing.DeletedAt;
                await _mealSelectionService.UpdateAsync(mealSelection, companyId.Value);
                return RedirectToAction(nameof(Index));
            }

            return View(await BuildEditViewModelAsync(mealSelection, companyId.Value));
        }

        // GET: MealSelections/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            var mealSelection = await _mealSelectionService.GetByIdAsync(id.Value, companyId.Value);
            if (mealSelection == null)
            {
                return NotFound();
            }

            return View(mealSelection);
        }

        // POST: MealSelections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            await _mealSelectionService.RemoveAsync(id, companyId.Value);
            return RedirectToAction(nameof(Index));
        }

        private async Task<MealSelectionEditViewModel> BuildEditViewModelAsync(MealSelection mealSelection, Guid companyId)
        {
            var mealSubscriptions = await _mealSubscriptionService.GetAllByCompanyIdAsync(companyId);
            var weeklyMenus = await _weeklyMenuService.GetAllByCompanyIdAsync(companyId);
            var recipes = await _recipeService.GetAllByCompanyIdAsync(companyId);

            return new MealSelectionEditViewModel
            {
                MealSelection = mealSelection,
                MealSubscriptionOptions = mealSubscriptions
                    .Select(ms => new SelectListItem(ms.Id.ToString(), ms.Id.ToString(), ms.Id == mealSelection.MealSubscriptionId))
                    .ToList(),
                WeeklyMenuOptions = weeklyMenus
                    .Select(wm => new SelectListItem(wm.WeekStartDate.ToString("yyyy-MM-dd"), wm.Id.ToString(), wm.Id == mealSelection.WeeklyMenuId))
                    .ToList(),
                RecipeOptions = recipes
                    .Select(r => new SelectListItem(r.Name, r.Id.ToString(), r.Id == mealSelection.RecipeId))
                    .ToList()
            };
        }

        private Guid? GetCurrentCompanyId()
        {
            var companyIdRaw = User.FindFirst("company_id")?.Value
                               ?? User.FindFirst("tenant_id")?.Value
                               ?? User.FindFirst("companyId")?.Value;

            return Guid.TryParse(companyIdRaw, out var companyId)
                ? companyId
                : null;
        }

        private static bool MeetsNutritionFilter(Recipe recipe, decimal? maxCaloriesKcal, decimal? minProteinG)
        {
            if (maxCaloriesKcal == null && minProteinG == null)
            {
                return true;
            }

            if (recipe.NutritionalInfo == null)
            {
                return false;
            }

            if (maxCaloriesKcal.HasValue && recipe.NutritionalInfo.CaloriesKcal > maxCaloriesKcal.Value)
            {
                return false;
            }

            if (minProteinG.HasValue && recipe.NutritionalInfo.ProteinG < minProteinG.Value)
            {
                return false;
            }

            return true;
        }

        private static bool MatchesPreferenceFilter(Recipe recipe, IReadOnlyCollection<CustomerPreference> preferences)
        {
            if (preferences.Count == 0)
            {
                return true;
            }

            if (recipe.RecipeDietaryCategories == null)
            {
                return false;
            }

            var recipeCategoryIds = recipe.RecipeDietaryCategories.Select(c => c.DietaryCategoryId).ToHashSet();
            return preferences.Any(p => recipeCategoryIds.Contains(p.DietaryCategoryId));
        }

        private static bool PassesExclusionFilter(Recipe recipe, IReadOnlyCollection<CustomerExclusion> exclusions)
        {
            if (exclusions.Count == 0)
            {
                return true;
            }

            if (recipe.RecipeIngredients == null)
            {
                return true;
            }

            var excludedIngredientIds = exclusions.Select(e => e.IngredientId).ToHashSet();
            return recipe.RecipeIngredients.All(ri => !excludedIngredientIds.Contains(ri.IngredientId));
        }

        private async Task<MealSelectionRecommendationViewModel> BuildRecommendationViewModelAsync(
            MealSelectionRecommendationViewModel viewModel,
            Guid companyId)
        {
            var mealSubscriptions = await _mealSubscriptionService.GetAllByCompanyIdAsync(companyId);
            var weeklyMenus = await _weeklyMenuService.GetAllByCompanyIdAsync(companyId);
            var recipes = await _recipeService.GetAllByCompanyIdAsync(companyId);

            viewModel.MealSubscriptionOptions = mealSubscriptions
                .Select(ms => new SelectListItem(ms.Id.ToString(), ms.Id.ToString(), ms.Id == viewModel.MealSubscriptionId))
                .ToList();

            viewModel.WeeklyMenuOptions = weeklyMenus
                .Select(wm => new SelectListItem(
                    $"{wm.WeekStartDate:yyyy-MM-dd} (deadline {wm.SelectionDeadlineAt:yyyy-MM-dd HH:mm})",
                    wm.Id.ToString(),
                    wm.Id == viewModel.WeeklyMenuId))
                .ToList();

            viewModel.RecommendedRecipeOptions = recipes
                .Where(r => r.IsActive)
                .Select(r => new SelectListItem(r.Name, r.Id.ToString(), r.Id == viewModel.RecommendedRecipeId))
                .ToList();

            return viewModel;
        }
    }
}
