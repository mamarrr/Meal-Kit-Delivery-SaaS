using App.Contracts.BLL.Menu;
using App.Domain.Menu;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApp.Controllers
{
    [Authorize]
    public class WeeklyMenusController : Controller
    {
        private readonly IWeeklyMenuService _weeklyMenuService;

        public WeeklyMenusController(IWeeklyMenuService weeklyMenuService)
        {
            _weeklyMenuService = weeklyMenuService;
        }

        // GET: WeeklyMenus
        public async Task<IActionResult> Index()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            return View(await _weeklyMenuService.GetAllByCompanyIdAsync(companyId.Value));
        }

        // GET: WeeklyMenus/Details/5
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

            var weeklyMenu = await _weeklyMenuService.GetByIdAsync(id.Value, companyId.Value);
            if (weeklyMenu == null)
            {
                return NotFound();
            }

            return View(weeklyMenu);
        }

        // GET: WeeklyMenus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WeeklyMenus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WeekStartDate,SelectionDeadlineAt,TotalRecipes,IsPublished,PublishedAt")] WeeklyMenu weeklyMenu)
        {
            var companyId = GetCurrentCompanyId();
            var userId = GetCurrentUserId();
            if (companyId == null || userId == null)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                weeklyMenu.CreatedAt = DateTime.UtcNow;
                weeklyMenu.UpdatedAt = null;
                weeklyMenu.DeletedAt = null;
                weeklyMenu.CreatedByAppUserId = userId.Value;

                await _weeklyMenuService.AddAsync(weeklyMenu, companyId.Value);
                return RedirectToAction(nameof(Index));
            }

            return View(weeklyMenu);
        }

        // GET: WeeklyMenus/Edit/5
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

            var weeklyMenu = await _weeklyMenuService.GetByIdAsync(id.Value, companyId.Value);
            if (weeklyMenu == null)
            {
                return NotFound();
            }

            return View(weeklyMenu);
        }

        // POST: WeeklyMenus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,WeekStartDate,SelectionDeadlineAt,TotalRecipes,IsPublished,PublishedAt")] WeeklyMenu weeklyMenu)
        {
            if (id != weeklyMenu.Id)
            {
                return NotFound();
            }

            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                var existing = await _weeklyMenuService.GetByIdAsync(id, companyId.Value);
                if (existing == null)
                {
                    return NotFound();
                }

                weeklyMenu.CompanyId = companyId.Value;
                weeklyMenu.CreatedByAppUserId = existing.CreatedByAppUserId;
                weeklyMenu.CreatedAt = existing.CreatedAt;
                weeklyMenu.UpdatedAt = DateTime.UtcNow;
                weeklyMenu.DeletedAt = existing.DeletedAt;

                await _weeklyMenuService.UpdateAsync(weeklyMenu, companyId.Value);
                return RedirectToAction(nameof(Index));
            }

            return View(weeklyMenu);
        }

        // GET: WeeklyMenus/Delete/5
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

            var weeklyMenu = await _weeklyMenuService.GetByIdAsync(id.Value, companyId.Value);
            if (weeklyMenu == null)
            {
                return NotFound();
            }

            return View(weeklyMenu);
        }

        // POST: WeeklyMenus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            await _weeklyMenuService.RemoveAsync(id, companyId.Value);
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
