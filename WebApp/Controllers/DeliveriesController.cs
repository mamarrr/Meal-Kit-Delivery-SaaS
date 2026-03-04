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
    public class DeliveriesController : Controller
    {
        private readonly AppDbContext _context;

        public DeliveriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Deliveries
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Deliveries.Include(d => d.Box).Include(d => d.Company).Include(d => d.CreatedByAppUser).Include(d => d.Customer).Include(d => d.DeliveryStatus).Include(d => d.DeliveryWindow).Include(d => d.DeliveryZone).Include(d => d.MealSelection).Include(d => d.MealSubscription).Include(d => d.WeeklyMenu);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Deliveries/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var delivery = await _context.Deliveries
                .Include(d => d.Box)
                .Include(d => d.Company)
                .Include(d => d.CreatedByAppUser)
                .Include(d => d.Customer)
                .Include(d => d.DeliveryStatus)
                .Include(d => d.DeliveryWindow)
                .Include(d => d.DeliveryZone)
                .Include(d => d.MealSelection)
                .Include(d => d.MealSubscription)
                .Include(d => d.WeeklyMenu)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (delivery == null)
            {
                return NotFound();
            }

            return View(delivery);
        }

        // GET: Deliveries/Create
        public IActionResult Create()
        {
            ViewData["BoxId"] = new SelectList(_context.Boxes, "Id", "DisplayName");
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "ContactEmail");
            ViewData["CreatedByAppUserId"] = new SelectList(_context.Users, "Id", "FirstName");
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "AddressLine");
            ViewData["DeliveryStatusId"] = new SelectList(_context.DeliveryStatuses, "Id", "Code");
            ViewData["DeliveryWindowId"] = new SelectList(_context.DeliveryWindows, "Id", "Id");
            ViewData["DeliveryZoneId"] = new SelectList(_context.DeliveryZones, "Id", "Name");
            ViewData["MealSelectionId"] = new SelectList(_context.MealSelections, "Id", "Id");
            ViewData["MealSubscriptionId"] = new SelectList(_context.MealSubscriptions, "Id", "Id");
            ViewData["WeeklyMenuId"] = new SelectList(_context.WeeklyMenus, "Id", "Id");
            return View();
        }

        // POST: Deliveries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ScheduledTime,DeliveredAt,FailureReason,AddressLine,City,PostalCode,Country,CreatedAt,UpdatedAt,DeletedAt,DeliveryStatusId,CompanyId,CustomerId,WeeklyMenuId,DeliveryZoneId,DeliveryWindowId,BoxId,MealSelectionId,MealSubscriptionId,CreatedByAppUserId,Id")] Delivery delivery)
        {
            if (ModelState.IsValid)
            {
                delivery.Id = Guid.NewGuid();
                _context.Add(delivery);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BoxId"] = new SelectList(_context.Boxes, "Id", "DisplayName", delivery.BoxId);
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "ContactEmail", delivery.CompanyId);
            ViewData["CreatedByAppUserId"] = new SelectList(_context.Users, "Id", "FirstName", delivery.CreatedByAppUserId);
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "AddressLine", delivery.CustomerId);
            ViewData["DeliveryStatusId"] = new SelectList(_context.DeliveryStatuses, "Id", "Code", delivery.DeliveryStatusId);
            ViewData["DeliveryWindowId"] = new SelectList(_context.DeliveryWindows, "Id", "Id", delivery.DeliveryWindowId);
            ViewData["DeliveryZoneId"] = new SelectList(_context.DeliveryZones, "Id", "Name", delivery.DeliveryZoneId);
            ViewData["MealSelectionId"] = new SelectList(_context.MealSelections, "Id", "Id", delivery.MealSelectionId);
            ViewData["MealSubscriptionId"] = new SelectList(_context.MealSubscriptions, "Id", "Id", delivery.MealSubscriptionId);
            ViewData["WeeklyMenuId"] = new SelectList(_context.WeeklyMenus, "Id", "Id", delivery.WeeklyMenuId);
            return View(delivery);
        }

        // GET: Deliveries/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var delivery = await _context.Deliveries.FindAsync(id);
            if (delivery == null)
            {
                return NotFound();
            }
            ViewData["BoxId"] = new SelectList(_context.Boxes, "Id", "DisplayName", delivery.BoxId);
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "ContactEmail", delivery.CompanyId);
            ViewData["CreatedByAppUserId"] = new SelectList(_context.Users, "Id", "FirstName", delivery.CreatedByAppUserId);
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "AddressLine", delivery.CustomerId);
            ViewData["DeliveryStatusId"] = new SelectList(_context.DeliveryStatuses, "Id", "Code", delivery.DeliveryStatusId);
            ViewData["DeliveryWindowId"] = new SelectList(_context.DeliveryWindows, "Id", "Id", delivery.DeliveryWindowId);
            ViewData["DeliveryZoneId"] = new SelectList(_context.DeliveryZones, "Id", "Name", delivery.DeliveryZoneId);
            ViewData["MealSelectionId"] = new SelectList(_context.MealSelections, "Id", "Id", delivery.MealSelectionId);
            ViewData["MealSubscriptionId"] = new SelectList(_context.MealSubscriptions, "Id", "Id", delivery.MealSubscriptionId);
            ViewData["WeeklyMenuId"] = new SelectList(_context.WeeklyMenus, "Id", "Id", delivery.WeeklyMenuId);
            return View(delivery);
        }

        // POST: Deliveries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ScheduledTime,DeliveredAt,FailureReason,AddressLine,City,PostalCode,Country,CreatedAt,UpdatedAt,DeletedAt,DeliveryStatusId,CompanyId,CustomerId,WeeklyMenuId,DeliveryZoneId,DeliveryWindowId,BoxId,MealSelectionId,MealSubscriptionId,CreatedByAppUserId,Id")] Delivery delivery)
        {
            if (id != delivery.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(delivery);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeliveryExists(delivery.Id))
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
            ViewData["BoxId"] = new SelectList(_context.Boxes, "Id", "DisplayName", delivery.BoxId);
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "ContactEmail", delivery.CompanyId);
            ViewData["CreatedByAppUserId"] = new SelectList(_context.Users, "Id", "FirstName", delivery.CreatedByAppUserId);
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "AddressLine", delivery.CustomerId);
            ViewData["DeliveryStatusId"] = new SelectList(_context.DeliveryStatuses, "Id", "Code", delivery.DeliveryStatusId);
            ViewData["DeliveryWindowId"] = new SelectList(_context.DeliveryWindows, "Id", "Id", delivery.DeliveryWindowId);
            ViewData["DeliveryZoneId"] = new SelectList(_context.DeliveryZones, "Id", "Name", delivery.DeliveryZoneId);
            ViewData["MealSelectionId"] = new SelectList(_context.MealSelections, "Id", "Id", delivery.MealSelectionId);
            ViewData["MealSubscriptionId"] = new SelectList(_context.MealSubscriptions, "Id", "Id", delivery.MealSubscriptionId);
            ViewData["WeeklyMenuId"] = new SelectList(_context.WeeklyMenus, "Id", "Id", delivery.WeeklyMenuId);
            return View(delivery);
        }

        // GET: Deliveries/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var delivery = await _context.Deliveries
                .Include(d => d.Box)
                .Include(d => d.Company)
                .Include(d => d.CreatedByAppUser)
                .Include(d => d.Customer)
                .Include(d => d.DeliveryStatus)
                .Include(d => d.DeliveryWindow)
                .Include(d => d.DeliveryZone)
                .Include(d => d.MealSelection)
                .Include(d => d.MealSubscription)
                .Include(d => d.WeeklyMenu)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (delivery == null)
            {
                return NotFound();
            }

            return View(delivery);
        }

        // POST: Deliveries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var delivery = await _context.Deliveries.FindAsync(id);
            if (delivery != null)
            {
                _context.Deliveries.Remove(delivery);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DeliveryExists(Guid id)
        {
            return _context.Deliveries.Any(e => e.Id == id);
        }
    }
}
