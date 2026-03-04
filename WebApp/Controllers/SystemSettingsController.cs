using App.Contracts.BLL.Identity;
using App.Contracts.BLL.Support;
using App.Domain.Support;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApp.ViewModels.SystemSettings;

namespace WebApp.Controllers
{
    [Authorize(Roles = "admin")]
    public class SystemSettingsController : Controller
    {
        private readonly ISystemSettingService _systemSettingService;
        private readonly IAppUserService _appUserService;

        public SystemSettingsController(ISystemSettingService systemSettingService, IAppUserService appUserService)
        {
            _systemSettingService = systemSettingService;
            _appUserService = appUserService;
        }

        // GET: SystemSettings
        public async Task<IActionResult> Index()
        {
            return View(await _systemSettingService.GetAllAsync());
        }

        // GET: SystemSettings/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemSetting = await _systemSettingService.GetByIdAsync(id.Value);
            if (systemSetting == null)
            {
                return NotFound();
            }

            return View(systemSetting);
        }

        // GET: SystemSettings/Create
        public async Task<IActionResult> Create()
        {
            return View(await BuildEditViewModelAsync(new SystemSetting()));
        }

        // POST: SystemSettings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SystemSettingEditViewModel viewModel)
        {
            if (viewModel.SystemSetting == null)
            {
                return BadRequest();
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Forbid();
            }

            var systemSetting = viewModel.SystemSetting;
            if (ModelState.IsValid)
            {
                systemSetting.UpdatedAt = DateTime.UtcNow;
                systemSetting.UpdatedByAppUserId = userId;
                await _systemSettingService.AddAsync(systemSetting);
                return RedirectToAction(nameof(Index));
            }

            return View(await BuildEditViewModelAsync(systemSetting));
        }

        // GET: SystemSettings/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemSetting = await _systemSettingService.GetByIdAsync(id.Value);
            if (systemSetting == null)
            {
                return NotFound();
            }
            return View(await BuildEditViewModelAsync(systemSetting));
        }

        // POST: SystemSettings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, SystemSettingEditViewModel viewModel)
        {
            if (viewModel.SystemSetting == null)
            {
                return BadRequest();
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Forbid();
            }

            var systemSetting = viewModel.SystemSetting;
            if (id != systemSetting.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existing = await _systemSettingService.GetByIdAsync(id);
                if (existing == null)
                {
                    return NotFound();
                }

                systemSetting.UpdatedAt = DateTime.UtcNow;
                systemSetting.UpdatedByAppUserId = userId;
                await _systemSettingService.UpdateAsync(systemSetting);
                return RedirectToAction(nameof(Index));
            }

            return View(await BuildEditViewModelAsync(systemSetting));
        }

        // GET: SystemSettings/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemSetting = await _systemSettingService.GetByIdAsync(id.Value);
            if (systemSetting == null)
            {
                return NotFound();
            }

            return View(systemSetting);
        }

        // POST: SystemSettings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _systemSettingService.RemoveAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<SystemSettingEditViewModel> BuildEditViewModelAsync(SystemSetting systemSetting)
        {
            var users = await _appUserService.GetAllAsync();

            return new SystemSettingEditViewModel
            {
                SystemSetting = systemSetting,
                UpdatedByUserOptions = users
                    .Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(
                        $"{u.FirstName} {u.LastName}".Trim(),
                        u.Id.ToString(),
                        u.Id == systemSetting.UpdatedByAppUserId))
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
