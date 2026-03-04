using App.Contracts.BLL.Core;
using App.Domain.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApp.ViewModels.CompanySettings;

namespace WebApp.Controllers
{
    [Authorize(Roles = "user")]
    public class CompanySettingsController : Controller
    {
        private readonly ICompanySettingsService _companySettingsService;

        public CompanySettingsController(ICompanySettingsService companySettingsService)
        {
            _companySettingsService = companySettingsService;
        }

        // GET: CompanySettings
        public async Task<IActionResult> Index()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            return View(await _companySettingsService.GetAllByCompanyIdAsync(companyId.Value));
        }

        // GET: CompanySettings/Details/5
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

            var companySettings = await _companySettingsService.GetByIdAsync(id.Value, companyId.Value);
            if (companySettings == null)
            {
                return NotFound();
            }

            return View(companySettings);
        }

        // GET: CompanySettings/Create
        public IActionResult Create()
        {
            return View(new CompanySettingsEditViewModel
            {
                CompanySettings = new CompanySettings()
            });
        }

        // POST: CompanySettings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompanySettingsEditViewModel viewModel)
        {
            var companyId = GetCurrentCompanyId();
            var userId = GetCurrentUserId();
            if (companyId == null || userId == null)
            {
                return Forbid();
            }

            if (viewModel.CompanySettings == null)
            {
                return BadRequest();
            }

            var companySettings = viewModel.CompanySettings;
            if (ModelState.IsValid)
            {
                companySettings.CompanyId = companyId.Value;
                companySettings.UpdatedByAppUserId = userId.Value;
                companySettings.UpdatedAt = DateTime.UtcNow;

                await _companySettingsService.AddAsync(companySettings, companyId.Value);
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        // GET: CompanySettings/Edit/5
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

            var companySettings = await _companySettingsService.GetByIdAsync(id.Value, companyId.Value);
            if (companySettings == null)
            {
                return NotFound();
            }

            return View(new CompanySettingsEditViewModel { CompanySettings = companySettings });
        }

        // POST: CompanySettings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CompanySettingsEditViewModel viewModel)
        {
            var companyId = GetCurrentCompanyId();
            var userId = GetCurrentUserId();
            if (companyId == null || userId == null)
            {
                return Forbid();
            }

            if (viewModel.CompanySettings == null)
            {
                return BadRequest();
            }

            var companySettings = viewModel.CompanySettings;
            if (id != companySettings.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existing = await _companySettingsService.GetByIdAsync(id, companyId.Value);
                if (existing == null)
                {
                    return NotFound();
                }

                companySettings.CompanyId = companyId.Value;
                companySettings.UpdatedByAppUserId = userId.Value;
                companySettings.UpdatedAt = DateTime.UtcNow;

                await _companySettingsService.UpdateAsync(companySettings, companyId.Value);
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        // GET: CompanySettings/Delete/5
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

            var companySettings = await _companySettingsService.GetByIdAsync(id.Value, companyId.Value);
            if (companySettings == null)
            {
                return NotFound();
            }

            return View(companySettings);
        }

        // POST: CompanySettings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            await _companySettingsService.RemoveAsync(id, companyId.Value);
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
