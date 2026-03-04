using App.Contracts.BLL.Menu;
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

        public MealSelectionsController(
            IMealSelectionService mealSelectionService,
            IMealSubscriptionService mealSubscriptionService,
            IWeeklyMenuService weeklyMenuService,
            IRecipeService recipeService)
        {
            _mealSelectionService = mealSelectionService;
            _mealSubscriptionService = mealSubscriptionService;
            _weeklyMenuService = weeklyMenuService;
            _recipeService = recipeService;
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
    }
}
