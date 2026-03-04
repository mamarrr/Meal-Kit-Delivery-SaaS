using App.Domain.Delivery;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels.Deliveries;

public class DeliveryEditViewModel
{
    public Delivery Delivery { get; set; } = default!;

    public IReadOnlyList<SelectListItem> DeliveryStatusOptions { get; set; } = [];

    public IReadOnlyList<SelectListItem> CustomerOptions { get; set; } = [];

    public IReadOnlyList<SelectListItem> WeeklyMenuOptions { get; set; } = [];

    public IReadOnlyList<SelectListItem> DeliveryZoneOptions { get; set; } = [];

    public IReadOnlyList<SelectListItem> DeliveryWindowOptions { get; set; } = [];

    public IReadOnlyList<SelectListItem> BoxOptions { get; set; } = [];

    public IReadOnlyList<SelectListItem> MealSelectionOptions { get; set; } = [];

    public IReadOnlyList<SelectListItem> MealSubscriptionOptions { get; set; } = [];
}
