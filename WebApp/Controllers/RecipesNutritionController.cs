using System.Security.Claims;
using App.Contracts.BLL.Menu;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.ViewModels.Menu;

namespace WebApp.Controllers;

[Authorize(Policy = "CompanyEmployee")]
public class RecipesNutritionController(
    IRecipeService recipeService,
    IIngredientService ingredientService,
    IDietaryCategoryService dietaryCategoryService,
    App.DAL.EF.AppDbContext dbContext,
    ILogger<RecipesNutritionController> logger) : Controller
{
    [HttpGet("/{slug}/recipes-nutrition")]
    public async Task<IActionResult> Index(
        string slug,
        string? search,
        Guid? dietaryCategoryId,
        Guid? ingredientId,
        bool activeOnly = true,
        Guid? selectedRecipeId = null,
        Guid? editingIngredientId = null,
        Guid? editingDietaryCategoryId = null)
    {
        if (!TryGetCompanyContext(slug, out var companyId))
        {
            return Forbid();
        }

        var model = await BuildViewModelAsync(companyId, slug, search, dietaryCategoryId, ingredientId, activeOnly, selectedRecipeId, editingIngredientId, editingDietaryCategoryId);
        return View(model);
    }

    [HttpPost("/{slug}/recipes-nutrition/recipe")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveRecipe(string slug, RecipesNutritionIndexViewModel model)
    {
        if (!TryGetCompanyContext(slug, out var companyId))
        {
            return Forbid();
        }

        if (!model.RecipeForm.RecipeId.HasValue)
        {
            model.SelectedRecipeId = null;
        }

        logger.LogInformation(
            "RecipesNutrition/SaveRecipe start: slug={Slug}, companyId={CompanyId}, recipeFormId={RecipeFormId}, selectedRecipeId={SelectedRecipeId}, name={Name}, servings={Servings}",
            slug,
            companyId,
            model.RecipeForm.RecipeId,
            model.SelectedRecipeId,
            model.RecipeForm.Name,
            model.RecipeForm.DefaultServings);

        try
        {
            var actorId = GetCurrentUserId();
            var saved = await recipeService.UpsertRecipeEditorAsync(companyId, actorId, new RecipeEditorUpsertDto
            {
                RecipeId = model.RecipeForm.RecipeId,
                Name = model.RecipeForm.Name,
                Description = model.RecipeForm.Description,
                DefaultServings = model.RecipeForm.DefaultServings,
                IsActive = model.RecipeForm.IsActive,
                IngredientIds = model.RecipeForm.SelectedIngredientIds,
                DietaryCategoryIds = model.RecipeForm.SelectedDietaryCategoryIds,
                Nutrition = new RecipeNutritionDto
                {
                    CaloriesKcal = model.RecipeForm.CaloriesKcal,
                    ProteinG = model.RecipeForm.ProteinG,
                    CarbsG = model.RecipeForm.CarbsG,
                    FatG = model.RecipeForm.FatG,
                    FiberG = model.RecipeForm.FiberG,
                    SodiumMg = model.RecipeForm.SodiumMg
                }
            });

            await dbContext.SaveChangesAsync();
            logger.LogInformation(
                "RecipesNutrition/SaveRecipe success: slug={Slug}, companyId={CompanyId}, savedRecipeId={SavedRecipeId}",
                slug,
                companyId,
                saved.RecipeId);
            TempData["SuccessMessage"] = "Recipe saved.";
            return RedirectToAction(nameof(Index), new { slug, selectedRecipeId = saved.RecipeId, model.Search, dietaryCategoryId = model.FilterDietaryCategoryId, ingredientId = model.FilterIngredientId, model.ActiveOnly });
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                ex,
                "RecipesNutrition/SaveRecipe failed: slug={Slug}, companyId={CompanyId}, recipeFormId={RecipeFormId}, selectedRecipeId={SelectedRecipeId}, name={Name}",
                slug,
                companyId,
                model.RecipeForm.RecipeId,
                model.SelectedRecipeId,
                model.RecipeForm.Name);
            var vm = await BuildViewModelAsync(companyId, slug, model.Search, model.FilterDietaryCategoryId, model.FilterIngredientId, model.ActiveOnly, model.RecipeForm.RecipeId, model.EditingIngredientId, model.EditingDietaryCategoryId);
            vm.RecipeForm = model.RecipeForm;
            ModelState.AddModelError(string.Empty, ex.Message);
            return View("Index", vm);
        }
    }

    [HttpPost("/{slug}/recipes-nutrition/ingredient")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveIngredient(string slug, RecipesNutritionIndexViewModel model)
    {
        if (!TryGetCompanyContext(slug, out var companyId))
        {
            return Forbid();
        }
        
        try
        {
            var actorId = GetCurrentUserId();
            var saved = await ingredientService.UpsertCatalogItemAsync(companyId, actorId, new IngredientCatalogUpsertDto
            {
                IngredientId = model.IngredientForm.IngredientId,
                Name = model.IngredientForm.Name,
                IsAllergen = model.IngredientForm.IsAllergen,
                IsExclusionTag = true
            });

            await dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Ingredient '{saved.Name}' saved.";
            return RedirectToAction(nameof(Index), new { slug, model.Search, dietaryCategoryId = model.FilterDietaryCategoryId, ingredientId = model.FilterIngredientId, model.ActiveOnly, model.SelectedRecipeId });
        }
        catch (Exception ex)
        {
            var vm = await BuildViewModelAsync(companyId, slug, model.Search, model.FilterDietaryCategoryId, model.FilterIngredientId, model.ActiveOnly, model.SelectedRecipeId, model.EditingIngredientId, model.EditingDietaryCategoryId);
            vm.IngredientForm = model.IngredientForm;
            ModelState.AddModelError(string.Empty, ex.Message);
            return View("Index", vm);
        }
    }

    [HttpPost("/{slug}/recipes-nutrition/ingredient/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteIngredient(
        string slug,
        Guid ingredientId,
        string? search,
        Guid? filterDietaryCategoryId,
        Guid? filterIngredientId,
        bool activeOnly = true,
        Guid? selectedRecipeId = null,
        Guid? editingDietaryCategoryId = null)
    {
        if (!TryGetCompanyContext(slug, out var companyId))
        {
            return Forbid();
        }

        try
        {
            await ingredientService.RemoveCatalogItemAsync(companyId, ingredientId);
            await dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Ingredient deleted.";
            return RedirectToAction(nameof(Index), new { slug, search, dietaryCategoryId = filterDietaryCategoryId, ingredientId = filterIngredientId, activeOnly, selectedRecipeId, editingDietaryCategoryId });
        }
        catch (Exception ex)
        {
            var vm = await BuildViewModelAsync(companyId, slug, search, filterDietaryCategoryId, filterIngredientId, activeOnly, selectedRecipeId, null, editingDietaryCategoryId);
            ModelState.AddModelError(string.Empty, ex.Message);
            return View("Index", vm);
        }
    }

    [HttpPost("/{slug}/recipes-nutrition/dietary-category")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveDietaryCategory(string slug, RecipesNutritionIndexViewModel model)
    {
        if (!TryGetCompanyContext(slug, out var companyId))
        {
            return Forbid();
        }

        try
        {
            var actorId = GetCurrentUserId();
            var saved = await dietaryCategoryService.UpsertCatalogItemAsync(companyId, actorId, new DietaryCategoryCatalogUpsertDto
            {
                DietaryCategoryId = model.DietaryCategoryForm.DietaryCategoryId,
                Code = model.DietaryCategoryForm.Name,
                Name = model.DietaryCategoryForm.Name,
                IsActive = model.DietaryCategoryForm.IsActive
            });

            await dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Dietary category '{saved.Name}' saved.";
            return RedirectToAction(nameof(Index), new { slug, model.Search, dietaryCategoryId = model.FilterDietaryCategoryId, ingredientId = model.FilterIngredientId, model.ActiveOnly, model.SelectedRecipeId, editingDietaryCategoryId = saved.DietaryCategoryId });
        }
        catch (Exception ex)
        {
            var vm = await BuildViewModelAsync(companyId, slug, model.Search, model.FilterDietaryCategoryId, model.FilterIngredientId, model.ActiveOnly, model.SelectedRecipeId, model.EditingIngredientId, model.EditingDietaryCategoryId);
            vm.DietaryCategoryForm = model.DietaryCategoryForm;
            ModelState.AddModelError(string.Empty, ex.Message);
            return View("Index", vm);
        }
    }

    [HttpPost("/{slug}/recipes-nutrition/dietary-category/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteDietaryCategory(
        string slug,
        Guid dietaryCategoryId,
        string? search,
        Guid? filterDietaryCategoryId,
        Guid? filterIngredientId,
        bool activeOnly = true,
        Guid? selectedRecipeId = null,
        Guid? editingIngredientId = null)
    {
        if (!TryGetCompanyContext(slug, out var companyId))
        {
            return Forbid();
        }

        try
        {
            await dietaryCategoryService.RemoveCatalogItemAsync(companyId, dietaryCategoryId);
            await dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Dietary category deleted.";
            return RedirectToAction(nameof(Index), new
            {
                slug,
                search,
                dietaryCategoryId = filterDietaryCategoryId,
                ingredientId = filterIngredientId,
                activeOnly,
                selectedRecipeId,
                editingIngredientId
            });
        }
        catch (Exception ex)
        {
            var vm = await BuildViewModelAsync(companyId, slug, search, filterDietaryCategoryId, filterIngredientId, activeOnly, selectedRecipeId, editingIngredientId, null);
            ModelState.AddModelError(string.Empty, ex.Message);
            return View("Index", vm);
        }
    }

    [HttpPost("/{slug}/recipes-nutrition/recipe/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteRecipe(
        string slug,
        Guid recipeId,
        string? search,
        Guid? filterDietaryCategoryId,
        Guid? filterIngredientId,
        bool activeOnly = true,
        Guid? editingIngredientId = null,
        Guid? editingDietaryCategoryId = null)
    {
        if (!TryGetCompanyContext(slug, out var companyId))
        {
            return Forbid();
        }

        try
        {
            await recipeService.RemoveRecipeAsync(companyId, recipeId);
            await dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Recipe deleted.";
            return RedirectToAction(nameof(Index), new
            {
                slug,
                search,
                dietaryCategoryId = filterDietaryCategoryId,
                ingredientId = filterIngredientId,
                activeOnly,
                editingIngredientId,
                editingDietaryCategoryId
            });
        }
        catch (Exception ex)
        {
            var vm = await BuildViewModelAsync(companyId, slug, search, filterDietaryCategoryId, filterIngredientId, activeOnly, recipeId, editingIngredientId, editingDietaryCategoryId);
            ModelState.AddModelError(string.Empty, ex.Message);
            return View("Index", vm);
        }
    }

    [HttpGet("/{slug}/recipes-nutrition/messages")]
    public IActionResult Messages(string slug)
    {
        if (!TryGetCompanyContext(slug, out _))
        {
            return Forbid();
        }

        return PartialView("_PageMessages");
    }

    private async Task<RecipesNutritionIndexViewModel> BuildViewModelAsync(
        Guid companyId,
        string slug,
        string? search,
        Guid? dietaryCategoryId,
        Guid? ingredientId,
        bool activeOnly,
        Guid? selectedRecipeId,
        Guid? editingIngredientId,
        Guid? editingDietaryCategoryId)
    {
        var categories = await dietaryCategoryService.GetCatalogAsync(companyId);
        var ingredients = await ingredientService.GetCatalogAsync(companyId);
        var recipes = await recipeService.GetRecipeListAsync(companyId, new RecipeListFilterDto
        {
            Search = search,
            DietaryCategoryId = dietaryCategoryId,
            IngredientId = ingredientId,
            ActiveOnly = activeOnly
        });

        var selected = selectedRecipeId.HasValue
            ? await recipeService.GetRecipeEditorAsync(companyId, selectedRecipeId.Value)
            : null;

        var editingIngredient = editingIngredientId.HasValue
            ? ingredients.FirstOrDefault(x => x.IngredientId == editingIngredientId.Value)
            : null;

        var editingDietaryCategory = editingDietaryCategoryId.HasValue
            ? categories.FirstOrDefault(x => x.DietaryCategoryId == editingDietaryCategoryId.Value)
            : null;

        return new RecipesNutritionIndexViewModel
        {
            CompanyId = companyId,
            CompanySlug = slug,
            Search = search,
            FilterDietaryCategoryId = dietaryCategoryId,
            FilterIngredientId = ingredientId,
            ActiveOnly = activeOnly,
            SelectedRecipeId = selectedRecipeId,
            EditingIngredientId = editingIngredientId,
            EditingDietaryCategoryId = editingDietaryCategoryId,
            Recipes = recipes.ToList(),
            IngredientCatalog = ingredients.ToList(),
            DietaryCategoryCatalog = categories.ToList(),
            IngredientOptions = ingredients
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem(x.Name, x.IngredientId.ToString()))
                .ToList(),
            DietaryCategoryOptions = categories
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem(x.Name, x.DietaryCategoryId.ToString()))
                .ToList(),
            RecipeForm = selected == null
                ? new RecipeEditorFormViewModel()
                : new RecipeEditorFormViewModel
                {
                    RecipeId = selected.RecipeId,
                    Name = selected.Name,
                    Description = selected.Description,
                    DefaultServings = selected.DefaultServings,
                    IsActive = selected.IsActive,
                    SelectedIngredientIds = selected.IngredientIds.ToList(),
                    SelectedDietaryCategoryIds = selected.DietaryCategoryIds.ToList(),
                    CaloriesKcal = selected.Nutrition?.CaloriesKcal ?? 0,
                    ProteinG = selected.Nutrition?.ProteinG ?? 0,
                    CarbsG = selected.Nutrition?.CarbsG ?? 0,
                    FatG = selected.Nutrition?.FatG ?? 0,
                    FiberG = selected.Nutrition?.FiberG ?? 0,
                    SodiumMg = selected.Nutrition?.SodiumMg ?? 0
                },
            IngredientForm = editingIngredient == null
                ? new IngredientCatalogFormViewModel()
                : new IngredientCatalogFormViewModel
                {
                    IngredientId = editingIngredient.IngredientId,
                    Name = editingIngredient.Name,
                    IsAllergen = editingIngredient.IsAllergen
                },
            DietaryCategoryForm = editingDietaryCategory == null
                ? new DietaryCategoryCatalogFormViewModel()
                : new DietaryCategoryCatalogFormViewModel
                {
                    DietaryCategoryId = editingDietaryCategory.DietaryCategoryId,
                    Name = editingDietaryCategory.Name,
                    IsActive = editingDietaryCategory.IsActive
                }
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
}
