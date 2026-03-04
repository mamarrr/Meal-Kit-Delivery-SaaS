using App.Contracts.BLL.Core;
using App.Contracts.BLL.Menu;
using App.Domain.Menu;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.ViewModels.CustomerExclusions;

namespace WebApp.Controllers
{
    [Authorize(Roles = "user")]
    public class CustomerExclusionsController : Controller
    {
        private readonly ICustomerExclusionService _customerExclusionService;
        private readonly ICustomerService _customerService;
        private readonly IIngredientService _ingredientService;

        public CustomerExclusionsController(
            ICustomerExclusionService customerExclusionService,
            ICustomerService customerService,
            IIngredientService ingredientService)
        {
            _customerExclusionService = customerExclusionService;
            _customerService = customerService;
            _ingredientService = ingredientService;
        }

        // GET: CustomerExclusions
        public async Task<IActionResult> Index()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            return View(await _customerExclusionService.GetAllByCompanyIdAsync(companyId.Value));
        }

        // GET: CustomerExclusions/Details/5
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

            var customerExclusion = await _customerExclusionService.GetByIdAsync(id.Value, companyId.Value);
            if (customerExclusion == null)
            {
                return NotFound();
            }

            return View(customerExclusion);
        }

        // GET: CustomerExclusions/Create
        public async Task<IActionResult> Create()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            return View(await BuildEditViewModelAsync(new CustomerExclusion(), companyId.Value));
        }

        // POST: CustomerExclusions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerExclusionEditViewModel viewModel)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            var customerExclusion = viewModel.CustomerExclusion;
            if (ModelState.IsValid)
            {
                customerExclusion.CreatedAt = DateTime.UtcNow;
                customerExclusion.DeletedAt = null;
                await _customerExclusionService.AddAsync(customerExclusion, companyId.Value);
                return RedirectToAction(nameof(Index));
            }

            return View(await BuildEditViewModelAsync(customerExclusion, companyId.Value));
        }

        // GET: CustomerExclusions/Edit/5
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

            var customerExclusion = await _customerExclusionService.GetByIdAsync(id.Value, companyId.Value);
            if (customerExclusion == null)
            {
                return NotFound();
            }

            return View(await BuildEditViewModelAsync(customerExclusion, companyId.Value));
        }

        // POST: CustomerExclusions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CustomerExclusionEditViewModel viewModel)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            var customerExclusion = viewModel.CustomerExclusion;
            if (id != customerExclusion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existing = await _customerExclusionService.GetByIdAsync(id, companyId.Value);
                if (existing == null)
                {
                    return NotFound();
                }

                customerExclusion.CreatedAt = existing.CreatedAt;
                customerExclusion.DeletedAt = existing.DeletedAt;
                await _customerExclusionService.UpdateAsync(customerExclusion, companyId.Value);
                return RedirectToAction(nameof(Index));
            }

            return View(await BuildEditViewModelAsync(customerExclusion, companyId.Value));
        }

        // GET: CustomerExclusions/Delete/5
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

            var customerExclusion = await _customerExclusionService.GetByIdAsync(id.Value, companyId.Value);
            if (customerExclusion == null)
            {
                return NotFound();
            }

            return View(customerExclusion);
        }

        // POST: CustomerExclusions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            await _customerExclusionService.RemoveAsync(id, companyId.Value);
            return RedirectToAction(nameof(Index));
        }

        private async Task<CustomerExclusionEditViewModel> BuildEditViewModelAsync(CustomerExclusion customerExclusion, Guid companyId)
        {
            var customers = await _customerService.GetAllByCompanyIdAsync(companyId);
            var ingredients = await _ingredientService.GetAllByCompanyIdAsync(companyId);

            return new CustomerExclusionEditViewModel
            {
                CustomerExclusion = customerExclusion,
                CustomerOptions = customers
                    .Select(c => new SelectListItem($"{c.FirstName} {c.LastName} ({c.Email})", c.Id.ToString(), c.Id == customerExclusion.CustomerId))
                    .ToList(),
                IngredientOptions = ingredients
                    .Select(i => new SelectListItem(i.Name, i.Id.ToString(), i.Id == customerExclusion.IngredientId))
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
