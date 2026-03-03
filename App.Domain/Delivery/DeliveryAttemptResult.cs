using Base.Contracts.Domain;

namespace App.Domain.Delivery;

public class DeliveryAttemptResult : BaseEntity
{
    public string Code { get; set; } = default!;
    public string Label { get; set; } = default!;
    
    // Navigation Properties
    public ICollection<DeliveryAttempt>? DeliveryAttempts { get; set; }
}
