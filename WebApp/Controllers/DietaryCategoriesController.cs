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
    public class DietaryCategoriesController : Controller
    {
        private readonly AppDbContext _context;

        public DietaryCategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: DietaryCategories
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.DietaryCategories.Include(d => d.Company).Include(d => d.CreatedByAppUser);
            return View(await appDbContext.ToListAsync());
        }

        // GET: DietaryCategories/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dietaryCategory = await _context.DietaryCategories
                .Include(d => d.Company)
                .Include(d => d.CreatedByAppUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dietaryCategory == null)
            {
                return NotFound();
            }

            return View(dietaryCategory);
        }

        // GET: DietaryCategories/Create
        public IActionResult Create()
        {
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "ContactEmail");
            ViewData["CreatedByAppUserId"] = new SelectList(_context.Users, "Id", "FirstName");
            return View();
        }

        // POST: DietaryCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Code,Name,IsActive,CreatedAt,UpdatedAt,DeletedAt,CompanyId,CreatedByAppUserId,Id")] DietaryCategory dietaryCategory)
        {
            if (ModelState.IsValid)
            {
                dietaryCategory.Id = Guid.NewGuid();
                _context.Add(dietaryCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "ContactEmail", dietaryCategory.CompanyId);
            ViewData["CreatedByAppUserId"] = new SelectList(_context.Users, "Id", "FirstName", dietaryCategory.CreatedByAppUserId);
            return View(dietaryCategory);
        }

        // GET: DietaryCategories/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dietaryCategory = await _context.DietaryCategories.FindAsync(id);
            if (dietaryCategory == null)
            {
                return NotFound();
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "ContactEmail", dietaryCategory.CompanyId);
            ViewData["CreatedByAppUserId"] = new SelectList(_context.Users, "Id", "FirstName", dietaryCategory.CreatedByAppUserId);
            return View(dietaryCategory);
        }

        // POST: DietaryCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Code,Name,IsActive,CreatedAt,UpdatedAt,DeletedAt,CompanyId,CreatedByAppUserId,Id")] DietaryCategory dietaryCategory)
        {
            if (id != dietaryCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dietaryCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DietaryCategoryExists(dietaryCategory.Id))
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
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "ContactEmail", dietaryCategory.CompanyId);
            ViewData["CreatedByAppUserId"] = new SelectList(_context.Users, "Id", "FirstName", dietaryCategory.CreatedByAppUserId);
            return View(dietaryCategory);
        }

        // GET: DietaryCategories/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dietaryCategory = await _context.DietaryCategories
                .Include(d => d.Company)
                .Include(d => d.CreatedByAppUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dietaryCategory == null)
            {
                return NotFound();
            }

            return View(dietaryCategory);
        }

        // POST: DietaryCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var dietaryCategory = await _context.DietaryCategories.FindAsync(id);
            if (dietaryCategory != null)
            {
                _context.DietaryCategories.Remove(dietaryCategory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DietaryCategoryExists(Guid id)
        {
            return _context.DietaryCategories.Any(e => e.Id == id);
        }
    }
}
