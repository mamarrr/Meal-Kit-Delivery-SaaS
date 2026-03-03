using App.Domain.Identity;
using Base.Contracts.Domain;

namespace App.Domain.Delivery;

public class DeliveryZone : BaseEntity, ITenantProvider
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    // Foreign Keys
    public Guid CompanyId { get; set; }
    public Guid CreatedByAppUserId { get; set; }
    
    // Navigation Properties
    public Company? Company { get; set; }
    public AppUser? CreatedByAppUser { get; set; }
    public ICollection<DeliveryWindow>? DeliveryWindows { get; set; }
    public ICollection<Delivery>? Deliveries { get; set; }
}
