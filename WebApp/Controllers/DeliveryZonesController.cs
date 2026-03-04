using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain.Delivery;

namespace WebApp.Controllers
{
    public class DeliveryZonesController : Controller
    {
        private readonly AppDbContext _context;

        public DeliveryZonesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: DeliveryZones
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.DeliveryZones.Include(d => d.Company).Include(d => d.CreatedByAppUser);
            return View(await appDbContext.ToListAsync());
        }

        // GET: DeliveryZones/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deliveryZone = await _context.DeliveryZones
                .Include(d => d.Company)
                .Include(d => d.CreatedByAppUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deliveryZone == null)
            {
                return NotFound();
            }

            return View(deliveryZone);
        }

        // GET: DeliveryZones/Create
        public IActionResult Create()
        {
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "ContactEmail");
            ViewData["CreatedByAppUserId"] = new SelectList(_context.Users, "Id", "FirstName");
            return View();
        }

        // POST: DeliveryZones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,IsActive,CreatedAt,UpdatedAt,DeletedAt,CompanyId,CreatedByAppUserId,Id")] DeliveryZone deliveryZone)
        {
            if (ModelState.IsValid)
            {
                deliveryZone.Id = Guid.NewGuid();
                _context.Add(deliveryZone);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "ContactEmail", deliveryZone.CompanyId);
            ViewData["CreatedByAppUserId"] = new SelectList(_context.Users, "Id", "FirstName", deliveryZone.CreatedByAppUserId);
            return View(deliveryZone);
        }

        // GET: DeliveryZones/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deliveryZone = await _context.DeliveryZones.FindAsync(id);
            if (deliveryZone == null)
            {
                return NotFound();
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "ContactEmail", deliveryZone.CompanyId);
            ViewData["CreatedByAppUserId"] = new SelectList(_context.Users, "Id", "FirstName", deliveryZone.CreatedByAppUserId);
            return View(deliveryZone);
        }

        // POST: DeliveryZones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Name,Description,IsActive,CreatedAt,UpdatedAt,DeletedAt,CompanyId,CreatedByAppUserId,Id")] DeliveryZone deliveryZone)
        {
            if (id != deliveryZone.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(deliveryZone);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeliveryZoneExists(deliveryZone.Id))
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
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "ContactEmail", deliveryZone.CompanyId);
            ViewData["CreatedByAppUserId"] = new SelectList(_context.Users, "Id", "FirstName", deliveryZone.CreatedByAppUserId);
            return View(deliveryZone);
        }

        // GET: DeliveryZones/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deliveryZone = await _context.DeliveryZones
                .Include(d => d.Company)
                .Include(d => d.CreatedByAppUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deliveryZone == null)
            {
                return NotFound();
            }

            return View(deliveryZone);
        }

        // POST: DeliveryZones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var deliveryZone = await _context.DeliveryZones.FindAsync(id);
            if (deliveryZone != null)
            {
                _context.DeliveryZones.Remove(deliveryZone);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DeliveryZoneExists(Guid id)
        {
            return _context.DeliveryZones.Any(e => e.Id == id);
        }
    }
}
