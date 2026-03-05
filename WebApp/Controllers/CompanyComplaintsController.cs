using System.Security.Claims;
using App.Contracts.BLL.Delivery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.ViewModels.Delivery;

namespace WebApp.Controllers;

[Authorize(Policy = "CompanyEmployee")]
public class CompanyComplaintsController(
    IQualityComplaintService qualityComplaintService) : Controller
{
    [HttpGet("/{slug}/complaints")]
    public async Task<IActionResult> Index(string slug)
    {
        if (!TryGetCompanyContext(slug, out var companyId))
        {
            return Forbid();
        }

        var complaints = (await qualityComplaintService.GetAllByCompanyIdAsync(companyId))
            .Where(q => q.DeletedAt == null)
            .OrderByDescending(q => q.CreatedAt)
            .ToList();

        var model = new CompanyComplaintsIndexViewModel
        {
            CompanyId = companyId,
            CompanySlug = slug,
            Complaints = complaints.Select(q => new CompanyComplaintListItemViewModel
            {
                ComplaintId = q.Id,
                CreatedAt = q.CreatedAt,
                CustomerName = BuildCustomerName(q.Customer),
                DeliveryReference = q.DeliveryId.ToString("N")[..8].ToUpperInvariant(),
                TypeLabel = q.QualityComplaintType?.Label ?? "Unknown",
                StatusLabel = q.QualityComplaintStatus?.Label ?? "Unknown",
                Severity = q.Severity
            }).ToList()
        };

        return View(model);
    }

    [HttpGet("/{slug}/complaints/{id:guid}")]
    public async Task<IActionResult> Details(string slug, Guid id)
    {
        if (!TryGetCompanyContext(slug, out var companyId))
        {
            return Forbid();
        }

        var complaint = await qualityComplaintService.GetByIdAsync(id, companyId);
        if (complaint == null || complaint.DeletedAt != null)
        {
            return NotFound();
        }

        var model = new CompanyComplaintDetailsViewModel
        {
            CompanyId = companyId,
            CompanySlug = slug,
            ComplaintId = complaint.Id,
            CreatedAt = complaint.CreatedAt,
            UpdatedAt = complaint.UpdatedAt,
            EscalatedAt = complaint.EscalatedAt,
            CustomerName = BuildCustomerName(complaint.Customer),
            DeliveryReference = complaint.DeliveryId.ToString("N")[..8].ToUpperInvariant(),
            TypeLabel = complaint.QualityComplaintType?.Label ?? "Unknown",
            StatusLabel = complaint.QualityComplaintStatus?.Label ?? "Unknown",
            Severity = complaint.Severity,
            Description = complaint.Description,
            EscalationAction = complaint.EscalationAction
        };

        return View(model);
    }

    private bool TryGetCompanyContext(string slug, out Guid companyId)
    {
        companyId = Guid.Empty;

        var companyIdRaw = User.FindFirstValue("company_id")
                           ?? User.FindFirstValue("tenant_id")
                           ?? User.FindFirstValue("companyId");
        var currentSlug = User.FindFirstValue("company_slug")
                          ?? User.FindFirstValue("tenant_slug");

        return Guid.TryParse(companyIdRaw, out companyId)
               && !string.IsNullOrWhiteSpace(currentSlug)
               && string.Equals(currentSlug, slug, StringComparison.OrdinalIgnoreCase);
    }

    private static string BuildCustomerName(App.Domain.Core.Customer? customer)
    {
        if (customer == null)
        {
            return "Unknown";
        }

        var fullName = $"{customer.FirstName} {customer.LastName}".Trim();
        return string.IsNullOrWhiteSpace(fullName)
            ? customer.Email
            : fullName;
    }
}
