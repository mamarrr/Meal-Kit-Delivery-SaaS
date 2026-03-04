using App.Contracts.BLL.Core;
using App.Contracts.BLL.Delivery;
using App.Contracts.BLL.Menu;
using App.Contracts.BLL.Subscription;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using App.Domain.Delivery;
using System.Security.Claims;
using WebApp.ViewModels.Deliveries;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Customer,CompanyOwner,CompanyAdmin,CompanyManager,CompanyEmployee")]
    public class DeliveriesController : Controller
    {
        private readonly IDeliveryService _deliveryService;
        private readonly IDeliveryAttemptService _deliveryAttemptService;
        private readonly ICustomerService _customerService;
        private readonly IWeeklyMenuService _weeklyMenuService;
        private readonly IDeliveryZoneService _deliveryZoneService;
        private readonly IBoxService _boxService;
        private readonly IMealSelectionService _mealSelectionService;
        private readonly IMealSubscriptionService _mealSubscriptionService;
        private readonly IOperationalLookupService _operationalLookupService;

        public DeliveriesController(
            IDeliveryService deliveryService,
            IDeliveryAttemptService deliveryAttemptService,
            ICustomerService customerService,
            IWeeklyMenuService weeklyMenuService,
            IDeliveryZoneService deliveryZoneService,
            IBoxService boxService,
            IMealSelectionService mealSelectionService,
            IMealSubscriptionService mealSubscriptionService,
            IOperationalLookupService operationalLookupService)
        {
            _deliveryService = deliveryService;
            _deliveryAttemptService = deliveryAttemptService;
            _customerService = customerService;
            _weeklyMenuService = weeklyMenuService;
            _deliveryZoneService = deliveryZoneService;
            _boxService = boxService;
            _mealSelectionService = mealSelectionService;
            _mealSubscriptionService = mealSubscriptionService;
            _operationalLookupService = operationalLookupService;
        }

        // GET: Deliveries
        public async Task<IActionResult> Index()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            return View(await _deliveryService.GetAllByCompanyIdAsync(companyId.Value));
        }

        // GET: Deliveries/Details/5
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

            var delivery = await _deliveryService.GetByIdAsync(id.Value, companyId.Value);
            if (delivery == null)
            {
                return NotFound();
            }

            return View(delivery);
        }

        // GET: Deliveries/Create
        public async Task<IActionResult> Create()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            return View(await BuildEditViewModelAsync(new Delivery(), companyId.Value));
        }

        // POST: Deliveries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DeliveryEditViewModel viewModel)
        {
            var companyId = GetCurrentCompanyId();
            var userId = GetCurrentUserId();
            if (companyId == null || userId == null)
            {
                return Forbid();
            }

            if (viewModel.Delivery == null)
            {
                return BadRequest();
            }

            var delivery = viewModel.Delivery;
            if (!await IsDeliveryWindowInCompanyAsync(delivery.DeliveryWindowId, companyId.Value))
            {
                ModelState.AddModelError("Delivery.DeliveryWindowId", "Selected delivery window is outside current company scope.");
            }

            if (ModelState.IsValid)
            {
                delivery.CreatedAt = DateTime.UtcNow;
                delivery.UpdatedAt = null;
                delivery.DeletedAt = null;
                delivery.CreatedByAppUserId = userId.Value;

                await _deliveryService.AddAsync(delivery, companyId.Value);
                return RedirectToAction(nameof(Index));
            }

            return View(await BuildEditViewModelAsync(delivery, companyId.Value));
        }

        // GET: Deliveries/Edit/5
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

            var delivery = await _deliveryService.GetByIdAsync(id.Value, companyId.Value);
            if (delivery == null)
            {
                return NotFound();
            }

            return View(await BuildEditViewModelAsync(delivery, companyId.Value));
        }

        // POST: Deliveries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, DeliveryEditViewModel viewModel)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            if (viewModel.Delivery == null)
            {
                return BadRequest();
            }

            var delivery = viewModel.Delivery;
            if (id != delivery.Id)
            {
                return NotFound();
            }

            if (!await IsDeliveryWindowInCompanyAsync(delivery.DeliveryWindowId, companyId.Value))
            {
                ModelState.AddModelError("Delivery.DeliveryWindowId", "Selected delivery window is outside current company scope.");
            }

            if (ModelState.IsValid)
            {
                var existing = await _deliveryService.GetByIdAsync(id, companyId.Value);
                if (existing == null)
                {
                    return NotFound();
                }

                delivery.CompanyId = companyId.Value;
                delivery.CreatedByAppUserId = existing.CreatedByAppUserId;
                delivery.CreatedAt = existing.CreatedAt;
                delivery.UpdatedAt = DateTime.UtcNow;
                delivery.DeletedAt = existing.DeletedAt;

                await _deliveryService.UpdateAsync(delivery, companyId.Value);
                return RedirectToAction(nameof(Index));
            }

            return View(await BuildEditViewModelAsync(delivery, companyId.Value));
        }

        // GET: Deliveries/Delete/5
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

            var delivery = await _deliveryService.GetByIdAsync(id.Value, companyId.Value);
            if (delivery == null)
            {
                return NotFound();
            }

            return View(delivery);
        }

        // POST: Deliveries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            await _deliveryService.RemoveAsync(id, companyId.Value);
            return RedirectToAction(nameof(Index));
        }

        // GET: Deliveries/Attempts/5
        public async Task<IActionResult> Attempts(Guid? id)
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

            var delivery = await _deliveryService.GetByIdAsync(id.Value, companyId.Value);
            if (delivery == null)
            {
                return NotFound();
            }

            var attempts = await _deliveryAttemptService.GetAllByDeliveryIdAsync(id.Value);
            ViewBag.Delivery = delivery;
            return View(attempts);
        }

        private async Task<DeliveryEditViewModel> BuildEditViewModelAsync(Delivery delivery, Guid companyId)
        {
            var customers = await _customerService.GetAllByCompanyIdAsync(companyId);
            var weeklyMenus = await _weeklyMenuService.GetAllByCompanyIdAsync(companyId);
            var deliveryZones = await _deliveryZoneService.GetAllByCompanyIdAsync(companyId);
            var boxes = await _boxService.GetAllByCompanyIdAsync(companyId);
            var mealSelections = await _mealSelectionService.GetAllByCompanyIdAsync(companyId);
            var mealSubscriptions = await _mealSubscriptionService.GetAllByCompanyIdAsync(companyId);
            var deliveryStatuses = await _operationalLookupService.GetDeliveryStatusesAsync();
            var deliveryWindows = await _operationalLookupService.GetDeliveryWindowsByCompanyIdAsync(companyId);

            return new DeliveryEditViewModel
            {
                Delivery = delivery,
                DeliveryStatusOptions = deliveryStatuses
                    .Select(s => new SelectListItem(s.Label, s.Id.ToString(), s.Id == delivery.DeliveryStatusId))
                    .ToList(),
                CustomerOptions = customers
                    .Select(c => new SelectListItem($"{c.FirstName} {c.LastName} ({c.Email})", c.Id.ToString(), c.Id == delivery.CustomerId))
                    .ToList(),
                WeeklyMenuOptions = weeklyMenus
                    .Select(w => new SelectListItem(w.WeekStartDate.ToString("yyyy-MM-dd"), w.Id.ToString(), w.Id == delivery.WeeklyMenuId))
                    .ToList(),
                DeliveryZoneOptions = deliveryZones
                    .Select(z => new SelectListItem(z.Name, z.Id.ToString(), z.Id == delivery.DeliveryZoneId))
                    .ToList(),
                DeliveryWindowOptions = deliveryWindows
                    .Select(w => new SelectListItem($"Day {w.DayOfWeek}: {w.StartTime:hh\\:mm}-{w.EndTime:hh\\:mm}", w.Id.ToString(), w.Id == delivery.DeliveryWindowId))
                    .ToList(),
                BoxOptions = boxes
                    .Select(b => new SelectListItem(b.DisplayName, b.Id.ToString(), b.Id == delivery.BoxId))
                    .ToList(),
                MealSelectionOptions = mealSelections
                    .Select(ms => new SelectListItem(ms.Id.ToString(), ms.Id.ToString(), ms.Id == delivery.MealSelectionId))
                    .ToList(),
                MealSubscriptionOptions = mealSubscriptions
                    .Select(ms => new SelectListItem(ms.Id.ToString(), ms.Id.ToString(), ms.Id == delivery.MealSubscriptionId))
                    .ToList()
            };
        }

        private async Task<bool> IsDeliveryWindowInCompanyAsync(Guid deliveryWindowId, Guid companyId)
        {
            return await _operationalLookupService.DeliveryWindowBelongsToCompanyAsync(deliveryWindowId, companyId);
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
