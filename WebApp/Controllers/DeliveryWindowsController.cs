using App.Contracts.BLL.Delivery;
using App.Domain.Delivery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApp.ViewModels.DeliveryWindows;

namespace WebApp.Controllers
{
    [Authorize(Policy = "CompanyManager")]
    public class DeliveryWindowsController : Controller
    {
        private readonly IDeliveryWindowService _deliveryWindowService;
        private readonly IDeliveryZoneService _deliveryZoneService;

        public DeliveryWindowsController(IDeliveryWindowService deliveryWindowService, IDeliveryZoneService deliveryZoneService)
        {
            _deliveryWindowService = deliveryWindowService;
            _deliveryZoneService = deliveryZoneService;
        }

        // GET: DeliveryWindows
        public async Task<IActionResult> Index()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            var windows = await _deliveryWindowService.GetAllByCompanyIdAsync(companyId.Value);
            return View(windows);
        }

        // GET: DeliveryWindows/Details/5
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

            var window = await _deliveryWindowService.GetByIdAsync(id.Value);
            if (window == null)
            {
                return NotFound();
            }

            // Verify the window belongs to a zone owned by the company
            var zone = await _deliveryZoneService.GetByIdAsync(window.DeliveryZoneId, companyId.Value);
            if (zone == null)
            {
                return Forbid();
            }

            return View(window);
        }

        // GET: DeliveryWindows/Create
        public async Task<IActionResult> Create()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            var zones = await _deliveryZoneService.GetAllByCompanyIdAsync(companyId.Value);
            var viewModel = new DeliveryWindowEditViewModel
            {
                DeliveryWindow = new DeliveryWindow { IsActive = true, DayOfWeek = 1 },
                DeliveryZoneOptions = zones.Select(z => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = z.Id.ToString(),
                    Text = z.Name
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: DeliveryWindows/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DeliveryWindowEditViewModel viewModel)
        {
            var companyId = GetCurrentCompanyId();
            var userId = GetCurrentUserId();
            if (companyId == null || userId == null)
            {
                return Forbid();
            }

            if (viewModel.DeliveryWindow == null)
            {
                return BadRequest();
            }

            var window = viewModel.DeliveryWindow;

            // Validate zone belongs to company
            var zone = await _deliveryZoneService.GetByIdAsync(window.DeliveryZoneId, companyId.Value);
            if (zone == null)
            {
                ModelState.AddModelError("DeliveryWindow.DeliveryZoneId", "Selected delivery zone is not valid or does not belong to your company.");
            }

            if (ModelState.IsValid)
            {
                window.CreatedAt = DateTime.UtcNow;
                window.UpdatedAt = null;
                window.DeletedAt = null;
                window.CreatedByAppUserId = userId.Value;

                await _deliveryWindowService.AddAsync(window);
                return RedirectToAction(nameof(Index));
            }

            // Repopulate dropdown on error
            var zones = await _deliveryZoneService.GetAllByCompanyIdAsync(companyId.Value);
            viewModel.DeliveryZoneOptions = zones.Select(z => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = z.Id.ToString(),
                Text = z.Name,
                Selected = z.Id == window.DeliveryZoneId
            }).ToList();

            return View(viewModel);
        }

        // GET: DeliveryWindows/Edit/5
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

            var window = await _deliveryWindowService.GetByIdAsync(id.Value);
            if (window == null)
            {
                return NotFound();
            }

            // Verify the window belongs to a zone owned by the company
            var zone = await _deliveryZoneService.GetByIdAsync(window.DeliveryZoneId, companyId.Value);
            if (zone == null)
            {
                return Forbid();
            }

            var zones = await _deliveryZoneService.GetAllByCompanyIdAsync(companyId.Value);
            var viewModel = new DeliveryWindowEditViewModel
            {
                DeliveryWindow = window,
                DeliveryZoneOptions = zones.Select(z => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = z.Id.ToString(),
                    Text = z.Name,
                    Selected = z.Id == window.DeliveryZoneId
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: DeliveryWindows/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, DeliveryWindowEditViewModel viewModel)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            if (viewModel.DeliveryWindow == null)
            {
                return BadRequest();
            }

            var window = viewModel.DeliveryWindow;
            if (id != window.Id)
            {
                return NotFound();
            }

            // Validate zone belongs to company
            var zone = await _deliveryZoneService.GetByIdAsync(window.DeliveryZoneId, companyId.Value);
            if (zone == null)
            {
                ModelState.AddModelError("DeliveryWindow.DeliveryZoneId", "Selected delivery zone is not valid or does not belong to your company.");
            }

            if (ModelState.IsValid)
            {
                var existing = await _deliveryWindowService.GetByIdAsync(id);
                if (existing == null)
                {
                    return NotFound();
                }

                // Verify the existing window belongs to a zone owned by the company
                var existingZone = await _deliveryZoneService.GetByIdAsync(existing.DeliveryZoneId, companyId.Value);
                if (existingZone == null)
                {
                    return Forbid();
                }

                window.CreatedByAppUserId = existing.CreatedByAppUserId;
                window.CreatedAt = existing.CreatedAt;
                window.DeletedAt = existing.DeletedAt;
                window.UpdatedAt = DateTime.UtcNow;

                await _deliveryWindowService.UpdateAsync(window);
                return RedirectToAction(nameof(Index));
            }

            // Repopulate dropdown on error
            var zones = await _deliveryZoneService.GetAllByCompanyIdAsync(companyId.Value);
            viewModel.DeliveryZoneOptions = zones.Select(z => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = z.Id.ToString(),
                Text = z.Name,
                Selected = z.Id == window.DeliveryZoneId
            }).ToList();

            return View(viewModel);
        }

        // GET: DeliveryWindows/Delete/5
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

            var window = await _deliveryWindowService.GetByIdAsync(id.Value);
            if (window == null)
            {
                return NotFound();
            }

            // Verify the window belongs to a zone owned by the company
            var zone = await _deliveryZoneService.GetByIdAsync(window.DeliveryZoneId, companyId.Value);
            if (zone == null)
            {
                return Forbid();
            }

            return View(window);
        }

        // POST: DeliveryWindows/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            var window = await _deliveryWindowService.GetByIdAsync(id);
            if (window == null)
            {
                return NotFound();
            }

            // Verify the window belongs to a zone owned by the company
            var zone = await _deliveryZoneService.GetByIdAsync(window.DeliveryZoneId, companyId.Value);
            if (zone == null)
            {
                return Forbid();
            }

            await _deliveryWindowService.RemoveAsync(id);
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
