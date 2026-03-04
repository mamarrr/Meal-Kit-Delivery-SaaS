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
    public class PlatformSubscriptionTiersController : Controller
    {
        private readonly AppDbContext _context;

        public PlatformSubscriptionTiersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: PlatformSubscriptionTiers
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.PlatformSubscriptionTiers.Include(p => p.CreatedByAppUser);
            return View(await appDbContext.ToListAsync());
        }

        // GET: PlatformSubscriptionTiers/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var platformSubscriptionTier = await _context.PlatformSubscriptionTiers
                .Include(p => p.CreatedByAppUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (platformSubscriptionTier == null)
            {
                return NotFound();
            }

            return View(platformSubscriptionTier);
        }

        // GET: PlatformSubscriptionTiers/Create
        public IActionResult Create()
        {
            ViewData["CreatedByAppUserId"] = new SelectList(_context.Users, "Id", "FirstName");
            return View();
        }

        // POST: PlatformSubscriptionTiers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Code,Name,MaxZones,MaxSubscribers,MaxEmployees,MaxRecipes,IsActive,CreatedAt,UpdatedAt,DeletedAt,CreatedByAppUserId,Id")] PlatformSubscriptionTier platformSubscriptionTier)
        {
            if (ModelState.IsValid)
            {
                platformSubscriptionTier.Id = Guid.NewGuid();
                _context.Add(platformSubscriptionTier);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatedByAppUserId"] = new SelectList(_context.Users, "Id", "FirstName", platformSubscriptionTier.CreatedByAppUserId);
            return View(platformSubscriptionTier);
        }

        // GET: PlatformSubscriptionTiers/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var platformSubscriptionTier = await _context.PlatformSubscriptionTiers.FindAsync(id);
            if (platformSubscriptionTier == null)
            {
                return NotFound();
            }
            ViewData["CreatedByAppUserId"] = new SelectList(_context.Users, "Id", "FirstName", platformSubscriptionTier.CreatedByAppUserId);
            return View(platformSubscriptionTier);
        }

        // POST: PlatformSubscriptionTiers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Code,Name,MaxZones,MaxSubscribers,MaxEmployees,MaxRecipes,IsActive,CreatedAt,UpdatedAt,DeletedAt,CreatedByAppUserId,Id")] PlatformSubscriptionTier platformSubscriptionTier)
        {
            if (id != platformSubscriptionTier.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(platformSubscriptionTier);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlatformSubscriptionTierExists(platformSubscriptionTier.Id))
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
            ViewData["CreatedByAppUserId"] = new SelectList(_context.Users, "Id", "FirstName", platformSubscriptionTier.CreatedByAppUserId);
            return View(platformSubscriptionTier);
        }

        // GET: PlatformSubscriptionTiers/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var platformSubscriptionTier = await _context.PlatformSubscriptionTiers
                .Include(p => p.CreatedByAppUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (platformSubscriptionTier == null)
            {
                return NotFound();
            }

            return View(platformSubscriptionTier);
        }

        // POST: PlatformSubscriptionTiers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var platformSubscriptionTier = await _context.PlatformSubscriptionTiers.FindAsync(id);
            if (platformSubscriptionTier != null)
            {
                _context.PlatformSubscriptionTiers.Remove(platformSubscriptionTier);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlatformSubscriptionTierExists(Guid id)
        {
            return _context.PlatformSubscriptionTiers.Any(e => e.Id == id);
        }
    }
}
