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
    public class CustomerExclusionsController : Controller
    {
        private readonly AppDbContext _context;

        public CustomerExclusionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: CustomerExclusions
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.CustomerExclusions.Include(c => c.Customer).Include(c => c.Ingredient);
            return View(await appDbContext.ToListAsync());
        }

        // GET: CustomerExclusions/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerExclusion = await _context.CustomerExclusions
                .Include(c => c.Customer)
                .Include(c => c.Ingredient)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customerExclusion == null)
            {
                return NotFound();
            }

            return View(customerExclusion);
        }

        // GET: CustomerExclusions/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "AddressLine");
            ViewData["IngredientId"] = new SelectList(_context.Ingredients, "Id", "Name");
            return View();
        }

        // POST: CustomerExclusions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CreatedAt,DeletedAt,IngredientId,CustomerId,Id")] CustomerExclusion customerExclusion)
        {
            if (ModelState.IsValid)
            {
                customerExclusion.Id = Guid.NewGuid();
                _context.Add(customerExclusion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "AddressLine", customerExclusion.CustomerId);
            ViewData["IngredientId"] = new SelectList(_context.Ingredients, "Id", "Name", customerExclusion.IngredientId);
            return View(customerExclusion);
        }

        // GET: CustomerExclusions/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerExclusion = await _context.CustomerExclusions.FindAsync(id);
            if (customerExclusion == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "AddressLine", customerExclusion.CustomerId);
            ViewData["IngredientId"] = new SelectList(_context.Ingredients, "Id", "Name", customerExclusion.IngredientId);
            return View(customerExclusion);
        }

        // POST: CustomerExclusions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CreatedAt,DeletedAt,IngredientId,CustomerId,Id")] CustomerExclusion customerExclusion)
        {
            if (id != customerExclusion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customerExclusion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExclusionExists(customerExclusion.Id))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "AddressLine", customerExclusion.CustomerId);
            ViewData["IngredientId"] = new SelectList(_context.Ingredients, "Id", "Name", customerExclusion.IngredientId);
            return View(customerExclusion);
        }

        // GET: CustomerExclusions/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerExclusion = await _context.CustomerExclusions
                .Include(c => c.Customer)
                .Include(c => c.Ingredient)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customerExclusion == null)
            {
                return NotFound();
            }

            return View(customerExclusion);
        }

        // POST: CustomerExclusions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var customerExclusion = await _context.CustomerExclusions.FindAsync(id);
            if (customerExclusion != null)
            {
                _context.CustomerExclusions.Remove(customerExclusion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExclusionExists(Guid id)
        {
            return _context.CustomerExclusions.Any(e => e.Id == id);
        }
    }
}
