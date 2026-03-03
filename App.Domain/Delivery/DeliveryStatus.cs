using Base.Contracts.Domain;

namespace App.Domain.Delivery;

public class DeliveryStatus : BaseEntity
{
    public string Code { get; set; } = default!;
    public string Label { get; set; } = default!;
    
    // Navigation Properties
    public ICollection<Delivery>? Deliveries { get; set; }
}
