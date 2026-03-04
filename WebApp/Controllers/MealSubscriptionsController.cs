using App.Contracts.BLL.Core;
using App.Contracts.BLL.Subscription;
using App.Domain.Subscription;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.ViewModels.MealSubscriptions;

namespace WebApp.Controllers
{
    [Authorize(Roles = "user")]
    public class MealSubscriptionsController : Controller
    {
        private readonly IMealSubscriptionService _mealSubscriptionService;
        private readonly ICustomerService _customerService;
        private readonly IBoxService _boxService;

        public MealSubscriptionsController(
            IMealSubscriptionService mealSubscriptionService,
            ICustomerService customerService,
            IBoxService boxService)
        {
            _mealSubscriptionService = mealSubscriptionService;
            _customerService = customerService;
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

            return View(await _mealSubscriptionService.GetAllByCompanyIdAsync(companyId.Value));
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

            var mealSubscription = await _mealSubscriptionService.GetByIdAsync(id.Value, companyId.Value);
            if (mealSubscription == null)
            {
                return NotFound();
            }

            return View(mealSubscription);
        }

        // GET: MealSubscriptions/Create
        public async Task<IActionResult> Create()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            return View(await BuildEditViewModelAsync(new MealSubscription(), companyId.Value));
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

            var mealSubscription = viewModel.MealSubscription;
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

            var mealSubscription = await _mealSubscriptionService.GetByIdAsync(id.Value, companyId.Value);
            if (mealSubscription == null)
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

                mealSubscription.CreatedAt = existing.CreatedAt;
                mealSubscription.CompanyId = companyId.Value;
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

            var mealSubscription = await _mealSubscriptionService.GetByIdAsync(id.Value, companyId.Value);
            if (mealSubscription == null)
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
    }
}
