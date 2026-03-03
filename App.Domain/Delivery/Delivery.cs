using App.Domain.Identity;
using Base.Contracts.Domain;

namespace App.Domain.Delivery;

public class Delivery : BaseEntity, ITenantProvider
{
    public DateTime ScheduledTime { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public string? FailureReason { get; set; }
    public string AddressLine { get; set; } = default!;
    public string City { get; set; } = default!;
    public string PostalCode { get; set; } = default!;
    public string Country { get; set; } = default!;
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    // Foreign Keys
    public Guid DeliveryStatusId { get; set; }
    public Guid CompanyId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid WeeklyMenuId { get; set; }
    public Guid DeliveryZoneId { get; set; }
    public Guid DeliveryWindowId { get; set; }
    public Guid BoxId { get; set; }
    public Guid MealSelectionId { get; set; }
    public Guid MealSubscriptionId { get; set; }
    public Guid CreatedByAppUserId { get; set; }
    
    // Navigation Properties
    public DeliveryStatus? DeliveryStatus { get; set; }
    public Company? Company { get; set; }
    public Customer? Customer { get; set; }
    public WeeklyMenu? WeeklyMenu { get; set; }
    public DeliveryZone? DeliveryZone { get; set; }
    public DeliveryWindow? DeliveryWindow { get; set; }
    public Box? Box { get; set; }
    public MealSelection? MealSelection { get; set; }
    public MealSubscription? MealSubscription { get; set; }
    public AppUser? CreatedByAppUser { get; set; }
    public ICollection<DeliveryAttempt>? DeliveryAttempts { get; set; }
    public ICollection<QualityComplaint>? QualityComplaints { get; set; }
}
