using System.Security.Claims;
using App.Contracts.BLL.Menu;
using App.Contracts.BLL.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.ViewModels.Menu;

namespace WebApp.Controllers;

[Authorize(Policy = "CompanyEmployee")]
public class MenusSchedulingController(
    IWeeklyMenuService weeklyMenuService,
    IRecipeService recipeService,
    IDietaryCategoryService dietaryCategoryService,
    App.DAL.EF.AppDbContext dbContext,
    ILogger<MenusSchedulingController> logger) : Controller
{
    [HttpGet("/{slug}/menus-scheduling")]
    public async Task<IActionResult> Index(string slug, DateTime? weekStartDate)
    {
        if (!TryGetCompanyContext(slug, out var companyId))
        {
            return Forbid();
        }

        var week = NormalizeWeekStart(weekStartDate ?? DateTime.UtcNow.Date);
        var model = await BuildViewModelAsync(companyId, slug, week);
        return View(model);
    }

    [HttpPost("/{slug}/menus-scheduling/config")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveConfig(string slug, MenusSchedulingIndexViewModel model)
    {
        if (!TryGetCompanyContext(slug, out var companyId))
        {
            return Forbid();
        }

        await weeklyMenuService.SaveRuleConfigAsync(companyId, model.RuleConfigForm);
        await dbContext.SaveChangesAsync();
        TempData["SuccessMessage"] = "Menu configuration saved.";
        return RedirectToAction(nameof(Index), new { slug, weekStartDate = NormalizeWeekStart(model.WeekStartDate).ToString("yyyy-MM-dd") });
    }

    [HttpPost("/{slug}/menus-scheduling/assign")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignRecipe(string slug, MenusSchedulingIndexViewModel model)
    {
        if (!TryGetCompanyContext(slug, out var companyId))
        {
            return Forbid();
        }

        logger.LogInformation(
            "MenusScheduling/AssignRecipe start: slug={Slug}, companyId={CompanyId}, week={Week}, recipeId={RecipeId}, categoryId={CategoryId}",
            slug,
            companyId,
            model.WeekStartDate,
            model.AssignmentForm.RecipeId,
            model.AssignmentForm.DietaryCategoryId);

        var actorId = GetCurrentUserId();
        var request = new WeeklyMenuAssignmentCreateDto
        {
            WeekStartDate = NormalizeWeekStart(model.WeekStartDate),
            RecipeId = model.AssignmentForm.RecipeId,
            DietaryCategoryId = model.AssignmentForm.DietaryCategoryId,
            CreatedByAppUserId = actorId
        };

        var result = await weeklyMenuService.AssignRecipeToWeekAsync(companyId, request);
        await dbContext.SaveChangesAsync();
        var latestAssignments = await weeklyMenuService.GetWeeklyAssignmentsAsync(companyId, request.WeekStartDate);

        logger.LogInformation(
            "MenusScheduling/AssignRecipe result: success={Success}, message={Message}, assignmentId={AssignmentId}, postQueryAssignments={PostQueryAssignments}",
            result.Success,
            result.Message,
            result.Assignment?.WeeklyMenuRecipeId,
            latestAssignments.Count);

        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.Message;
        }
        else
        {
            TempData["SuccessMessage"] = result.Message;
        }

        return RedirectToAction(nameof(Index), new { slug, weekStartDate = request.WeekStartDate.ToString("yyyy-MM-dd") });
    }

    [HttpPost("/{slug}/menus-scheduling/simulate")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Simulate(string slug, MenusSchedulingIndexViewModel model)
    {
        if (!TryGetCompanyContext(slug, out var companyId))
        {
            return Forbid();
        }

        var week = NormalizeWeekStart(model.WeekStartDate);
        var viewModel = await BuildViewModelAsync(companyId, slug, week);
        viewModel.Simulation = await weeklyMenuService.SimulateAutoSelectionAsync(companyId, new WeeklyMenuSimulationRequestDto
        {
            WeekStartDate = week
        });

        return View(nameof(Index), viewModel);
    }

    private async Task<MenusSchedulingIndexViewModel> BuildViewModelAsync(Guid companyId, string slug, DateTime weekStartDate)
    {
        var categories = await dietaryCategoryService.GetAllByCompanyIdAsync(companyId);
        var recipes = await recipeService.GetAllByCompanyIdAsync(companyId);
        var config = await weeklyMenuService.GetRuleConfigAsync(companyId);
        var assignments = await weeklyMenuService.GetWeeklyAssignmentsAsync(companyId, weekStartDate);

        return new MenusSchedulingIndexViewModel
        {
            CompanyId = companyId,
            CompanySlug = slug,
            WeekStartDate = weekStartDate,
            RuleConfig = config,
            RuleConfigForm = new WeeklyMenuRuleConfigUpdateDto
            {
                RecipesPerCategory = config.RecipesPerCategory,
                NoRepeatWeeks = config.NoRepeatWeeks,
                SelectionDeadlineDaysBeforeWeekStart = config.SelectionDeadlineDaysBeforeWeekStart
            },
            AssignmentForm = new WeeklyMenuAssignmentCreateDto
            {
                WeekStartDate = weekStartDate
            },
            CategoryOptions = categories
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
                .ToList(),
            RecipeOptions = recipes
                .Where(x => x.IsActive && x.DeletedAt == null)
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
                .ToList(),
            Assignments = assignments.ToList()
        };
    }

    private bool TryGetCompanyContext(string slug, out Guid companyId)
    {
        companyId = Guid.Empty;

        var companyIdRaw = User.FindFirstValue("company_id")
                           ?? User.FindFirstValue("tenant_id")
                           ?? User.FindFirstValue("companyId");
        var currentSlug = User.FindFirstValue("company_slug")
                          ?? User.FindFirstValue("tenant_slug");

        return Guid.TryParse(companyIdRaw, out companyId)
               && !string.IsNullOrWhiteSpace(currentSlug)
               && string.Equals(currentSlug, slug, StringComparison.OrdinalIgnoreCase);
    }

    private Guid GetCurrentUserId()
    {
        var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? User.FindFirstValue("sub")
                        ?? User.FindFirstValue("user_id");

        if (!Guid.TryParse(userIdRaw, out var userId))
        {
            throw new UnauthorizedAccessException("Unable to resolve current user id.");
        }

        return userId;
    }

    private static DateTime NormalizeWeekStart(DateTime value)
    {
        var day = value.Date;
        while (day.DayOfWeek != DayOfWeek.Monday)
        {
            day = day.AddDays(-1);
        }

        return day;
    }
}
