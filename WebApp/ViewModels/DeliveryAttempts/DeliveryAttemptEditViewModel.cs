using App.Domain.Delivery;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels.DeliveryAttempts;

public class DeliveryAttemptEditViewModel
{
    public DeliveryAttempt DeliveryAttempt { get; set; } = default!;

    public IReadOnlyList<SelectListItem> DeliveryOptions { get; set; } = [];

    public IReadOnlyList<SelectListItem> DeliveryAttemptResultOptions { get; set; } = [];
}
