using App.Domain.Identity;
using Base.Contracts.Domain;

namespace App.Domain.Subscription;

public class PlatformSubscriptionTier : BaseEntity
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public int MaxZones { get; set; }
    public int MaxSubscribers { get; set; }
    public int MaxEmployees { get; set; }
    public int MaxRecipes { get; set; }
    public bool IsActive { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    // Foreign Keys
    public Guid? CreatedByAppUserId { get; set; }
    
    // Navigation Properties
    public AppUser? CreatedByAppUser { get; set; }
    public ICollection<PlatformSubscription>? PlatformSubscriptions { get; set; }
}
