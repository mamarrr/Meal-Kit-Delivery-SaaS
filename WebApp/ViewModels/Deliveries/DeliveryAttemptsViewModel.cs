using App.Domain.Delivery;

namespace WebApp.ViewModels.Deliveries;

public class DeliveryAttemptsViewModel
{
    public Delivery Delivery { get; set; } = default!;

    public IReadOnlyList<DeliveryAttempt> Attempts { get; set; } = [];
}
