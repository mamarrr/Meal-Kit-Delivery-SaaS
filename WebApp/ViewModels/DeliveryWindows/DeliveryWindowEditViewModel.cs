using App.Domain.Delivery;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels.DeliveryWindows;

public class DeliveryWindowEditViewModel
{
    public DeliveryWindow DeliveryWindow { get; set; } = default!;
    public List<SelectListItem> DeliveryZoneOptions { get; set; } = new();
}
