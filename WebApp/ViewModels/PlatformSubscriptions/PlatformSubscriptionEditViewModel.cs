using App.Domain.Subscription;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels.PlatformSubscriptions;

public class PlatformSubscriptionEditViewModel
{
    public PlatformSubscription PlatformSubscription { get; set; } = default!;

    public IReadOnlyList<SelectListItem> PlatformSubscriptionTierOptions { get; set; } = [];

    public IReadOnlyList<SelectListItem> PlatformSubscriptionStatusOptions { get; set; } = [];
}

