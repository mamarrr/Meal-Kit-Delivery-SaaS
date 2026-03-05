using System.Security.Claims;
using App.Contracts.BLL.Delivery;
using App.Contracts.DAL.Delivery;
using App.DAL.EF;
using App.Domain.Delivery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.ViewModels.Delivery;

namespace WebApp.Controllers;

[Authorize(Policy = "CustomerAccess")]
public class CustomerComplaintsController(
    IQualityComplaintService qualityComplaintService,
    IOperationalLookupService operationalLookupService,
    IDeliveryRepository deliveryRepository,
    AppDbContext dbContext,
    ILogger<CustomerComplaintsController> logger) : Controller
{
    [HttpGet("/customer/complaints")]
    public async Task<IActionResult> Index()
    {
        var customer = await ResolveCustomerAsync();
        if (customer == null)
        {
            return Forbid();
        }

        var complaints = (await qualityComplaintService.GetAllByCustomerIdAsync(customer.Id))
            .Where(qc => qc.DeletedAt == null)
            .OrderByDescending(qc => qc.CreatedAt)
            .ToList();

        var model = new CustomerComplaintsIndexViewModel
        {
            Complaints = complaints.Select(qc => new CustomerComplaintListItemViewModel
            {
                ComplaintId = qc.Id,
                DeliveryId = qc.DeliveryId,
                CreatedAt = qc.CreatedAt,
                StatusLabel = qc.QualityComplaintStatus?.Label ?? "Unknown",
                TypeLabel = qc.QualityComplaintType?.Label ?? "Unknown",
                Severity = qc.Severity
            }).ToList()
        };

        return View(model);
    }

    [HttpGet("/customer/complaints/create")]
    public async Task<IActionResult> Create(Guid? deliveryId)
    {
        var customer = await ResolveCustomerAsync();
        if (customer == null)
        {
            return Forbid();
        }

        var model = await BuildCreateModelAsync(customer.Id, deliveryId, null);
        return View(model);
    }

    [HttpPost("/customer/complaints/create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CustomerComplaintCreateViewModel model)
    {
        var customer = await ResolveCustomerAsync();
        if (customer == null)
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            var invalidModel = await BuildCreateModelAsync(customer.Id, null, model);
            return View(invalidModel);
        }

        if (!model.DeliveryId.HasValue || !model.ComplaintTypeId.HasValue)
        {
            ModelState.AddModelError(string.Empty, "Delivery and complaint type are required.");
            var invalidModel = await BuildCreateModelAsync(customer.Id, null, model);
            return View(invalidModel);
        }

        try
        {
            var deliveryAllowed = await deliveryRepository.DeliveryBelongsToCustomerAsync(model.DeliveryId.Value, customer.Id);
            if (!deliveryAllowed)
            {
                ModelState.AddModelError(string.Empty, "Selected delivery is outside your scope.");
                var invalidModel = await BuildCreateModelAsync(customer.Id, null, model);
                return View(invalidModel);
            }

            var statusId = await ResolveOpenStatusIdAsync();
            if (!statusId.HasValue)
            {
                ModelState.AddModelError(string.Empty, "Complaint statuses are not configured.");
                var invalidModel = await BuildCreateModelAsync(customer.Id, null, model);
                return View(invalidModel);
            }

            var now = DateTime.UtcNow;
            await qualityComplaintService.AddAsync(new QualityComplaint
            {
                CompanyId = customer.CompanyId,
                CustomerId = customer.Id,
                DeliveryId = model.DeliveryId.Value,
                QualityComplaintTypeId = model.ComplaintTypeId.Value,
                QualityComplaintStatusId = statusId.Value,
                Severity = model.Severity,
                Description = model.Description,
                CreatedAt = now,
                UpdatedAt = now,
                DeletedAt = null
            }, customer.CompanyId);

            await dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Complaint submitted.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "CustomerComplaints/Create failed");
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        var errorModel = await BuildCreateModelAsync(customer.Id, null, model);
        return View(errorModel);
    }

    [HttpGet("/customer/complaints/{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        var customer = await ResolveCustomerAsync();
        if (customer == null)
        {
            return Forbid();
        }

        var complaint = await qualityComplaintService.GetByCustomerIdAsync(id, customer.Id);
        if (complaint == null || complaint.DeletedAt != null)
        {
            return NotFound();
        }

        var model = new CustomerComplaintDetailsViewModel
        {
            ComplaintId = complaint.Id,
            DeliveryId = complaint.DeliveryId,
            TypeLabel = complaint.QualityComplaintType?.Label ?? "Unknown",
            StatusLabel = complaint.QualityComplaintStatus?.Label ?? "Unknown",
            Severity = complaint.Severity,
            Description = complaint.Description,
            CreatedAt = complaint.CreatedAt,
            UpdatedAt = complaint.UpdatedAt,
            EscalatedAt = complaint.EscalatedAt,
            EscalationAction = complaint.EscalationAction
        };

        return View(model);
    }

    private async Task<CustomerComplaintCreateViewModel> BuildCreateModelAsync(
        Guid customerId,
        Guid? preferredDeliveryId,
        CustomerComplaintCreateViewModel? input)
    {
        var deliveries = await dbContext.Deliveries
            .Where(d => d.CustomerId == customerId && d.DeletedAt == null)
            .OrderByDescending(d => d.ScheduledTime)
            .ToListAsync();

        var types = await operationalLookupService.GetQualityComplaintTypesAsync();

        return new CustomerComplaintCreateViewModel
        {
            DeliveryId = input?.DeliveryId ?? preferredDeliveryId,
            ComplaintTypeId = input?.ComplaintTypeId,
            Severity = input?.Severity ?? 3,
            Description = input?.Description ?? string.Empty,
            DeliveryOptions = deliveries.Select(d => new SelectListItem(
                $"{d.ScheduledTime:yyyy-MM-dd} ({d.Id.ToString()[..8]})", d.Id.ToString(), (input?.DeliveryId ?? preferredDeliveryId) == d.Id)).ToList(),
            ComplaintTypeOptions = types.Select(t => new SelectListItem(
                t.Label, t.Id.ToString(), input?.ComplaintTypeId == t.Id)).ToList()
        };
    }

    private async Task<Guid?> ResolveOpenStatusIdAsync()
    {
        var statuses = await operationalLookupService.GetQualityComplaintStatusesAsync();
        return statuses.FirstOrDefault(s => s.Code == "open")?.Id;
    }

    private async Task<App.Domain.Core.Customer?> ResolveCustomerAsync()
    {
        var userId = GetCurrentUserId();

        var mappedCustomers = await dbContext.CustomerAppUsers
            .Where(link => link.AppUserId == userId && link.Customer != null)
            .Select(link => new
            {
                link.CustomerId,
                CompanyId = link.Customer!.CompanyId,
                CustomerDeletedAt = link.Customer!.DeletedAt
            })
            .ToListAsync();

        logger.LogInformation(
            "CustomerComplaints.ResolveCustomer start: userId={UserId}, claimCompanyId={ClaimCompanyId}, claimCompanySlug={ClaimCompanySlug}, mappings=[{Mappings}]",
            userId,
            User.FindFirstValue("company_id") ?? "<null>",
            User.FindFirstValue("company_slug") ?? "<null>",
            string.Join(",", mappedCustomers.Select(x => $"{x.CustomerId}@{x.CompanyId}:deleted={x.CustomerDeletedAt != null}")));

        var customerId = await dbContext.CustomerAppUsers
            .Where(link => link.AppUserId == userId && link.Customer != null && link.Customer.DeletedAt == null)
            .Select(link => link.CustomerId)
            .FirstOrDefaultAsync();

        if (customerId == Guid.Empty)
        {
            logger.LogWarning("CustomerComplaints.ResolveCustomer unresolved mapping: userId={UserId}", userId);
            return null;
        }

        var customer = await dbContext.Customers
            .FirstOrDefaultAsync(c => c.Id == customerId && c.DeletedAt == null);

        logger.LogInformation(
            "CustomerComplaints.ResolveCustomer result: userId={UserId}, customerId={CustomerId}, resolved={Resolved}",
            userId,
            customerId,
            customer != null);

        return customer;
    }

    private Guid GetCurrentUserId()
    {
        var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? User.FindFirstValue("sub")
                        ?? User.FindFirstValue("user_id");

        if (!Guid.TryParse(userIdRaw, out var userId))
        {
            throw new UnauthorizedAccessException("Unable to resolve current user id.");
        }

        return userId;
    }
}
