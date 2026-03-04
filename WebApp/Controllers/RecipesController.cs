using App.Contracts.BLL.Menu;
using App.DAL.EF;
using App.Domain.Menu;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApp.ViewModels.Recipes;

namespace WebApp.Controllers
{
    [Authorize(Policy = "CompanyManager")]
    public class RecipesController : Controller
    {
        private readonly IRecipeService _recipeService;
        private readonly AppDbContext _appDbContext;

        public RecipesController(IRecipeService recipeService, AppDbContext appDbContext)
        {
            _recipeService = recipeService;
            _appDbContext = appDbContext;
        }

        // GET: Recipes
        public async Task<IActionResult> Index()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            return View(await _recipeService.GetAllByCompanyIdAsync(companyId.Value));
        }

        // GET: Recipes/Details/5
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

            var recipe = await _recipeService.GetByIdAsync(id.Value, companyId.Value);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        // GET: Recipes/Nutrition/5
        [Authorize(Policy = "CompanyManager")]
        public async Task<IActionResult> Nutrition(Guid? id)
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

            var recipe = await _recipeService.GetByIdAsync(id.Value, companyId.Value);
            if (recipe == null)
            {
                return NotFound();
            }

            var nutritionalInfo = recipe.NutritionalInfo;

            return View(new RecipeNutritionEditViewModel
            {
                RecipeId = recipe.Id,
                RecipeName = recipe.Name,
                CaloriesKcal = nutritionalInfo?.CaloriesKcal ?? 0,
                ProteinG = nutritionalInfo?.ProteinG ?? 0,
                CarbsG = nutritionalInfo?.CarbsG ?? 0,
                FatG = nutritionalInfo?.FatG ?? 0,
                FiberG = nutritionalInfo?.FiberG ?? 0,
                SodiumMg = nutritionalInfo?.SodiumMg ?? 0,
                SugarG = nutritionalInfo?.SugarG ?? 0,
                SaturatedFatG = nutritionalInfo?.SaturatedFatG ?? 0
            });
        }

        // POST: Recipes/Nutrition/5
        [HttpPost]
        [Authorize(Policy = "CompanyManager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Nutrition(Guid id, RecipeNutritionEditViewModel viewModel)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            if (id != viewModel.RecipeId)
            {
                return NotFound();
            }

            var recipe = await _recipeService.GetByIdAsync(id, companyId.Value);
            if (recipe == null)
            {
                return NotFound();
            }

            viewModel.RecipeName = recipe.Name;

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var nutritionalInfo = await _appDbContext.NutritionalInfos
                .FirstOrDefaultAsync(n => n.RecipeId == id);

            if (nutritionalInfo == null)
            {
                nutritionalInfo = new NutritionalInfo
                {
                    RecipeId = id,
                    CreatedAt = DateTime.UtcNow
                };

                _appDbContext.NutritionalInfos.Add(nutritionalInfo);
            }
            else
            {
                nutritionalInfo.UpdatedAt = DateTime.UtcNow;
            }

            nutritionalInfo.CaloriesKcal = viewModel.CaloriesKcal;
            nutritionalInfo.ProteinG = viewModel.ProteinG;
            nutritionalInfo.CarbsG = viewModel.CarbsG;
            nutritionalInfo.FatG = viewModel.FatG;
            nutritionalInfo.FiberG = viewModel.FiberG;
            nutritionalInfo.SodiumMg = viewModel.SodiumMg;
            nutritionalInfo.SugarG = viewModel.SugarG;
            nutritionalInfo.SaturatedFatG = viewModel.SaturatedFatG;

            await _appDbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Nutritional data updated.";

            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Recipes/Create
        public IActionResult Create()
        {
            return View(new RecipeEditViewModel
            {
                Recipe = new Recipe { IsActive = true }
            });
        }

        // POST: Recipes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RecipeEditViewModel viewModel)
        {
            var companyId = GetCurrentCompanyId();
            var userId = GetCurrentUserId();
            if (companyId == null || userId == null)
            {
                return Forbid();
            }

            if (viewModel.Recipe == null)
            {
                return BadRequest();
            }

            var recipe = viewModel.Recipe;
            if (ModelState.IsValid)
            {
                recipe.CreatedAt = DateTime.UtcNow;
                recipe.UpdatedAt = null;
                recipe.DeletedAt = null;
                recipe.CompanyId = companyId.Value;
                recipe.CreatedByAppUserId = userId.Value;

                await _recipeService.AddAsync(recipe, companyId.Value);
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        // GET: Recipes/Edit/5
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

            var recipe = await _recipeService.GetByIdAsync(id.Value, companyId.Value);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(new RecipeEditViewModel { Recipe = recipe });
        }

        // POST: Recipes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, RecipeEditViewModel viewModel)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            if (viewModel.Recipe == null)
            {
                return BadRequest();
            }

            var recipe = viewModel.Recipe;
            if (id != recipe.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existing = await _recipeService.GetByIdAsync(id, companyId.Value);
                if (existing == null)
                {
                    return NotFound();
                }

                recipe.CompanyId = companyId.Value;
                recipe.CreatedByAppUserId = existing.CreatedByAppUserId;
                recipe.CreatedAt = existing.CreatedAt;
                recipe.DeletedAt = existing.DeletedAt;
                recipe.UpdatedAt = DateTime.UtcNow;

                await _recipeService.UpdateAsync(recipe, companyId.Value);
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        // GET: Recipes/Delete/5
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

            var recipe = await _recipeService.GetByIdAsync(id.Value, companyId.Value);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        // POST: Recipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            await _recipeService.RemoveAsync(id, companyId.Value);
            return RedirectToAction(nameof(Index));
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

        private Guid? GetCurrentUserId()
        {
            var userIdRaw = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                            ?? User.FindFirst("sub")?.Value
                            ?? User.FindFirst("user_id")?.Value;

            return Guid.TryParse(userIdRaw, out var userId)
                ? userId
                : null;
        }
    }
}
