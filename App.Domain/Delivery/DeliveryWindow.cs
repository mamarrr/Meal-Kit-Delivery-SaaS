using App.Domain.Identity;
using Base.Contracts.Domain;

namespace App.Domain.Delivery;

public class DeliveryWindow : BaseEntity
{
    public int DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int? Capacity { get; set; }
    public bool IsActive { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    // Foreign Keys
    public Guid DeliveryZoneId { get; set; }
    public Guid CreatedByAppUserId { get; set; }
    
    // Navigation Properties
    public DeliveryZone? DeliveryZone { get; set; }
    public AppUser? CreatedByAppUser { get; set; }
    public ICollection<Delivery>? Deliveries { get; set; }
}
