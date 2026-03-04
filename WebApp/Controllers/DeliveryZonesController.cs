using App.Contracts.BLL.Delivery;
using App.Domain.Delivery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApp.ViewModels.DeliveryZones;

namespace WebApp.Controllers
{
    [Authorize(Policy = "CompanyAdmin")]
    public class DeliveryZonesController : Controller
    {
        private readonly IDeliveryZoneService _deliveryZoneService;

        public DeliveryZonesController(IDeliveryZoneService deliveryZoneService)
        {
            _deliveryZoneService = deliveryZoneService;
        }

        // GET: DeliveryZones
        public async Task<IActionResult> Index()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            return View(await _deliveryZoneService.GetAllByCompanyIdAsync(companyId.Value));
        }

        // GET: DeliveryZones/Details/5
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

            var deliveryZone = await _deliveryZoneService.GetByIdAsync(id.Value, companyId.Value);
            if (deliveryZone == null)
            {
                return NotFound();
            }

            return View(deliveryZone);
        }

        // GET: DeliveryZones/Create
        public IActionResult Create()
        {
            return View(new DeliveryZoneEditViewModel
            {
                DeliveryZone = new DeliveryZone { IsActive = true }
            });
        }

        // POST: DeliveryZones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DeliveryZoneEditViewModel viewModel)
        {
            var companyId = GetCurrentCompanyId();
            var userId = GetCurrentUserId();
            if (companyId == null || userId == null)
            {
                return Forbid();
            }

            if (viewModel.DeliveryZone == null)
            {
                return BadRequest();
            }

            var deliveryZone = viewModel.DeliveryZone;

            var canCreateZone = await _deliveryZoneService.CanCreateZoneAsync(companyId.Value);
            if (!canCreateZone)
            {
                var maxZones = await _deliveryZoneService.GetMaxZonesForCompanyAsync(companyId.Value);
                ModelState.AddModelError(string.Empty, $"Your current plan allows up to {maxZones} delivery zones.");
            }

            if (ModelState.IsValid)
            {
                deliveryZone.CreatedAt = DateTime.UtcNow;
                deliveryZone.UpdatedAt = null;
                deliveryZone.DeletedAt = null;
                deliveryZone.CompanyId = companyId.Value;
                deliveryZone.CreatedByAppUserId = userId.Value;

                await _deliveryZoneService.AddAsync(deliveryZone, companyId.Value);
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        // GET: DeliveryZones/Edit/5
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

            var deliveryZone = await _deliveryZoneService.GetByIdAsync(id.Value, companyId.Value);
            if (deliveryZone == null)
            {
                return NotFound();
            }

            return View(new DeliveryZoneEditViewModel { DeliveryZone = deliveryZone });
        }

        // POST: DeliveryZones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, DeliveryZoneEditViewModel viewModel)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            if (viewModel.DeliveryZone == null)
            {
                return BadRequest();
            }

            var deliveryZone = viewModel.DeliveryZone;
            if (id != deliveryZone.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existing = await _deliveryZoneService.GetByIdAsync(id, companyId.Value);
                if (existing == null)
                {
                    return NotFound();
                }

                deliveryZone.CompanyId = companyId.Value;
                deliveryZone.CreatedByAppUserId = existing.CreatedByAppUserId;
                deliveryZone.CreatedAt = existing.CreatedAt;
                deliveryZone.DeletedAt = existing.DeletedAt;
                deliveryZone.UpdatedAt = DateTime.UtcNow;

                await _deliveryZoneService.UpdateAsync(deliveryZone, companyId.Value);
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        // GET: DeliveryZones/Delete/5
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

            var deliveryZone = await _deliveryZoneService.GetByIdAsync(id.Value, companyId.Value);
            if (deliveryZone == null)
            {
                return NotFound();
            }

            return View(deliveryZone);
        }

        // POST: DeliveryZones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            await _deliveryZoneService.RemoveAsync(id, companyId.Value);
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
