using App.Contracts.BLL.Core;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels.Subscribers;

public class SubscribersIndexViewModel
{
    public Guid CompanyId { get; set; }
    public string CompanySlug { get; set; } = string.Empty;

    public string? Search { get; set; }
    public string? Status { get; set; }
    public string? Tier { get; set; }
    public Guid? DeliveryZoneId { get; set; }
    public Guid? SelectedSubscriberId { get; set; }

    public List<SelectListItem> StatusOptions { get; set; } = [];
    public List<SelectListItem> TierOptions { get; set; } = [];
    public List<SelectListItem> DeliveryZoneOptions { get; set; } = [];

    public List<SubscriberListItem> Subscribers { get; set; } = [];
    public SubscriberDetails? SelectedSubscriber { get; set; }
}
