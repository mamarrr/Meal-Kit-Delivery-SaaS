using App.Contracts.BLL.Core;
using App.Contracts.BLL.Delivery;
using App.Domain.Delivery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.ViewModels.QualityComplaints;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Customer,CompanyOwner,CompanyAdmin,CompanyManager,CompanyEmployee")]
    public class QualityComplaintsController : Controller
    {
        private readonly IQualityComplaintService _qualityComplaintService;
        private readonly ICustomerService _customerService;
        private readonly IDeliveryService _deliveryService;
        private readonly IOperationalLookupService _operationalLookupService;

        public QualityComplaintsController(
            IQualityComplaintService qualityComplaintService,
            ICustomerService customerService,
            IDeliveryService deliveryService,
            IOperationalLookupService operationalLookupService)
        {
            _qualityComplaintService = qualityComplaintService;
            _customerService = customerService;
            _deliveryService = deliveryService;
            _operationalLookupService = operationalLookupService;
        }

        // GET: QualityComplaints
        public async Task<IActionResult> Index()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            return View(await _qualityComplaintService.GetAllByCompanyIdAsync(companyId.Value));
        }

        // GET: QualityComplaints/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            var qualityComplaint = await _qualityComplaintService.GetByIdAsync(id.Value, companyId.Value);
            if (qualityComplaint == null)
            {
                return NotFound();
            }

            return View(qualityComplaint);
        }

        // GET: QualityComplaints/Create
        public async Task<IActionResult> Create()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            return View(await BuildEditViewModelAsync(new QualityComplaint(), companyId.Value));
        }

        // POST: QualityComplaints/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QualityComplaintEditViewModel viewModel)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            if (viewModel.QualityComplaint == null)
            {
                return BadRequest();
            }

            var qualityComplaint = viewModel.QualityComplaint;
            if (ModelState.IsValid)
            {
                qualityComplaint.CreatedAt = DateTime.UtcNow;
                qualityComplaint.UpdatedAt = null;
                qualityComplaint.DeletedAt = null;
                qualityComplaint.CompanyId = companyId.Value;

                await _qualityComplaintService.AddAsync(qualityComplaint, companyId.Value);
                return RedirectToAction(nameof(Index));
            }

            return View(await BuildEditViewModelAsync(qualityComplaint, companyId.Value));
        }

        // GET: QualityComplaints/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            var qualityComplaint = await _qualityComplaintService.GetByIdAsync(id.Value, companyId.Value);
            if (qualityComplaint == null)
            {
                return NotFound();
            }

            return View(await BuildEditViewModelAsync(qualityComplaint, companyId.Value));
        }

        // POST: QualityComplaints/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, QualityComplaintEditViewModel viewModel)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            if (viewModel.QualityComplaint == null)
            {
                return BadRequest();
            }

            var qualityComplaint = viewModel.QualityComplaint;
            if (id != qualityComplaint.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existing = await _qualityComplaintService.GetByIdAsync(id, companyId.Value);
                if (existing == null)
                {
                    return NotFound();
                }

                qualityComplaint.CompanyId = companyId.Value;
                qualityComplaint.CreatedAt = existing.CreatedAt;
                qualityComplaint.UpdatedAt = DateTime.UtcNow;
                qualityComplaint.DeletedAt = existing.DeletedAt;

                await _qualityComplaintService.UpdateAsync(qualityComplaint, companyId.Value);
                return RedirectToAction(nameof(Index));
            }

            return View(await BuildEditViewModelAsync(qualityComplaint, companyId.Value));
        }

        // GET: QualityComplaints/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            var qualityComplaint = await _qualityComplaintService.GetByIdAsync(id.Value, companyId.Value);
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
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            await _qualityComplaintService.RemoveAsync(id, companyId.Value);
            return RedirectToAction(nameof(Index));
        }

        // GET: QualityComplaints/Escalate/5
        [Authorize(Roles = "CompanyManager,CompanyAdmin")]
        public async Task<IActionResult> Escalate(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            var qualityComplaint = await _qualityComplaintService.GetByIdAsync(id.Value, companyId.Value);
            if (qualityComplaint == null)
            {
                return NotFound();
            }

            return View(qualityComplaint);
        }

        // POST: QualityComplaints/Escalate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CompanyManager,CompanyAdmin")]
        public async Task<IActionResult> Escalate(Guid id, string escalationAction)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            var qualityComplaint = await _qualityComplaintService.GetByIdAsync(id, companyId.Value);
            if (qualityComplaint == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(escalationAction))
            {
                ModelState.AddModelError("escalationAction", "Escalation action is required");
                return View(qualityComplaint);
            }

            // Update escalation fields
            qualityComplaint.EscalatedAt = DateTime.UtcNow;
            qualityComplaint.EscalationAction = escalationAction;
            qualityComplaint.UpdatedAt = DateTime.UtcNow;

            await _qualityComplaintService.UpdateAsync(qualityComplaint, companyId.Value);
            return RedirectToAction(nameof(Details), new { id });
        }

        private async Task<QualityComplaintEditViewModel> BuildEditViewModelAsync(QualityComplaint qualityComplaint, Guid companyId)
        {
            var customers = await _customerService.GetAllByCompanyIdAsync(companyId);
            var deliveries = await _deliveryService.GetAllByCompanyIdAsync(companyId);
            var qualityComplaintTypes = await _operationalLookupService.GetQualityComplaintTypesAsync();
            var qualityComplaintStatuses = await _operationalLookupService.GetQualityComplaintStatusesAsync();

            return new QualityComplaintEditViewModel
            {
                QualityComplaint = qualityComplaint,
                CustomerOptions = customers
                    .Select(c => new SelectListItem($"{c.FirstName} {c.LastName} ({c.Email})", c.Id.ToString(), c.Id == qualityComplaint.CustomerId))
                    .ToList(),
                DeliveryOptions = deliveries
                    .Select(d => new SelectListItem(d.AddressLine, d.Id.ToString(), d.Id == qualityComplaint.DeliveryId))
                    .ToList(),
                QualityComplaintTypeOptions = qualityComplaintTypes
                    .Select(t => new SelectListItem(t.Label, t.Id.ToString(), t.Id == qualityComplaint.QualityComplaintTypeId))
                    .ToList(),
                QualityComplaintStatusOptions = qualityComplaintStatuses
                    .Select(s => new SelectListItem(s.Label, s.Id.ToString(), s.Id == qualityComplaint.QualityComplaintStatusId))
                    .ToList()
            };
        }

        private Guid? GetCurrentCompanyId()
        {
            var companyIdRaw = User.FindFirst("company_id")?.Value
                               ?? User.FindFirst("tenant_id")?.Value
                               ?? User.FindFirst("companyId")?.Value;

            return Guid.TryParse(companyIdRaw, out var companyId)
                ? companyId
                : null;
        }
    }
}
