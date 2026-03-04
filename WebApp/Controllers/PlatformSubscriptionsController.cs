using App.Contracts.BLL.Subscription;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using App.Domain.Subscription;
using System.Linq;
using System.Security.Claims;
using WebApp.ViewModels.PlatformSubscriptions;

namespace WebApp.Controllers
{
    [Authorize(Roles = "user")]
    public class PlatformSubscriptionsController : Controller
    {
        private readonly IPlatformSubscriptionService _platformSubscriptionService;
        private readonly IPlatformSubscriptionTierService _platformSubscriptionTierService;
        private readonly IPlatformSubscriptionStatusService _platformSubscriptionStatusService;

        public PlatformSubscriptionsController(
            IPlatformSubscriptionService platformSubscriptionService,
            IPlatformSubscriptionTierService platformSubscriptionTierService,
            IPlatformSubscriptionStatusService platformSubscriptionStatusService)
        {
            _platformSubscriptionService = platformSubscriptionService;
            _platformSubscriptionTierService = platformSubscriptionTierService;
            _platformSubscriptionStatusService = platformSubscriptionStatusService;
        }

        // GET: PlatformSubscriptions
        public async Task<IActionResult> Index()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            return View(await _platformSubscriptionService.GetAllByCompanyIdAsync(companyId.Value));
        }

        // GET: PlatformSubscriptions/Details/5
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

            var platformSubscription = await _platformSubscriptionService.GetByIdAsync(id.Value, companyId.Value);
            if (platformSubscription == null)
            {
                return NotFound();
            }

            return View(platformSubscription);
        }

        // GET: PlatformSubscriptions/Create
        public async Task<IActionResult> Create()
        {
            return View(await BuildEditViewModelAsync(new PlatformSubscription()));
        }

        // POST: PlatformSubscriptions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PlatformSubscriptionEditViewModel viewModel)
        {
            var companyId = GetCurrentCompanyId();
            var userId = GetCurrentUserId();
            if (companyId == null || userId == null)
            {
                return Forbid();
            }

            if (viewModel.PlatformSubscription == null)
            {
                return BadRequest();
            }

            var platformSubscription = viewModel.PlatformSubscription;
            if (ModelState.IsValid)
            {
                platformSubscription.CompanyId = companyId.Value;
                platformSubscription.CreatedByAppUserId = userId.Value;
                platformSubscription.CreatedAt = DateTime.UtcNow;
                platformSubscription.UpdatedAt = null;
                platformSubscription.DeletedAt = null;

                await _platformSubscriptionService.AddAsync(platformSubscription, companyId.Value);
                return RedirectToAction(nameof(Index));
            }

            return View(await BuildEditViewModelAsync(platformSubscription));
        }

        // GET: PlatformSubscriptions/Edit/5
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

            var platformSubscription = await _platformSubscriptionService.GetByIdAsync(id.Value, companyId.Value);
            if (platformSubscription == null)
            {
                return NotFound();
            }

            return View(await BuildEditViewModelAsync(platformSubscription));
        }

        // POST: PlatformSubscriptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, PlatformSubscriptionEditViewModel viewModel)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            if (viewModel.PlatformSubscription == null)
            {
                return BadRequest();
            }

            var platformSubscription = viewModel.PlatformSubscription;
            if (id != platformSubscription.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existing = await _platformSubscriptionService.GetByIdAsync(id, companyId.Value);
                if (existing == null)
                {
                    return NotFound();
                }

                platformSubscription.CompanyId = companyId.Value;
                platformSubscription.CreatedByAppUserId = existing.CreatedByAppUserId;
                platformSubscription.CreatedAt = existing.CreatedAt;
                platformSubscription.DeletedAt = existing.DeletedAt;
                platformSubscription.UpdatedAt = DateTime.UtcNow;

                await _platformSubscriptionService.UpdateAsync(platformSubscription, companyId.Value);
                return RedirectToAction(nameof(Index));
            }

            return View(await BuildEditViewModelAsync(platformSubscription));
        }

        // GET: PlatformSubscriptions/Delete/5
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

            var platformSubscription = await _platformSubscriptionService.GetByIdAsync(id.Value, companyId.Value);
            if (platformSubscription == null)
            {
                return NotFound();
            }

            return View(platformSubscription);
        }

        // POST: PlatformSubscriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            await _platformSubscriptionService.RemoveAsync(id, companyId.Value);
            return RedirectToAction(nameof(Index));
        }

        private async Task<PlatformSubscriptionEditViewModel> BuildEditViewModelAsync(PlatformSubscription platformSubscription)
        {
            var tiers = await _platformSubscriptionTierService.GetAllAsync();
            var statuses = await _platformSubscriptionStatusService.GetAllAsync();

            return new PlatformSubscriptionEditViewModel
            {
                PlatformSubscription = platformSubscription,
                PlatformSubscriptionTierOptions = tiers
                    .Select(t => new SelectListItem(t.Name, t.Id.ToString(), t.Id == platformSubscription.PlatformSubscriptionTierId))
                    .ToList(),
                PlatformSubscriptionStatusOptions = statuses
                    .Select(s => new SelectListItem(s.Label, s.Id.ToString(), s.Id == platformSubscription.PlatformSubscriptionStatusId))
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
