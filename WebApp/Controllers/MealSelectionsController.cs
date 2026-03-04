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
    public class MealSelectionsController : Controller
    {
        private readonly AppDbContext _context;

        public MealSelectionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: MealSelections
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.MealSelections.Include(m => m.MealSubscription).Include(m => m.Recipe).Include(m => m.WeeklyMenu);
            return View(await appDbContext.ToListAsync());
        }

        // GET: MealSelections/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mealSelection = await _context.MealSelections
                .Include(m => m.MealSubscription)
                .Include(m => m.Recipe)
                .Include(m => m.WeeklyMenu)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mealSelection == null)
            {
                return NotFound();
            }

            return View(mealSelection);
        }

        // GET: MealSelections/Create
        public IActionResult Create()
        {
            ViewData["MealSubscriptionId"] = new SelectList(_context.MealSubscriptions, "Id", "Id");
            ViewData["RecipeId"] = new SelectList(_context.Recipes, "Id", "Name");
            ViewData["WeeklyMenuId"] = new SelectList(_context.WeeklyMenus, "Id", "Id");
            return View();
        }

        // POST: MealSelections/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SelectedAutomatically,SelectedAt,LockedAt,CreatedAt,UpdatedAt,DeletedAt,MealSubscriptionId,WeeklyMenuId,RecipeId,Id")] MealSelection mealSelection)
        {
            if (ModelState.IsValid)
            {
                mealSelection.Id = Guid.NewGuid();
                _context.Add(mealSelection);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MealSubscriptionId"] = new SelectList(_context.MealSubscriptions, "Id", "Id", mealSelection.MealSubscriptionId);
            ViewData["RecipeId"] = new SelectList(_context.Recipes, "Id", "Name", mealSelection.RecipeId);
            ViewData["WeeklyMenuId"] = new SelectList(_context.WeeklyMenus, "Id", "Id", mealSelection.WeeklyMenuId);
            return View(mealSelection);
        }

        // GET: MealSelections/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mealSelection = await _context.MealSelections.FindAsync(id);
            if (mealSelection == null)
            {
                return NotFound();
            }
            ViewData["MealSubscriptionId"] = new SelectList(_context.MealSubscriptions, "Id", "Id", mealSelection.MealSubscriptionId);
            ViewData["RecipeId"] = new SelectList(_context.Recipes, "Id", "Name", mealSelection.RecipeId);
            ViewData["WeeklyMenuId"] = new SelectList(_context.WeeklyMenus, "Id", "Id", mealSelection.WeeklyMenuId);
            return View(mealSelection);
        }

        // POST: MealSelections/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("SelectedAutomatically,SelectedAt,LockedAt,CreatedAt,UpdatedAt,DeletedAt,MealSubscriptionId,WeeklyMenuId,RecipeId,Id")] MealSelection mealSelection)
        {
            if (id != mealSelection.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mealSelection);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MealSelectionExists(mealSelection.Id))
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
            ViewData["MealSubscriptionId"] = new SelectList(_context.MealSubscriptions, "Id", "Id", mealSelection.MealSubscriptionId);
            ViewData["RecipeId"] = new SelectList(_context.Recipes, "Id", "Name", mealSelection.RecipeId);
            ViewData["WeeklyMenuId"] = new SelectList(_context.WeeklyMenus, "Id", "Id", mealSelection.WeeklyMenuId);
            return View(mealSelection);
        }

        // GET: MealSelections/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mealSelection = await _context.MealSelections
                .Include(m => m.MealSubscription)
                .Include(m => m.Recipe)
                .Include(m => m.WeeklyMenu)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mealSelection == null)
            {
                return NotFound();
            }

            return View(mealSelection);
        }

        // POST: MealSelections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var mealSelection = await _context.MealSelections.FindAsync(id);
            if (mealSelection != null)
            {
                _context.MealSelections.Remove(mealSelection);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MealSelectionExists(Guid id)
        {
            return _context.MealSelections.Any(e => e.Id == id);
        }
    }
}
