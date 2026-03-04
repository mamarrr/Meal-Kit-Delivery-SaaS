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
    public class CompanyUsersController : Controller
    {
        private readonly AppDbContext _context;

        public CompanyUsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: CompanyUsers
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.CompanyAppUsers.Include(c => c.AppUser).Include(c => c.Company).Include(c => c.CompanyRole).Include(c => c.CreatedByAppUser);
            return View(await appDbContext.ToListAsync());
        }

        // GET: CompanyUsers/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyAppUser = await _context.CompanyAppUsers
                .Include(c => c.AppUser)
                .Include(c => c.Company)
                .Include(c => c.CompanyRole)
                .Include(c => c.CreatedByAppUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (companyAppUser == null)
            {
                return NotFound();
            }

            return View(companyAppUser);
        }

        // GET: CompanyUsers/Create
        public IActionResult Create()
        {
            ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "FirstName");
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "ContactEmail");
            ViewData["CompanyRoleId"] = new SelectList(_context.CompanyRoles, "Id", "Code");
            ViewData["CreatedByAppUserId"] = new SelectList(_context.Users, "Id", "FirstName");
            return View();
        }

        // POST: CompanyUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IsOwner,IsActive,CreatedAt,UpdatedAt,DeletedAt,CompanyId,AppUserId,CompanyRoleId,CreatedByAppUserId,Id")] CompanyAppUser companyAppUser)
        {
            if (ModelState.IsValid)
            {
                companyAppUser.Id = Guid.NewGuid();
                _context.Add(companyAppUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "FirstName", companyAppUser.AppUserId);
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "ContactEmail", companyAppUser.CompanyId);
            ViewData["CompanyRoleId"] = new SelectList(_context.CompanyRoles, "Id", "Code", companyAppUser.CompanyRoleId);
            ViewData["CreatedByAppUserId"] = new SelectList(_context.Users, "Id", "FirstName", companyAppUser.CreatedByAppUserId);
            return View(companyAppUser);
        }

        // GET: CompanyUsers/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyAppUser = await _context.CompanyAppUsers.FindAsync(id);
            if (companyAppUser == null)
            {
                return NotFound();
            }
            ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "FirstName", companyAppUser.AppUserId);
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "ContactEmail", companyAppUser.CompanyId);
            ViewData["CompanyRoleId"] = new SelectList(_context.CompanyRoles, "Id", "Code", companyAppUser.CompanyRoleId);
            ViewData["CreatedByAppUserId"] = new SelectList(_context.Users, "Id", "FirstName", companyAppUser.CreatedByAppUserId);
            return View(companyAppUser);
        }

        // POST: CompanyUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("IsOwner,IsActive,CreatedAt,UpdatedAt,DeletedAt,CompanyId,AppUserId,CompanyRoleId,CreatedByAppUserId,Id")] CompanyAppUser companyAppUser)
        {
            if (id != companyAppUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(companyAppUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyAppUserExists(companyAppUser.Id))
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
            ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "FirstName", companyAppUser.AppUserId);
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "ContactEmail", companyAppUser.CompanyId);
            ViewData["CompanyRoleId"] = new SelectList(_context.CompanyRoles, "Id", "Code", companyAppUser.CompanyRoleId);
            ViewData["CreatedByAppUserId"] = new SelectList(_context.Users, "Id", "FirstName", companyAppUser.CreatedByAppUserId);
            return View(companyAppUser);
        }

        // GET: CompanyUsers/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyAppUser = await _context.CompanyAppUsers
                .Include(c => c.AppUser)
                .Include(c => c.Company)
                .Include(c => c.CompanyRole)
                .Include(c => c.CreatedByAppUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (companyAppUser == null)
            {
                return NotFound();
            }

            return View(companyAppUser);
        }

        // POST: CompanyUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var companyAppUser = await _context.CompanyAppUsers.FindAsync(id);
            if (companyAppUser != null)
            {
                _context.CompanyAppUsers.Remove(companyAppUser);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyAppUserExists(Guid id)
        {
            return _context.CompanyAppUsers.Any(e => e.Id == id);
        }
    }
}
