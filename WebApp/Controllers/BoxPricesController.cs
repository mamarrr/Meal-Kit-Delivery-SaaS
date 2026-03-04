using App.Contracts.BLL.Subscription;
using App.Domain.Subscription;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApp.ViewModels.BoxPrices;

namespace WebApp.Controllers
{
    [Authorize(Roles = "user")]
    public class BoxPricesController : Controller
    {
        private readonly IBoxPriceService _boxPriceService;
        private readonly IBoxService _boxService;

        public BoxPricesController(IBoxPriceService boxPriceService, IBoxService boxService)
        {
            _boxPriceService = boxPriceService;
            _boxService = boxService;
        }

        // GET: BoxPrices
        public async Task<IActionResult> Index()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            var prices = await _boxPriceService.GetAllByCompanyIdAsync(companyId.Value);
            return View(prices);
        }

        // GET: BoxPrices/Details/5
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

            var boxPrice = await _boxPriceService.GetByIdAsync(id.Value, companyId.Value);
            if (boxPrice == null)
            {
                return NotFound();
            }

            return View(boxPrice);
        }

        // GET: BoxPrices/Create
        public async Task<IActionResult> Create()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            var boxes = await _boxService.GetAllByCompanyIdAsync(companyId.Value);
            var viewModel = new BoxPriceEditViewModel
            {
                BoxPrice = new BoxPrice { IsActive = true },
                BoxOptions = boxes.Select(b => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = $"{b.DisplayName} ({b.MealsCount} meals, {b.PeopleCount} people)"
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: BoxPrices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BoxPriceEditViewModel viewModel)
        {
            var companyId = GetCurrentCompanyId();
            var userId = GetCurrentUserId();
            if (companyId == null || userId == null)
            {
                return Forbid();
            }

            if (viewModel.BoxPrice == null)
            {
                return BadRequest();
            }

            var boxPrice = viewModel.BoxPrice;

            // Validate box belongs to company
            var box = await _boxService.GetByIdAsync(boxPrice.BoxId, companyId.Value);
            if (box == null)
            {
                ModelState.AddModelError("BoxPrice.BoxId", "Selected box is not valid or does not belong to your company.");
            }

            if (ModelState.IsValid)
            {
                boxPrice.CreatedAt = DateTime.UtcNow;
                boxPrice.UpdatedAt = null;
                boxPrice.DeletedAt = null;
                boxPrice.CreatedByAppUserId = userId.Value;
                boxPrice.CompanyId = companyId.Value;

                await _boxPriceService.AddAsync(boxPrice, companyId.Value);
                return RedirectToAction(nameof(Index));
            }

            // Repopulate dropdown on error
            var boxes = await _boxService.GetAllByCompanyIdAsync(companyId.Value);
            viewModel.BoxOptions = boxes.Select(b => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = b.Id.ToString(),
                Text = $"{b.DisplayName} ({b.MealsCount} meals, {b.PeopleCount} people)"
            }).ToList();

            return View(viewModel);
        }

        // GET: BoxPrices/Edit/5
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

            var boxPrice = await _boxPriceService.GetByIdAsync(id.Value, companyId.Value);
            if (boxPrice == null)
            {
                return NotFound();
            }

            var boxes = await _boxService.GetAllByCompanyIdAsync(companyId.Value);
            var viewModel = new BoxPriceEditViewModel
            {
                BoxPrice = boxPrice,
                BoxOptions = boxes.Select(b => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = $"{b.DisplayName} ({b.MealsCount} meals, {b.PeopleCount} people)",
                    Selected = b.Id == boxPrice.BoxId
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: BoxPrices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, BoxPriceEditViewModel viewModel)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            if (viewModel.BoxPrice == null)
            {
                return BadRequest();
            }

            var boxPrice = viewModel.BoxPrice;
            if (id != boxPrice.Id)
            {
                return NotFound();
            }

            // Validate box belongs to company
            var box = await _boxService.GetByIdAsync(boxPrice.BoxId, companyId.Value);
            if (box == null)
            {
                ModelState.AddModelError("BoxPrice.BoxId", "Selected box is not valid or does not belong to your company.");
            }

            if (ModelState.IsValid)
            {
                var existing = await _boxPriceService.GetByIdAsync(id, companyId.Value);
                if (existing == null)
                {
                    return NotFound();
                }

                boxPrice.CompanyId = companyId.Value;
                boxPrice.CreatedByAppUserId = existing.CreatedByAppUserId;
                boxPrice.CreatedAt = existing.CreatedAt;
                boxPrice.DeletedAt = existing.DeletedAt;
                boxPrice.UpdatedAt = DateTime.UtcNow;

                await _boxPriceService.UpdateAsync(boxPrice, companyId.Value);
                return RedirectToAction(nameof(Index));
            }

            // Repopulate dropdown on error
            var boxes = await _boxService.GetAllByCompanyIdAsync(companyId.Value);
            viewModel.BoxOptions = boxes.Select(b => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = b.Id.ToString(),
                Text = $"{b.DisplayName} ({b.MealsCount} meals, {b.PeopleCount} people)",
                Selected = b.Id == boxPrice.BoxId
            }).ToList();

            return View(viewModel);
        }

        // GET: BoxPrices/Delete/5
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

            var boxPrice = await _boxPriceService.GetByIdAsync(id.Value, companyId.Value);
            if (boxPrice == null)
            {
                return NotFound();
            }

            return View(boxPrice);
        }

        // POST: BoxPrices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            await _boxPriceService.RemoveAsync(id, companyId.Value);
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
