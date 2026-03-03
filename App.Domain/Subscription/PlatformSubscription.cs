using App.Domain.Identity;
using Base.Contracts.Domain;

namespace App.Domain.Subscription;

public class PlatformSubscription : BaseEntity
{
    public DateTime ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    // Foreign Keys
    public Guid CompanyId { get; set; }
    public Guid PlatformSubscriptionTierId { get; set; }
    public Guid PlatformSubscriptionStatusId { get; set; }
    public Guid CreatedByAppUserId { get; set; }
    
    // Navigation Properties
    public Company? Company { get; set; }
    public PlatformSubscriptionTier? PlatformSubscriptionTier { get; set; }
    public PlatformSubscriptionStatus? PlatformSubscriptionStatus { get; set; }
    public AppUser? CreatedByAppUser { get; set; }
}
