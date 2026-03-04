using App.Contracts.BLL.Identity;
using App.Contracts.BLL.Support;
using App.Domain.Support;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Controllers
{
    [Authorize]
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
            await LoadUsersAsync();
            return View();
        }

        // POST: SystemSettings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Category,Key,Value,ValueType,IsSensitive,UpdatedAt,UpdatedByAppUserId,Id")] SystemSetting systemSetting)
        {
            if (ModelState.IsValid)
            {
                systemSetting.UpdatedAt = DateTime.UtcNow;
                await _systemSettingService.AddAsync(systemSetting);
                return RedirectToAction(nameof(Index));
            }
            await LoadUsersAsync(systemSetting.UpdatedByAppUserId);
            return View(systemSetting);
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
            await LoadUsersAsync(systemSetting.UpdatedByAppUserId);
            return View(systemSetting);
        }

        // POST: SystemSettings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Category,Key,Value,ValueType,IsSensitive,UpdatedAt,UpdatedByAppUserId,Id")] SystemSetting systemSetting)
        {
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
                await _systemSettingService.UpdateAsync(systemSetting);
                return RedirectToAction(nameof(Index));
            }
            await LoadUsersAsync(systemSetting.UpdatedByAppUserId);
            return View(systemSetting);
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

        private async Task LoadUsersAsync(Guid? selectedUserId = null)
        {
            var users = await _appUserService.GetAllAsync();
            ViewData["UpdatedByAppUserId"] = new SelectList(users, "Id", "FirstName", selectedUserId);
        }
    }
}
