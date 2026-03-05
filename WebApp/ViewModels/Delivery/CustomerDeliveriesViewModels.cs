using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels.Delivery;

public class CustomerDeliveriesIndexViewModel
{
    public List<CustomerDeliveryListItemViewModel> Deliveries { get; set; } = [];
}

public class CustomerDeliveryListItemViewModel
{
    public Guid DeliveryId { get; set; }
    public DateTime ScheduledTime { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public string StatusLabel { get; set; } = string.Empty;
    public string BoxName { get; set; } = string.Empty;
    public int MealsCount { get; set; }
    public int PeopleCount { get; set; }
    public string? WindowLabel { get; set; }
    public string? FailureReason { get; set; }
}

public class CustomerDeliveryDetailsViewModel
{
    public Guid DeliveryId { get; set; }
    public DateTime ScheduledTime { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public string StatusLabel { get; set; } = string.Empty;
    public string BoxName { get; set; } = string.Empty;
    public int MealsCount { get; set; }
    public int PeopleCount { get; set; }
    public string? WindowLabel { get; set; }
    public string AddressLine { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? FailureReason { get; set; }
}
