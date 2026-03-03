using Base.Contracts.Domain;

namespace App.Domain.Delivery;

public class DeliveryAttempt : BaseEntity
{
    public int AttemptNo { get; set; }
    public DateTime AttemptAt { get; set; }
    public string Notes { get; set; } = default!;
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    // Foreign Keys
    public Guid DeliveryAttemptResultId { get; set; }
    public Guid DeliveryId { get; set; }
    
    // Navigation Properties
    public DeliveryAttemptResult? DeliveryAttemptResult { get; set; }
    public Delivery? Delivery { get; set; }
}
