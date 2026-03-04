using App.Contracts.BLL.Core;
using App.Contracts.BLL.Identity;
using App.Contracts.BLL.Support;
using App.Domain.Support;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApp.ViewModels.SupportAccess;

namespace WebApp.Controllers
{
    [Authorize(Roles = "admin")]
    public class SupportAccessController : Controller
    {
        private readonly ITenantSupportAccessService _tenantSupportAccessService;
        private readonly ICompanyService _companyService;
        private readonly IAppUserService _appUserService;

        public SupportAccessController(
            ITenantSupportAccessService tenantSupportAccessService,
            ICompanyService companyService,
            IAppUserService appUserService)
        {
            _tenantSupportAccessService = tenantSupportAccessService;
            _companyService = companyService;
            _appUserService = appUserService;
        }

        // GET: SupportAccess
        public async Task<IActionResult> Index()
        {
            return View(await _tenantSupportAccessService.GetAllAsync());
        }

        // GET: SupportAccess/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tenantSupportAccess = await _tenantSupportAccessService.GetByIdAsync(id.Value);
            if (tenantSupportAccess == null)
            {
                return NotFound();
            }

            return View(tenantSupportAccess);
        }

        // GET: SupportAccess/Create
        public async Task<IActionResult> Create()
        {
            return View(await BuildEditViewModelAsync(new TenantSupportAccess()));
        }

        // POST: SupportAccess/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupportAccessEditViewModel viewModel)
        {
            if (viewModel.TenantSupportAccess == null)
            {
                return BadRequest();
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Forbid();
            }

            var tenantSupportAccess = viewModel.TenantSupportAccess;
            if (ModelState.IsValid)
            {
                tenantSupportAccess.GrantedByAppUserId = userId.Value;
                tenantSupportAccess.GrantedAt = DateTime.UtcNow;
                await _tenantSupportAccessService.AddAsync(tenantSupportAccess);
                return RedirectToAction(nameof(Index));
            }

            return View(await BuildEditViewModelAsync(tenantSupportAccess));
        }

        // GET: SupportAccess/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tenantSupportAccess = await _tenantSupportAccessService.GetByIdAsync(id.Value);
            if (tenantSupportAccess == null)
            {
                return NotFound();
            }

            return View(await BuildEditViewModelAsync(tenantSupportAccess));
        }

        // POST: SupportAccess/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, SupportAccessEditViewModel viewModel)
        {
            if (viewModel.TenantSupportAccess == null)
            {
                return BadRequest();
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Forbid();
            }

            var tenantSupportAccess = viewModel.TenantSupportAccess;
            if (id != tenantSupportAccess.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existing = await _tenantSupportAccessService.GetByIdAsync(id);
                if (existing == null)
                {
                    return NotFound();
                }

                tenantSupportAccess.GrantedByAppUserId = existing.GrantedByAppUserId;
                tenantSupportAccess.GrantedAt = existing.GrantedAt;
                await _tenantSupportAccessService.UpdateAsync(tenantSupportAccess);
                return RedirectToAction(nameof(Index));
            }

            return View(await BuildEditViewModelAsync(tenantSupportAccess));
        }

        // GET: SupportAccess/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tenantSupportAccess = await _tenantSupportAccessService.GetByIdAsync(id.Value);
            if (tenantSupportAccess == null)
            {
                return NotFound();
            }

            return View(tenantSupportAccess);
        }

        // POST: SupportAccess/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _tenantSupportAccessService.RemoveAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<SupportAccessEditViewModel> BuildEditViewModelAsync(TenantSupportAccess tenantSupportAccess)
        {
            var companies = await _companyService.GetAllAsync();
            var users = await _appUserService.GetAllAsync();

            return new SupportAccessEditViewModel
            {
                TenantSupportAccess = tenantSupportAccess,
                CompanyOptions = companies
                    .Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(c.Name, c.Id.ToString(), c.Id == tenantSupportAccess.CompanyId))
                    .ToList(),
                GrantedByUserOptions = users
                    .Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(
                        $"{u.FirstName} {u.LastName}".Trim(),
                        u.Id.ToString(),
                        u.Id == tenantSupportAccess.GrantedByAppUserId))
                    .ToList(),
                SupportUserOptions = users
                    .Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(
                        $"{u.FirstName} {u.LastName}".Trim(),
                        u.Id.ToString(),
                        u.Id == tenantSupportAccess.SupportUserId))
                    .ToList()
            };
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
