using App.Contracts.BLL.Subscription;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using App.Domain.Subscription;
using System.Security.Claims;
using WebApp.ViewModels.PlatformSubscriptionTiers;

namespace WebApp.Controllers
{
    [Authorize(Roles = "admin")]
    public class PlatformSubscriptionTiersController : Controller
    {
        private readonly IPlatformSubscriptionTierService _platformSubscriptionTierService;

        public PlatformSubscriptionTiersController(IPlatformSubscriptionTierService platformSubscriptionTierService)
        {
            _platformSubscriptionTierService = platformSubscriptionTierService;
        }

        // GET: PlatformSubscriptionTiers
        public async Task<IActionResult> Index()
        {
            return View(await _platformSubscriptionTierService.GetAllAsync());
        }

        // GET: PlatformSubscriptionTiers/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var platformSubscriptionTier = await _platformSubscriptionTierService.GetByIdAsync(id.Value);
            if (platformSubscriptionTier == null)
            {
                return NotFound();
            }

            return View(platformSubscriptionTier);
        }

        // GET: PlatformSubscriptionTiers/Create
        public IActionResult Create()
        {
            return View(new PlatformSubscriptionTierEditViewModel
            {
                PlatformSubscriptionTier = new PlatformSubscriptionTier { IsActive = true }
            });
        }

        // POST: PlatformSubscriptionTiers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PlatformSubscriptionTierEditViewModel viewModel)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Forbid();
            }

            if (viewModel.PlatformSubscriptionTier == null)
            {
                return BadRequest();
            }

            var platformSubscriptionTier = viewModel.PlatformSubscriptionTier;
            if (ModelState.IsValid)
            {
                platformSubscriptionTier.CreatedByAppUserId = userId.Value;
                platformSubscriptionTier.CreatedAt = DateTime.UtcNow;
                platformSubscriptionTier.UpdatedAt = null;
                platformSubscriptionTier.DeletedAt = null;

                await _platformSubscriptionTierService.AddAsync(platformSubscriptionTier);
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        // GET: PlatformSubscriptionTiers/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var platformSubscriptionTier = await _platformSubscriptionTierService.GetByIdAsync(id.Value);
            if (platformSubscriptionTier == null)
            {
                return NotFound();
            }

            return View(new PlatformSubscriptionTierEditViewModel { PlatformSubscriptionTier = platformSubscriptionTier });
        }

        // POST: PlatformSubscriptionTiers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, PlatformSubscriptionTierEditViewModel viewModel)
        {
            if (viewModel.PlatformSubscriptionTier == null)
            {
                return BadRequest();
            }

            var platformSubscriptionTier = viewModel.PlatformSubscriptionTier;
            if (id != platformSubscriptionTier.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existing = await _platformSubscriptionTierService.GetByIdAsync(id);
                if (existing == null)
                {
                    return NotFound();
                }

                platformSubscriptionTier.CreatedByAppUserId = existing.CreatedByAppUserId;
                platformSubscriptionTier.CreatedAt = existing.CreatedAt;
                platformSubscriptionTier.DeletedAt = existing.DeletedAt;
                platformSubscriptionTier.UpdatedAt = DateTime.UtcNow;

                await _platformSubscriptionTierService.UpdateAsync(platformSubscriptionTier);
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        // GET: PlatformSubscriptionTiers/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var platformSubscriptionTier = await _platformSubscriptionTierService.GetByIdAsync(id.Value);
            if (platformSubscriptionTier == null)
            {
                return NotFound();
            }

            return View(platformSubscriptionTier);
        }

        // POST: PlatformSubscriptionTiers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _platformSubscriptionTierService.RemoveAsync(id);
            return RedirectToAction(nameof(Index));
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
