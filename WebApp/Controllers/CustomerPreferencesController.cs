using App.Contracts.BLL.Core;
using App.Contracts.BLL.Menu;
using App.Domain.Menu;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.ViewModels.CustomerPreferences;

namespace WebApp.Controllers
{
    [Authorize(Roles = "user")]
    public class CustomerPreferencesController : Controller
    {
        private readonly ICustomerPreferenceService _customerPreferenceService;
        private readonly ICustomerService _customerService;
        private readonly IDietaryCategoryService _dietaryCategoryService;

        public CustomerPreferencesController(
            ICustomerPreferenceService customerPreferenceService,
            ICustomerService customerService,
            IDietaryCategoryService dietaryCategoryService)
        {
            _customerPreferenceService = customerPreferenceService;
            _customerService = customerService;
            _dietaryCategoryService = dietaryCategoryService;
        }

        // GET: CustomerPreferences
        public async Task<IActionResult> Index()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            return View(await _customerPreferenceService.GetAllByCompanyIdAsync(companyId.Value));
        }

        // GET: CustomerPreferences/Details/5
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

            var customerPreference = await _customerPreferenceService.GetByIdAsync(id.Value, companyId.Value);
            if (customerPreference == null)
            {
                return NotFound();
            }

            return View(customerPreference);
        }

        // GET: CustomerPreferences/Create
        public async Task<IActionResult> Create()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            return View(await BuildEditViewModelAsync(new CustomerPreference(), companyId.Value));
        }

        // POST: CustomerPreferences/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerPreferenceEditViewModel viewModel)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            var customerPreference = viewModel.CustomerPreference;
            if (ModelState.IsValid)
            {
                customerPreference.CreatedAt = DateTime.UtcNow;
                customerPreference.DeletedAt = null;
                await _customerPreferenceService.AddAsync(customerPreference, companyId.Value);
                return RedirectToAction(nameof(Index));
            }

            return View(await BuildEditViewModelAsync(customerPreference, companyId.Value));
        }

        // GET: CustomerPreferences/Edit/5
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

            var customerPreference = await _customerPreferenceService.GetByIdAsync(id.Value, companyId.Value);
            if (customerPreference == null)
            {
                return NotFound();
            }

            return View(await BuildEditViewModelAsync(customerPreference, companyId.Value));
        }

        // POST: CustomerPreferences/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CustomerPreferenceEditViewModel viewModel)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            var customerPreference = viewModel.CustomerPreference;
            if (id != customerPreference.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existing = await _customerPreferenceService.GetByIdAsync(id, companyId.Value);
                if (existing == null)
                {
                    return NotFound();
                }

                customerPreference.CreatedAt = existing.CreatedAt;
                customerPreference.DeletedAt = existing.DeletedAt;
                await _customerPreferenceService.UpdateAsync(customerPreference, companyId.Value);
                return RedirectToAction(nameof(Index));
            }

            return View(await BuildEditViewModelAsync(customerPreference, companyId.Value));
        }

        // GET: CustomerPreferences/Delete/5
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

            var customerPreference = await _customerPreferenceService.GetByIdAsync(id.Value, companyId.Value);
            if (customerPreference == null)
            {
                return NotFound();
            }

            return View(customerPreference);
        }

        // POST: CustomerPreferences/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            await _customerPreferenceService.RemoveAsync(id, companyId.Value);
            return RedirectToAction(nameof(Index));
        }

        private async Task<CustomerPreferenceEditViewModel> BuildEditViewModelAsync(CustomerPreference customerPreference, Guid companyId)
        {
            var customers = await _customerService.GetAllByCompanyIdAsync(companyId);
            var dietaryCategories = await _dietaryCategoryService.GetAllByCompanyIdAsync(companyId);

            return new CustomerPreferenceEditViewModel
            {
                CustomerPreference = customerPreference,
                CustomerOptions = customers
                    .Select(c => new SelectListItem($"{c.FirstName} {c.LastName} ({c.Email})", c.Id.ToString(), c.Id == customerPreference.CustomerId))
                    .ToList(),
                DietaryCategoryOptions = dietaryCategories
                    .Select(dc => new SelectListItem(dc.Code, dc.Id.ToString(), dc.Id == customerPreference.DietaryCategoryId))
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
