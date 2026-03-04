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
    public class CustomerPreferencesController : Controller
    {
        private readonly AppDbContext _context;

        public CustomerPreferencesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: CustomerPreferences
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.CustomerPreferences.Include(c => c.Customer).Include(c => c.DietaryCategory);
            return View(await appDbContext.ToListAsync());
        }

        // GET: CustomerPreferences/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerPreference = await _context.CustomerPreferences
                .Include(c => c.Customer)
                .Include(c => c.DietaryCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customerPreference == null)
            {
                return NotFound();
            }

            return View(customerPreference);
        }

        // GET: CustomerPreferences/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "AddressLine");
            ViewData["DietaryCategoryId"] = new SelectList(_context.DietaryCategories, "Id", "Code");
            return View();
        }

        // POST: CustomerPreferences/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CreatedAt,DeletedAt,DietaryCategoryId,CustomerId,Id")] CustomerPreference customerPreference)
        {
            if (ModelState.IsValid)
            {
                customerPreference.Id = Guid.NewGuid();
                _context.Add(customerPreference);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "AddressLine", customerPreference.CustomerId);
            ViewData["DietaryCategoryId"] = new SelectList(_context.DietaryCategories, "Id", "Code", customerPreference.DietaryCategoryId);
            return View(customerPreference);
        }

        // GET: CustomerPreferences/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerPreference = await _context.CustomerPreferences.FindAsync(id);
            if (customerPreference == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "AddressLine", customerPreference.CustomerId);
            ViewData["DietaryCategoryId"] = new SelectList(_context.DietaryCategories, "Id", "Code", customerPreference.DietaryCategoryId);
            return View(customerPreference);
        }

        // POST: CustomerPreferences/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CreatedAt,DeletedAt,DietaryCategoryId,CustomerId,Id")] CustomerPreference customerPreference)
        {
            if (id != customerPreference.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customerPreference);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerPreferenceExists(customerPreference.Id))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "AddressLine", customerPreference.CustomerId);
            ViewData["DietaryCategoryId"] = new SelectList(_context.DietaryCategories, "Id", "Code", customerPreference.DietaryCategoryId);
            return View(customerPreference);
        }

        // GET: CustomerPreferences/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerPreference = await _context.CustomerPreferences
                .Include(c => c.Customer)
                .Include(c => c.DietaryCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customerPreference == null)
            {
                return NotFound();
            }

            return View(customerPreference);
        }

        // POST: CustomerPreferences/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var customerPreference = await _context.CustomerPreferences.FindAsync(id);
            if (customerPreference != null)
            {
                _context.CustomerPreferences.Remove(customerPreference);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerPreferenceExists(Guid id)
        {
            return _context.CustomerPreferences.Any(e => e.Id == id);
        }
    }
}
