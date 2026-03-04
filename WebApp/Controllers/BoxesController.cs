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
    public class BoxesController : Controller
    {
        private readonly AppDbContext _context;

        public BoxesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Boxes
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Boxes.Include(b => b.Company).Include(b => b.CreatedByAppUser);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Boxes/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var box = await _context.Boxes
                .Include(b => b.Company)
                .Include(b => b.CreatedByAppUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (box == null)
            {
                return NotFound();
            }

            return View(box);
        }

        // GET: Boxes/Create
        public IActionResult Create()
        {
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "ContactEmail");
            ViewData["CreatedByAppUserId"] = new SelectList(_context.Users, "Id", "FirstName");
            return View();
        }

        // POST: Boxes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MealsCount,PeopleCount,DisplayName,IsActive,CreatedAt,UpdatedAt,DeletedAt,CreatedByAppUserId,CompanyId,Id")] Box box)
        {
            if (ModelState.IsValid)
            {
                box.Id = Guid.NewGuid();
                _context.Add(box);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "ContactEmail", box.CompanyId);
            ViewData["CreatedByAppUserId"] = new SelectList(_context.Users, "Id", "FirstName", box.CreatedByAppUserId);
            return View(box);
        }

        // GET: Boxes/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var box = await _context.Boxes.FindAsync(id);
            if (box == null)
            {
                return NotFound();
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "ContactEmail", box.CompanyId);
            ViewData["CreatedByAppUserId"] = new SelectList(_context.Users, "Id", "FirstName", box.CreatedByAppUserId);
            return View(box);
        }

        // POST: Boxes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("MealsCount,PeopleCount,DisplayName,IsActive,CreatedAt,UpdatedAt,DeletedAt,CreatedByAppUserId,CompanyId,Id")] Box box)
        {
            if (id != box.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(box);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoxExists(box.Id))
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
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "ContactEmail", box.CompanyId);
            ViewData["CreatedByAppUserId"] = new SelectList(_context.Users, "Id", "FirstName", box.CreatedByAppUserId);
            return View(box);
        }

        // GET: Boxes/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var box = await _context.Boxes
                .Include(b => b.Company)
                .Include(b => b.CreatedByAppUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (box == null)
            {
                return NotFound();
            }

            return View(box);
        }

        // POST: Boxes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var box = await _context.Boxes.FindAsync(id);
            if (box != null)
            {
                _context.Boxes.Remove(box);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BoxExists(Guid id)
        {
            return _context.Boxes.Any(e => e.Id == id);
        }
    }
}
