using App.Contracts.BLL.Delivery;
using App.Domain.Delivery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using WebApp.ViewModels.DeliveryAttempts;

namespace WebApp.Controllers
{
    [Authorize(Policy = "CompanyEmployee")]
    public class DeliveryAttemptsController : Controller
    {
        private readonly IDeliveryAttemptService _deliveryAttemptService;
        private readonly IDeliveryService _deliveryService;

        public DeliveryAttemptsController(
            IDeliveryAttemptService deliveryAttemptService,
            IDeliveryService deliveryService)
        {
            _deliveryAttemptService = deliveryAttemptService;
            _deliveryService = deliveryService;
        }

        // GET: DeliveryAttempts
        public async Task<IActionResult> Index()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            return View(await _deliveryAttemptService.GetAllByCompanyIdAsync(companyId.Value));
        }

        // GET: DeliveryAttempts/Details/5
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

            var deliveryAttempt = await _deliveryAttemptService.GetByIdAndCompanyAsync(id.Value, companyId.Value);
            if (deliveryAttempt == null)
            {
                return NotFound();
            }

            return View(deliveryAttempt);
        }

        // GET: DeliveryAttempts/Create
        public async Task<IActionResult> Create()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            return View(await BuildEditViewModelAsync(new DeliveryAttempt(), companyId.Value));
        }

        // POST: DeliveryAttempts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DeliveryAttemptEditViewModel viewModel)
        {
            var companyId = GetCurrentCompanyId();
            var userId = GetCurrentUserId();
            if (companyId == null || userId == null)
            {
                return Forbid();
            }

            if (viewModel.DeliveryAttempt == null)
            {
                return BadRequest();
            }

            var deliveryAttempt = viewModel.DeliveryAttempt;
            if (ModelState.IsValid)
            {
                deliveryAttempt.CreatedAt = DateTime.UtcNow;
                deliveryAttempt.UpdatedAt = null;
                deliveryAttempt.DeletedAt = null;

                await _deliveryAttemptService.AddAsync(deliveryAttempt);
                return RedirectToAction(nameof(Index));
            }

            return View(await BuildEditViewModelAsync(deliveryAttempt, companyId.Value));
        }

        // GET: DeliveryAttempts/Edit/5
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

            var deliveryAttempt = await _deliveryAttemptService.GetByIdAndCompanyAsync(id.Value, companyId.Value);
            if (deliveryAttempt == null)
            {
                return NotFound();
            }

            return View(await BuildEditViewModelAsync(deliveryAttempt, companyId.Value));
        }

        // POST: DeliveryAttempts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, DeliveryAttemptEditViewModel viewModel)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            if (viewModel.DeliveryAttempt == null)
            {
                return BadRequest();
            }

            var deliveryAttempt = viewModel.DeliveryAttempt;
            if (id != deliveryAttempt.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existing = await _deliveryAttemptService.GetByIdAndCompanyAsync(id, companyId.Value);
                if (existing == null)
                {
                    return NotFound();
                }

                deliveryAttempt.CreatedAt = existing.CreatedAt;
                deliveryAttempt.DeletedAt = existing.DeletedAt;
                deliveryAttempt.UpdatedAt = DateTime.UtcNow;

                await _deliveryAttemptService.UpdateAsync(deliveryAttempt);
                return RedirectToAction(nameof(Index));
            }

            return View(await BuildEditViewModelAsync(deliveryAttempt, companyId.Value));
        }

        // GET: DeliveryAttempts/Delete/5
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

            var deliveryAttempt = await _deliveryAttemptService.GetByIdAndCompanyAsync(id.Value, companyId.Value);
            if (deliveryAttempt == null)
            {
                return NotFound();
            }

            return View(deliveryAttempt);
        }

        // POST: DeliveryAttempts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            await _deliveryAttemptService.RemoveAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<DeliveryAttemptEditViewModel> BuildEditViewModelAsync(DeliveryAttempt deliveryAttempt, Guid companyId)
        {
            var deliveries = await _deliveryService.GetAllByCompanyIdAsync(companyId);

            return new DeliveryAttemptEditViewModel
            {
                DeliveryAttempt = deliveryAttempt,
                DeliveryOptions = deliveries.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = $"{d.Id.ToString()[..8]}... - {d.ScheduledTime:yyyy-MM-dd}",
                    Selected = d.Id == deliveryAttempt.DeliveryId
                }).ToList(),
                DeliveryAttemptResultOptions = new List<SelectListItem>
                {
                    new() { Value = "", Text = "-- Select Result --", Selected = deliveryAttempt.DeliveryAttemptResultId == Guid.Empty },
                    new() { Value = "1", Text = "Successful", Selected = false },
                    new() { Value = "2", Text = "Failed - Customer Not Home", Selected = false },
                    new() { Value = "3", Text = "Failed - Address Issue", Selected = false },
                    new() { Value = "4", Text = "Failed - Other", Selected = false }
                }
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
