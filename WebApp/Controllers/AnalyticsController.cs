using App.Contracts.BLL.Support;
using App.Domain.Support;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    [Authorize(Policy = "SystemAdmin")]
    public class AnalyticsController : Controller
    {
        private readonly ISystemAnalyticsSnapshotService _systemAnalyticsSnapshotService;

        public AnalyticsController(ISystemAnalyticsSnapshotService systemAnalyticsSnapshotService)
        {
            _systemAnalyticsSnapshotService = systemAnalyticsSnapshotService;
        }

        // GET: Analytics
        public async Task<IActionResult> Index()
        {
            return View(await _systemAnalyticsSnapshotService.GetAllAsync());
        }

        // GET: Analytics/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemAnalyticsSnapshot = await _systemAnalyticsSnapshotService.GetByIdAsync(id.Value);
            if (systemAnalyticsSnapshot == null)
            {
                return NotFound();
            }

            return View(systemAnalyticsSnapshot);
        }

        // GET: Analytics/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Analytics/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CapturedAt,ActiveCompanies,ActiveSubscribers,WeeklyDeliveries,OpenSupportTickets,Id")] SystemAnalyticsSnapshot systemAnalyticsSnapshot)
        {
            if (ModelState.IsValid)
            {
                await _systemAnalyticsSnapshotService.AddAsync(systemAnalyticsSnapshot);
                return RedirectToAction(nameof(Index));
            }
            return View(systemAnalyticsSnapshot);
        }

        // GET: Analytics/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemAnalyticsSnapshot = await _systemAnalyticsSnapshotService.GetByIdAsync(id.Value);
            if (systemAnalyticsSnapshot == null)
            {
                return NotFound();
            }
            return View(systemAnalyticsSnapshot);
        }

        // POST: Analytics/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CapturedAt,ActiveCompanies,ActiveSubscribers,WeeklyDeliveries,OpenSupportTickets,Id")] SystemAnalyticsSnapshot systemAnalyticsSnapshot)
        {
            if (id != systemAnalyticsSnapshot.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existing = await _systemAnalyticsSnapshotService.GetByIdAsync(id);
                if (existing == null)
                {
                    return NotFound();
                }

                await _systemAnalyticsSnapshotService.UpdateAsync(systemAnalyticsSnapshot);
                return RedirectToAction(nameof(Index));
            }
            return View(systemAnalyticsSnapshot);
        }

        // GET: Analytics/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemAnalyticsSnapshot = await _systemAnalyticsSnapshotService.GetByIdAsync(id.Value);
            if (systemAnalyticsSnapshot == null)
            {
                return NotFound();
            }

            return View(systemAnalyticsSnapshot);
        }

        // POST: Analytics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _systemAnalyticsSnapshotService.RemoveAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
