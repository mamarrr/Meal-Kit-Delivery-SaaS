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
    public class QualityComplaintsController : Controller
    {
        private readonly AppDbContext _context;

        public QualityComplaintsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: QualityComplaints
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.QualityComplaints.Include(q => q.Company).Include(q => q.Customer).Include(q => q.Delivery).Include(q => q.QualityComplaintStatus).Include(q => q.QualityComplaintType);
            return View(await appDbContext.ToListAsync());
        }

        // GET: QualityComplaints/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var qualityComplaint = await _context.QualityComplaints
                .Include(q => q.Company)
                .Include(q => q.Customer)
                .Include(q => q.Delivery)
                .Include(q => q.QualityComplaintStatus)
                .Include(q => q.QualityComplaintType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (qualityComplaint == null)
            {
                return NotFound();
            }

            return View(qualityComplaint);
        }

        // GET: QualityComplaints/Create
        public IActionResult Create()
        {
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "ContactEmail");
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "AddressLine");
            ViewData["DeliveryId"] = new SelectList(_context.Deliveries, "Id", "AddressLine");
            ViewData["QualityComplaintStatusId"] = new SelectList(_context.QualityComplaintStatuses, "Id", "Code");
            ViewData["QualityComplaintTypeId"] = new SelectList(_context.QualityComplaintTypes, "Id", "Code");
            return View();
        }

        // POST: QualityComplaints/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Severity,Description,EscalatedAt,EscalationAction,CreatedAt,UpdatedAt,DeletedAt,CompanyId,CustomerId,DeliveryId,QualityComplaintTypeId,QualityComplaintStatusId,Id")] QualityComplaint qualityComplaint)
        {
            if (ModelState.IsValid)
            {
                qualityComplaint.Id = Guid.NewGuid();
                _context.Add(qualityComplaint);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "ContactEmail", qualityComplaint.CompanyId);
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "AddressLine", qualityComplaint.CustomerId);
            ViewData["DeliveryId"] = new SelectList(_context.Deliveries, "Id", "AddressLine", qualityComplaint.DeliveryId);
            ViewData["QualityComplaintStatusId"] = new SelectList(_context.QualityComplaintStatuses, "Id", "Code", qualityComplaint.QualityComplaintStatusId);
            ViewData["QualityComplaintTypeId"] = new SelectList(_context.QualityComplaintTypes, "Id", "Code", qualityComplaint.QualityComplaintTypeId);
            return View(qualityComplaint);
        }

        // GET: QualityComplaints/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var qualityComplaint = await _context.QualityComplaints.FindAsync(id);
            if (qualityComplaint == null)
            {
                return NotFound();
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "ContactEmail", qualityComplaint.CompanyId);
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "AddressLine", qualityComplaint.CustomerId);
            ViewData["DeliveryId"] = new SelectList(_context.Deliveries, "Id", "AddressLine", qualityComplaint.DeliveryId);
            ViewData["QualityComplaintStatusId"] = new SelectList(_context.QualityComplaintStatuses, "Id", "Code", qualityComplaint.QualityComplaintStatusId);
            ViewData["QualityComplaintTypeId"] = new SelectList(_context.QualityComplaintTypes, "Id", "Code", qualityComplaint.QualityComplaintTypeId);
            return View(qualityComplaint);
        }

        // POST: QualityComplaints/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Severity,Description,EscalatedAt,EscalationAction,CreatedAt,UpdatedAt,DeletedAt,CompanyId,CustomerId,DeliveryId,QualityComplaintTypeId,QualityComplaintStatusId,Id")] QualityComplaint qualityComplaint)
        {
            if (id != qualityComplaint.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(qualityComplaint);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QualityComplaintExists(qualityComplaint.Id))
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
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "ContactEmail", qualityComplaint.CompanyId);
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "AddressLine", qualityComplaint.CustomerId);
            ViewData["DeliveryId"] = new SelectList(_context.Deliveries, "Id", "AddressLine", qualityComplaint.DeliveryId);
            ViewData["QualityComplaintStatusId"] = new SelectList(_context.QualityComplaintStatuses, "Id", "Code", qualityComplaint.QualityComplaintStatusId);
            ViewData["QualityComplaintTypeId"] = new SelectList(_context.QualityComplaintTypes, "Id", "Code", qualityComplaint.QualityComplaintTypeId);
            return View(qualityComplaint);
        }

        // GET: QualityComplaints/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var qualityComplaint = await _context.QualityComplaints
                .Include(q => q.Company)
                .Include(q => q.Customer)
                .Include(q => q.Delivery)
                .Include(q => q.QualityComplaintStatus)
                .Include(q => q.QualityComplaintType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (qualityComplaint == null)
            {
                return NotFound();
            }

            return View(qualityComplaint);
        }

        // POST: QualityComplaints/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var qualityComplaint = await _context.QualityComplaints.FindAsync(id);
            if (qualityComplaint != null)
            {
                _context.QualityComplaints.Remove(qualityComplaint);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QualityComplaintExists(Guid id)
        {
            return _context.QualityComplaints.Any(e => e.Id == id);
        }
    }
}
