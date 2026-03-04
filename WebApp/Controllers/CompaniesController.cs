using App.Contracts.BLL.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using App.Domain.Core;
using System.Security.Claims;
using WebApp.ViewModels.Companies;

namespace WebApp.Controllers
{
    [Authorize(Roles = "admin")]
    public class CompaniesController : Controller
    {
        private readonly ICompanyService _companyService;

        public CompaniesController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        // GET: Companies
        public async Task<IActionResult> Index()
        {
            return View(await _companyService.GetAllAsync());
        }

        // GET: Companies/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _companyService.GetByIdAsync(id.Value);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // GET: Companies/Create
        public IActionResult Create()
        {
            return View(new CompanyEditViewModel
            {
                Company = new Company()
            });
        }

        // POST: Companies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompanyEditViewModel viewModel)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Forbid();
            }

            if (viewModel.Company == null)
            {
                return BadRequest();
            }

            var company = viewModel.Company;
            if (ModelState.IsValid)
            {
                company.CreatedByAppUserId = userId.Value;
                company.CreatedAt = DateTime.UtcNow;
                company.UpdatedAt = null;
                company.DeletedAt = null;

                await _companyService.AddAsync(company);
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        // GET: Companies/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _companyService.GetByIdAsync(id.Value);
            if (company == null)
            {
                return NotFound();
            }

            return View(new CompanyEditViewModel { Company = company });
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CompanyEditViewModel viewModel)
        {
            if (viewModel.Company == null)
            {
                return BadRequest();
            }

            var company = viewModel.Company;
            if (id != company.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existing = await _companyService.GetByIdAsync(id);
                if (existing == null)
                {
                    return NotFound();
                }

                company.CreatedByAppUserId = existing.CreatedByAppUserId;
                company.CreatedAt = existing.CreatedAt;
                company.DeletedAt = existing.DeletedAt;
                company.UpdatedAt = DateTime.UtcNow;

                await _companyService.UpdateAsync(company);
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        // GET: Companies/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _companyService.GetByIdAsync(id.Value);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _companyService.RemoveAsync(id);
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
