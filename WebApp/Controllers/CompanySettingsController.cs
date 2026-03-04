using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain.Core;

namespace WebApp.Controllers
{
    public class CompanySettingsController : Controller
    {
        private readonly AppDbContext _context;

        public CompanySettingsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: CompanySettings
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.CompanySettings.Include(c => c.Company).Include(c => c.UpdatedByAppUser);
            return View(await appDbContext.ToListAsync());
        }

        // GET: CompanySettings/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companySettings = await _context.CompanySettings
                .Include(c => c.Company)
                .Include(c => c.UpdatedByAppUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (companySettings == null)
            {
                return NotFound();
            }

            return View(companySettings);
        }

        // GET: CompanySettings/Create
        public IActionResult Create()
        {
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "ContactEmail");
            ViewData["UpdatedByAppUserId"] = new SelectList(_context.Users, "Id", "FirstName");
            return View();
        }

        // POST: CompanySettings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DefaultNoRepeatWeeks,SelectionDeadlineDaysBeforeDelivery,AllowAutoSelection,AllowPauseSubscription,AllowSkipWeek,MinimumSubscriptionWeeks,MaxDeliveryAttempts,AllowRedeliveryAfterFailure,ComplaintEscalationThreshold,ComplaintEscalationDaysWindow,AutoPrioritizeFreshestStock,AutoAssignEarliestSlot,UpdatedAt,CompanyId,UpdatedByAppUserId,Id")] CompanySettings companySettings)
        {
            if (ModelState.IsValid)
            {
                companySettings.Id = Guid.NewGuid();
                _context.Add(companySettings);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "ContactEmail", companySettings.CompanyId);
            ViewData["UpdatedByAppUserId"] = new SelectList(_context.Users, "Id", "FirstName", companySettings.UpdatedByAppUserId);
            return View(companySettings);
        }

        // GET: CompanySettings/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companySettings = await _context.CompanySettings.FindAsync(id);
            if (companySettings == null)
            {
                return NotFound();
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "ContactEmail", companySettings.CompanyId);
            ViewData["UpdatedByAppUserId"] = new SelectList(_context.Users, "Id", "FirstName", companySettings.UpdatedByAppUserId);
            return View(companySettings);
        }

        // POST: CompanySettings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("DefaultNoRepeatWeeks,SelectionDeadlineDaysBeforeDelivery,AllowAutoSelection,AllowPauseSubscription,AllowSkipWeek,MinimumSubscriptionWeeks,MaxDeliveryAttempts,AllowRedeliveryAfterFailure,ComplaintEscalationThreshold,ComplaintEscalationDaysWindow,AutoPrioritizeFreshestStock,AutoAssignEarliestSlot,UpdatedAt,CompanyId,UpdatedByAppUserId,Id")] CompanySettings companySettings)
        {
            if (id != companySettings.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(companySettings);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanySettingsExists(companySettings.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "ContactEmail", companySettings.CompanyId);
            ViewData["UpdatedByAppUserId"] = new SelectList(_context.Users, "Id", "FirstName", companySettings.UpdatedByAppUserId);
            return View(companySettings);
        }

        // GET: CompanySettings/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companySettings = await _context.CompanySettings
                .Include(c => c.Company)
                .Include(c => c.UpdatedByAppUser)
                .FirstOrDefaultAsync(m => m.Id == id);
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
            var companySettings = await _context.CompanySettings.FindAsync(id);
            if (companySettings != null)
            {
                _context.CompanySettings.Remove(companySettings);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompanySettingsExists(Guid id)
        {
            return _context.CompanySettings.Any(e => e.Id == id);
        }
    }
}
