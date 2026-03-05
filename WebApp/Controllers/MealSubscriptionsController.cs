using App.Contracts.BLL.Core;
using App.Contracts.BLL.Subscription;
using App.Domain.Subscription;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using WebApp.ViewModels.MealSubscriptions;

namespace WebApp.Controllers
{
    [Authorize(Policy = "CustomerAccess")]
    public class MealSubscriptionsController : Controller
    {
        private readonly IMealSubscriptionService _mealSubscriptionService;
        private readonly ICustomerService _customerService;
        private readonly ICustomerAppUserService _customerAppUserService;
        private readonly IBoxService _boxService;

        public MealSubscriptionsController(
            IMealSubscriptionService mealSubscriptionService,
            ICustomerService customerService,
            ICustomerAppUserService customerAppUserService,
            IBoxService boxService)
        {
            _mealSubscriptionService = mealSubscriptionService;
            _customerService = customerService;
            _customerAppUserService = customerAppUserService;
            _boxService = boxService;
        }

        // GET: MealSubscriptions
        public async Task<IActionResult> Index()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            var customerId = await GetCurrentCustomerIdAsync(companyId.Value);
            if (customerId == null)
            {
                return Forbid();
            }

            return View(await _mealSubscriptionService.GetAllByCustomerIdAsync(customerId.Value, companyId.Value));
        }

        // GET: MealSubscriptions/Details/5
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

            var customerId = await GetCurrentCustomerIdAsync(companyId.Value);
            if (customerId == null)
            {
                return Forbid();
            }

            var mealSubscription = await _mealSubscriptionService.GetByIdAsync(id.Value, companyId.Value);
            if (mealSubscription == null || mealSubscription.CustomerId != customerId.Value)
            {
                return NotFound();
            }

            return View(mealSubscription);
        }

        // GET: MealSubscriptions/Create
        public async Task<IActionResult> Create(Guid? boxId = null)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            var customerId = await GetCurrentCustomerIdAsync(companyId.Value);
            if (customerId == null)
            {
                return Forbid();
            }

            var mealSubscription = new MealSubscription
            {
                IsActive = true,
                StartDate = DateTime.UtcNow.Date,
                CustomerId = customerId.Value
            };

            if (boxId.HasValue)
            {
                mealSubscription.BoxId = boxId.Value;
            }

            return View(await BuildEditViewModelAsync(mealSubscription, companyId.Value));
        }

        // POST: MealSubscriptions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MealSubscriptionEditViewModel viewModel)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            var customerId = await GetCurrentCustomerIdAsync(companyId.Value);
            if (customerId == null)
            {
                return Forbid();
            }

            var mealSubscription = viewModel.MealSubscription;
            mealSubscription.CustomerId = customerId.Value;

            if (ModelState.IsValid)
            {
                mealSubscription.CreatedAt = DateTime.UtcNow;
                mealSubscription.UpdatedAt = null;
                mealSubscription.DeletedAt = null;
                await _mealSubscriptionService.AddAsync(mealSubscription, companyId.Value);
                return RedirectToAction(nameof(Index));
            }

            return View(await BuildEditViewModelAsync(mealSubscription, companyId.Value));
        }

        // GET: MealSubscriptions/Edit/5
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

            var customerId = await GetCurrentCustomerIdAsync(companyId.Value);
            if (customerId == null)
            {
                return Forbid();
            }

            var mealSubscription = await _mealSubscriptionService.GetByIdAsync(id.Value, companyId.Value);
            if (mealSubscription == null || mealSubscription.CustomerId != customerId.Value)
            {
                return NotFound();
            }

            return View(await BuildEditViewModelAsync(mealSubscription, companyId.Value));
        }

        // POST: MealSubscriptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, MealSubscriptionEditViewModel viewModel)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            var customerId = await GetCurrentCustomerIdAsync(companyId.Value);
            if (customerId == null)
            {
                return Forbid();
            }

            var mealSubscription = viewModel.MealSubscription;
            if (id != mealSubscription.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existing = await _mealSubscriptionService.GetByIdAsync(id, companyId.Value);
                if (existing == null)
                {
                    return NotFound();
                }

                if (existing.CustomerId != customerId.Value)
                {
                    return NotFound();
                }

                mealSubscription.CreatedAt = existing.CreatedAt;
                mealSubscription.CompanyId = companyId.Value;
                mealSubscription.CustomerId = existing.CustomerId;
                mealSubscription.UpdatedAt = DateTime.UtcNow;
                await _mealSubscriptionService.UpdateAsync(mealSubscription, companyId.Value);

                return RedirectToAction(nameof(Index));
            }

            return View(await BuildEditViewModelAsync(mealSubscription, companyId.Value));
        }

        // GET: MealSubscriptions/Delete/5
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

            var customerId = await GetCurrentCustomerIdAsync(companyId.Value);
            if (customerId == null)
            {
                return Forbid();
            }

            var mealSubscription = await _mealSubscriptionService.GetByIdAsync(id.Value, companyId.Value);
            if (mealSubscription == null || mealSubscription.CustomerId != customerId.Value)
            {
                return NotFound();
            }

            return View(mealSubscription);
        }

        // POST: MealSubscriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            var customerId = await GetCurrentCustomerIdAsync(companyId.Value);
            if (customerId == null)
            {
                return Forbid();
            }

            var existing = await _mealSubscriptionService.GetByIdAsync(id, companyId.Value);
            if (existing == null || existing.CustomerId != customerId.Value)
            {
                return NotFound();
            }

            await _mealSubscriptionService.RemoveAsync(id, companyId.Value);
            return RedirectToAction(nameof(Index));
        }

        private async Task<MealSubscriptionEditViewModel> BuildEditViewModelAsync(MealSubscription mealSubscription, Guid companyId)
        {
            var customers = await _customerService.GetAllByCompanyIdAsync(companyId);
            var boxes = await _boxService.GetAllByCompanyIdAsync(companyId);

            return new MealSubscriptionEditViewModel
            {
                MealSubscription = mealSubscription,
                CustomerOptions = customers
                    .Where(c => c.Id == mealSubscription.CustomerId)
                    .Select(c => new SelectListItem($"{c.FirstName} {c.LastName} ({c.Email})", c.Id.ToString(), c.Id == mealSubscription.CustomerId))
                    .ToList(),
                BoxOptions = boxes
                    .Select(b => new SelectListItem(b.DisplayName, b.Id.ToString(), b.Id == mealSubscription.BoxId))
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

        private async Task<Guid?> GetCurrentCustomerIdAsync(Guid companyId)
        {
            var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier)
                            ?? User.FindFirstValue("sub")
                            ?? User.FindFirstValue("user_id");

            if (!Guid.TryParse(userIdRaw, out var userId))
            {
                return null;
            }

            var customers = await _customerService.GetAllByCompanyIdAsync(companyId);
            var allowedCustomerIds = customers.Select(c => c.Id).ToHashSet();

            var customerLinks = await _customerAppUserService.GetAllByAppUserIdAsync(userId);
            var linkedCustomerId = customerLinks
                .Select(link => link.CustomerId)
                .FirstOrDefault(customerId => allowedCustomerIds.Contains(customerId));

            if (linkedCustomerId != Guid.Empty)
            {
                return linkedCustomerId;
            }

            var userEmail = User.Identity?.Name;
            if (!string.IsNullOrWhiteSpace(userEmail))
            {
                var emailMatch = customers.FirstOrDefault(c =>
                    string.Equals(c.Email, userEmail, StringComparison.OrdinalIgnoreCase));

                if (emailMatch != null)
                {
                    return emailMatch.Id;
                }
            }

            return null;
        }
    }
}
