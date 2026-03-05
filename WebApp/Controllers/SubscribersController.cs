using System.Security.Claims;
using App.Contracts.BLL.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.ViewModels.Subscribers;

namespace WebApp.Controllers;

[Authorize(Policy = "CompanyEmployee")]
public class SubscribersController(ICustomerService customerService) : Controller
{
    [HttpGet("/{slug}/subscribers")]
    public async Task<IActionResult> Index(
        string slug,
        string? search,
        string? status,
        string? tier,
        Guid? deliveryZoneId,
        Guid? selectedSubscriberId)
    {
        if (!TryGetCompanyContext(slug, out var companyId))
        {
            return Forbid();
        }

        var subscribers = await customerService.GetSubscriberListAsync(companyId, search, status, tier, deliveryZoneId);
        var selected = selectedSubscriberId.HasValue
            ? await customerService.GetSubscriberDetailsAsync(companyId, selectedSubscriberId.Value)
            : null;

        var tiers = subscribers
            .Select(x => x.Tier)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x)
            .ToList();

        var zones = subscribers
            .Where(x => x.DeliveryZoneId.HasValue)
            .GroupBy(x => x.DeliveryZoneId)
            .Select(g => new { Id = g.Key!.Value, Name = g.Select(x => x.DeliveryZoneName).FirstOrDefault() ?? string.Empty })
            .OrderBy(x => x.Name)
            .ToList();

        var model = new SubscribersIndexViewModel
        {
            CompanyId = companyId,
            CompanySlug = slug,
            Search = search,
            Status = status,
            Tier = tier,
            DeliveryZoneId = deliveryZoneId,
            SelectedSubscriberId = selectedSubscriberId,
            Subscribers = subscribers.ToList(),
            SelectedSubscriber = selected,
            StatusOptions =
            [
                new SelectListItem("All", string.Empty, string.IsNullOrWhiteSpace(status)),
                new SelectListItem("Active", "active", string.Equals(status, "active", StringComparison.OrdinalIgnoreCase)),
                new SelectListItem("Inactive", "inactive", string.Equals(status, "inactive", StringComparison.OrdinalIgnoreCase))
            ],
            TierOptions = [new SelectListItem("All", string.Empty, string.IsNullOrWhiteSpace(tier))],
            DeliveryZoneOptions = [new SelectListItem("All", string.Empty, !deliveryZoneId.HasValue)]
        };

        model.TierOptions.AddRange(tiers.Select(x =>
            new SelectListItem(x, x, string.Equals(x, tier, StringComparison.OrdinalIgnoreCase))));

        model.DeliveryZoneOptions.AddRange(zones.Select(x =>
            new SelectListItem(x.Name, x.Id.ToString(), deliveryZoneId == x.Id)));

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
}
