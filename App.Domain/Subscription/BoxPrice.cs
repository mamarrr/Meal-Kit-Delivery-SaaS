using App.Domain.Identity;
using Base.Contracts.Domain;

namespace App.Domain.Subscription;

public class BoxPrice : BaseEntity, ITenantProvider
{
    public string PricingName { get; set; } = default!;
    public decimal PriceAmount { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public bool IsActive { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    // Foreign Keys
    public Guid BoxId { get; set; }
    public Guid CompanyId { get; set; }
    public Guid CreatedByAppUserId { get; set; }
    
    // Navigation Properties
    public Box? Box { get; set; }
    public Company? Company { get; set; }
    public AppUser? CreatedByAppUser { get; set; }
}
