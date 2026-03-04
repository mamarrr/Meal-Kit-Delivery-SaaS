using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain.Subscription;

namespace WebApp.Controllers
{
    public class PlatformSubscriptionsController : Controller
    {
        private readonly AppDbContext _context;

        public PlatformSubscriptionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: PlatformSubscriptions
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.PlatformSubscriptions.Include(p => p.Company).Include(p => p.CreatedByAppUser).Include(p => p.PlatformSubscriptionStatus).Include(p => p.PlatformSubscriptionTier);
            return View(await appDbContext.ToListAsync());
        }

        // GET: PlatformSubscriptions/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var platformSubscription = await _context.PlatformSubscriptions
                .Include(p => p.Company)
                .Include(p => p.CreatedByAppUser)
                .Include(p => p.PlatformSubscriptionStatus)
                .Include(p => p.PlatformSubscriptionTier)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (platformSubscription == null)
            {
                return NotFound();
            }

            return View(platformSubscription);
        }

        // GET: PlatformSubscriptions/Create
        public IActionResult Create()
        {
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "ContactEmail");
            ViewData["CreatedByAppUserId"] = new SelectList(_context.Users, "Id", "FirstName");
            ViewData["PlatformSubscriptionStatusId"] = new SelectList(_context.PlatformSubscriptionStatuses, "Id", "Code");
            ViewData["PlatformSubscriptionTierId"] = new SelectList(_context.PlatformSubscriptionTiers, "Id", "Code");
            return View();
        }

        // POST: PlatformSubscriptions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ValidFrom,ValidTo,CreatedAt,UpdatedAt,DeletedAt,CompanyId,PlatformSubscriptionTierId,PlatformSubscriptionStatusId,CreatedByAppUserId,Id")] PlatformSubscription platformSubscription)
        {
            if (ModelState.IsValid)
            {
                platformSubscription.Id = Guid.NewGuid();
                _context.Add(platformSubscription);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "ContactEmail", platformSubscription.CompanyId);
            ViewData["CreatedByAppUserId"] = new SelectList(_context.Users, "Id", "FirstName", platformSubscription.CreatedByAppUserId);
            ViewData["PlatformSubscriptionStatusId"] = new SelectList(_context.PlatformSubscriptionStatuses, "Id", "Code", platformSubscription.PlatformSubscriptionStatusId);
            ViewData["PlatformSubscriptionTierId"] = new SelectList(_context.PlatformSubscriptionTiers, "Id", "Code", platformSubscription.PlatformSubscriptionTierId);
            return View(platformSubscription);
        }

        // GET: PlatformSubscriptions/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var platformSubscription = await _context.PlatformSubscriptions.FindAsync(id);
            if (platformSubscription == null)
            {
                return NotFound();
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "ContactEmail", platformSubscription.CompanyId);
            ViewData["CreatedByAppUserId"] = new SelectList(_context.Users, "Id", "FirstName", platformSubscription.CreatedByAppUserId);
            ViewData["PlatformSubscriptionStatusId"] = new SelectList(_context.PlatformSubscriptionStatuses, "Id", "Code", platformSubscription.PlatformSubscriptionStatusId);
            ViewData["PlatformSubscriptionTierId"] = new SelectList(_context.PlatformSubscriptionTiers, "Id", "Code", platformSubscription.PlatformSubscriptionTierId);
            return View(platformSubscription);
        }

        // POST: PlatformSubscriptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ValidFrom,ValidTo,CreatedAt,UpdatedAt,DeletedAt,CompanyId,PlatformSubscriptionTierId,PlatformSubscriptionStatusId,CreatedByAppUserId,Id")] PlatformSubscription platformSubscription)
        {
            if (id != platformSubscription.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(platformSubscription);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlatformSubscriptionExists(platformSubscription.Id))
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
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "ContactEmail", platformSubscription.CompanyId);
            ViewData["CreatedByAppUserId"] = new SelectList(_context.Users, "Id", "FirstName", platformSubscription.CreatedByAppUserId);
            ViewData["PlatformSubscriptionStatusId"] = new SelectList(_context.PlatformSubscriptionStatuses, "Id", "Code", platformSubscription.PlatformSubscriptionStatusId);
            ViewData["PlatformSubscriptionTierId"] = new SelectList(_context.PlatformSubscriptionTiers, "Id", "Code", platformSubscription.PlatformSubscriptionTierId);
            return View(platformSubscription);
        }

        // GET: PlatformSubscriptions/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var platformSubscription = await _context.PlatformSubscriptions
                .Include(p => p.Company)
                .Include(p => p.CreatedByAppUser)
                .Include(p => p.PlatformSubscriptionStatus)
                .Include(p => p.PlatformSubscriptionTier)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (platformSubscription == null)
            {
                return NotFound();
            }

            return View(platformSubscription);
        }

        // POST: PlatformSubscriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var platformSubscription = await _context.PlatformSubscriptions.FindAsync(id);
            if (platformSubscription != null)
            {
                _context.PlatformSubscriptions.Remove(platformSubscription);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlatformSubscriptionExists(Guid id)
        {
            return _context.PlatformSubscriptions.Any(e => e.Id == id);
        }
    }
}
