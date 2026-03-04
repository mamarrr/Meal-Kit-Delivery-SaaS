using App.Contracts.BLL.Menu;
using App.Domain.Menu;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApp.ViewModels.DietaryCategories;

namespace WebApp.Controllers
{
    [Authorize(Roles = "user")]
    public class DietaryCategoriesController : Controller
    {
        private readonly IDietaryCategoryService _dietaryCategoryService;

        public DietaryCategoriesController(IDietaryCategoryService dietaryCategoryService)
        {
            _dietaryCategoryService = dietaryCategoryService;
        }

        // GET: DietaryCategories
        public async Task<IActionResult> Index()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            return View(await _dietaryCategoryService.GetAllByCompanyIdAsync(companyId.Value));
        }

        // GET: DietaryCategories/Details/5
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

            var dietaryCategory = await _dietaryCategoryService.GetByIdAsync(id.Value, companyId.Value);
            if (dietaryCategory == null)
            {
                return NotFound();
            }

            return View(dietaryCategory);
        }

        // GET: DietaryCategories/Create
        public IActionResult Create()
        {
            return View(new DietaryCategoryEditViewModel
            {
                DietaryCategory = new DietaryCategory { IsActive = true }
            });
        }

        // POST: DietaryCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DietaryCategoryEditViewModel viewModel)
        {
            var companyId = GetCurrentCompanyId();
            var userId = GetCurrentUserId();
            if (companyId == null || userId == null)
            {
                return Forbid();
            }

            if (viewModel.DietaryCategory == null)
            {
                return BadRequest();
            }

            var dietaryCategory = viewModel.DietaryCategory;
            if (ModelState.IsValid)
            {
                dietaryCategory.CreatedAt = DateTime.UtcNow;
                dietaryCategory.UpdatedAt = null;
                dietaryCategory.DeletedAt = null;
                dietaryCategory.CompanyId = companyId.Value;
                dietaryCategory.CreatedByAppUserId = userId.Value;

                await _dietaryCategoryService.AddAsync(dietaryCategory, companyId.Value);
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        // GET: DietaryCategories/Edit/5
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

            var dietaryCategory = await _dietaryCategoryService.GetByIdAsync(id.Value, companyId.Value);
            if (dietaryCategory == null)
            {
                return NotFound();
            }

            return View(new DietaryCategoryEditViewModel { DietaryCategory = dietaryCategory });
        }

        // POST: DietaryCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, DietaryCategoryEditViewModel viewModel)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            if (viewModel.DietaryCategory == null)
            {
                return BadRequest();
            }

            var dietaryCategory = viewModel.DietaryCategory;
            if (id != dietaryCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existing = await _dietaryCategoryService.GetByIdAsync(id, companyId.Value);
                if (existing == null)
                {
                    return NotFound();
                }

                dietaryCategory.CompanyId = companyId.Value;
                dietaryCategory.CreatedByAppUserId = existing.CreatedByAppUserId;
                dietaryCategory.CreatedAt = existing.CreatedAt;
                dietaryCategory.DeletedAt = existing.DeletedAt;
                dietaryCategory.UpdatedAt = DateTime.UtcNow;

                await _dietaryCategoryService.UpdateAsync(dietaryCategory, companyId.Value);
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        // GET: DietaryCategories/Delete/5
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

            var dietaryCategory = await _dietaryCategoryService.GetByIdAsync(id.Value, companyId.Value);
            if (dietaryCategory == null)
            {
                return NotFound();
            }

            return View(dietaryCategory);
        }

        // POST: DietaryCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            await _dietaryCategoryService.RemoveAsync(id, companyId.Value);
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
