using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain.Menu;

namespace WebApp.Controllers
{
    public class WeeklyMenusController : Controller
    {
        private readonly AppDbContext _context;

        public WeeklyMenusController(AppDbContext context)
        {
            _context = context;
        }

        // GET: WeeklyMenus
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.WeeklyMenus.Include(w => w.Company).Include(w => w.CreatedByAppUser);
            return View(await appDbContext.ToListAsync());
        }

        // GET: WeeklyMenus/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weeklyMenu = await _context.WeeklyMenus
                .Include(w => w.Company)
                .Include(w => w.CreatedByAppUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (weeklyMenu == null)
            {
                return NotFound();
            }

            return View(weeklyMenu);
        }

        // GET: WeeklyMenus/Create
        public IActionResult Create()
        {
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "ContactEmail");
            ViewData["CreatedByAppUserId"] = new SelectList(_context.Users, "Id", "FirstName");
            return View();
        }

        // POST: WeeklyMenus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WeekStartDate,SelectionDeadlineAt,TotalRecipes,IsPublished,PublishedAt,CreatedAt,UpdatedAt,DeletedAt,CompanyId,CreatedByAppUserId,Id")] WeeklyMenu weeklyMenu)
        {
            if (ModelState.IsValid)
            {
                weeklyMenu.Id = Guid.NewGuid();
                _context.Add(weeklyMenu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "ContactEmail", weeklyMenu.CompanyId);
            ViewData["CreatedByAppUserId"] = new SelectList(_context.Users, "Id", "FirstName", weeklyMenu.CreatedByAppUserId);
            return View(weeklyMenu);
        }

        // GET: WeeklyMenus/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weeklyMenu = await _context.WeeklyMenus.FindAsync(id);
            if (weeklyMenu == null)
            {
                return NotFound();
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "ContactEmail", weeklyMenu.CompanyId);
            ViewData["CreatedByAppUserId"] = new SelectList(_context.Users, "Id", "FirstName", weeklyMenu.CreatedByAppUserId);
            return View(weeklyMenu);
        }

        // POST: WeeklyMenus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("WeekStartDate,SelectionDeadlineAt,TotalRecipes,IsPublished,PublishedAt,CreatedAt,UpdatedAt,DeletedAt,CompanyId,CreatedByAppUserId,Id")] WeeklyMenu weeklyMenu)
        {
            if (id != weeklyMenu.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(weeklyMenu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WeeklyMenuExists(weeklyMenu.Id))
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
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "ContactEmail", weeklyMenu.CompanyId);
            ViewData["CreatedByAppUserId"] = new SelectList(_context.Users, "Id", "FirstName", weeklyMenu.CreatedByAppUserId);
            return View(weeklyMenu);
        }

        // GET: WeeklyMenus/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weeklyMenu = await _context.WeeklyMenus
                .Include(w => w.Company)
                .Include(w => w.CreatedByAppUser)
                .FirstOrDefaultAsync(m => m.Id == id);
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
            var weeklyMenu = await _context.WeeklyMenus.FindAsync(id);
            if (weeklyMenu != null)
            {
                _context.WeeklyMenus.Remove(weeklyMenu);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WeeklyMenuExists(Guid id)
        {
            return _context.WeeklyMenus.Any(e => e.Id == id);
        }
    }
}
