using App.Contracts.BLL.Core;
using App.Contracts.BLL.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using App.Domain.Core;
using System.Security.Claims;
using System.Linq;
using WebApp.ViewModels.CompanyUsers;

namespace WebApp.Controllers
{
    [Authorize(Policy = "CompanyAdmin")]
    public class CompanyUsersController : Controller
    {
        private readonly ICompanyAppUserService _companyAppUserService;
        private readonly ICompanyRoleService _companyRoleService;
        private readonly IAppUserService _appUserService;

        public CompanyUsersController(
            ICompanyAppUserService companyAppUserService,
            ICompanyRoleService companyRoleService,
            IAppUserService appUserService)
        {
            _companyAppUserService = companyAppUserService;
            _companyRoleService = companyRoleService;
            _appUserService = appUserService;
        }

        // GET: CompanyUsers
        public async Task<IActionResult> Index()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            return View(await _companyAppUserService.GetAllByCompanyIdAsync(companyId.Value));
        }

        // GET: CompanyUsers/Details/5
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

            var companyAppUser = await _companyAppUserService.GetByIdAsync(id.Value, companyId.Value);
            if (companyAppUser == null)
            {
                return NotFound();
            }

            return View(companyAppUser);
        }

        // GET: CompanyUsers/Create
        public async Task<IActionResult> Create()
        {
            return View(await BuildEditViewModelAsync(new CompanyAppUser { IsActive = true }));
        }

        // POST: CompanyUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompanyUserEditViewModel viewModel)
        {
            var companyId = GetCurrentCompanyId();
            var userId = GetCurrentUserId();
            if (companyId == null || userId == null)
            {
                return Forbid();
            }

            if (viewModel.CompanyAppUser == null)
            {
                return BadRequest();
            }

            var companyAppUser = viewModel.CompanyAppUser;

            if (!await IsAllowedRoleAsync(companyAppUser.CompanyRoleId))
            {
                ModelState.AddModelError(nameof(viewModel.CompanyAppUser.CompanyRoleId), "Selected role is not allowed for tenant assignments.");
            }

            var existingAssignments = await _companyAppUserService.GetAllByCompanyIdAsync(companyId.Value);
            if (existingAssignments.Any(x => x.AppUserId == companyAppUser.AppUserId))
            {
                ModelState.AddModelError(nameof(viewModel.CompanyAppUser.AppUserId), "User is already assigned to this tenant.");
            }

            if (ModelState.IsValid)
            {
                companyAppUser.CompanyId = companyId.Value;
                companyAppUser.CreatedByAppUserId = userId.Value;
                companyAppUser.CreatedAt = DateTime.UtcNow;
                companyAppUser.UpdatedAt = null;
                companyAppUser.DeletedAt = null;

                await _companyAppUserService.AddAsync(companyAppUser, companyId.Value);
                return RedirectToAction(nameof(Index));
            }

            return View(await BuildEditViewModelAsync(companyAppUser));
        }

        // GET: CompanyUsers/Edit/5
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

            var companyAppUser = await _companyAppUserService.GetByIdAsync(id.Value, companyId.Value);
            if (companyAppUser == null)
            {
                return NotFound();
            }

            return View(await BuildEditViewModelAsync(companyAppUser));
        }

        // POST: CompanyUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CompanyUserEditViewModel viewModel)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            if (viewModel.CompanyAppUser == null)
            {
                return BadRequest();
            }

            var companyAppUser = viewModel.CompanyAppUser;
            if (id != companyAppUser.Id)
            {
                return NotFound();
            }

            if (!await IsAllowedRoleAsync(companyAppUser.CompanyRoleId))
            {
                ModelState.AddModelError(nameof(viewModel.CompanyAppUser.CompanyRoleId), "Selected role is not allowed for tenant assignments.");
            }

            var existingAssignments = await _companyAppUserService.GetAllByCompanyIdAsync(companyId.Value);
            if (existingAssignments.Any(x => x.AppUserId == companyAppUser.AppUserId && x.Id != companyAppUser.Id))
            {
                ModelState.AddModelError(nameof(viewModel.CompanyAppUser.AppUserId), "User is already assigned to this tenant.");
            }

            if (ModelState.IsValid)
            {
                var existing = await _companyAppUserService.GetByIdAsync(id, companyId.Value);
                if (existing == null)
                {
                    return NotFound();
                }

                companyAppUser.CompanyId = companyId.Value;
                companyAppUser.CreatedByAppUserId = existing.CreatedByAppUserId;
                companyAppUser.CreatedAt = existing.CreatedAt;
                companyAppUser.DeletedAt = existing.DeletedAt;
                companyAppUser.UpdatedAt = DateTime.UtcNow;

                await _companyAppUserService.UpdateAsync(companyAppUser, companyId.Value);
                return RedirectToAction(nameof(Index));
            }

            return View(await BuildEditViewModelAsync(companyAppUser));
        }

        // GET: CompanyUsers/Delete/5
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

            var companyAppUser = await _companyAppUserService.GetByIdAsync(id.Value, companyId.Value);
            if (companyAppUser == null)
            {
                return NotFound();
            }

            return View(companyAppUser);
        }

        // POST: CompanyUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            await _companyAppUserService.RemoveAsync(id, companyId.Value);
            return RedirectToAction(nameof(Index));
        }

        private async Task<CompanyUserEditViewModel> BuildEditViewModelAsync(CompanyAppUser companyAppUser)
        {
            var companyId = GetCurrentCompanyId();
            var allowedRoleCodes = new[] { "owner", "admin", "manager", "employee" };

            var users = await _appUserService.GetAllAsync();
            var roles = (await _companyRoleService.GetAllAsync())
                .Where(r => allowedRoleCodes.Contains(r.Code.ToLowerInvariant()))
                .ToList();

            var existingUsers = companyId == null
                ? []
                : (await _companyAppUserService.GetAllByCompanyIdAsync(companyId.Value))
                .Select(x => x.AppUserId)
                .ToHashSet();

            var candidateUsers = users
                .Where(u => companyAppUser.AppUserId == Guid.Empty || u.Id == companyAppUser.AppUserId || !existingUsers.Contains(u.Id))
                .OrderBy(u => u.Email)
                .ToList();

            return new CompanyUserEditViewModel
            {
                CompanyAppUser = companyAppUser,
                AppUserOptions = candidateUsers
                    .Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(u.Email ?? u.Id.ToString(), u.Id.ToString(), u.Id == companyAppUser.AppUserId))
                    .ToList(),
                CompanyRoleOptions = roles
                    .Select(r => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(r.Label, r.Id.ToString(), r.Id == companyAppUser.CompanyRoleId))
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

        private async Task<bool> IsAllowedRoleAsync(Guid roleId)
        {
            var allowedRoleCodes = new[] { "owner", "admin", "manager", "employee" };
            var role = await _companyRoleService.GetByIdAsync(roleId);

            return role != null && allowedRoleCodes.Contains(role.Code.ToLowerInvariant());
        }
    }
}
