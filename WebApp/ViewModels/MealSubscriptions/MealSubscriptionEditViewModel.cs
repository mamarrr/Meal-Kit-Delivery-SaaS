using App.Domain.Subscription;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels.MealSubscriptions;

public class MealSubscriptionEditViewModel
{
    public MealSubscription MealSubscription { get; set; } = default!;

    public IReadOnlyList<SelectListItem> CustomerOptions { get; set; } = [];

    public IReadOnlyList<SelectListItem> BoxOptions { get; set; } = [];
}
