using Base.Contracts.Domain;

namespace App.Domain.Subscription;

public class PlatformSubscriptionStatus : BaseEntity
{
    public string Code { get; set; } = default!;
    public string Label { get; set; } = default!;
    
    // Navigation Properties
    public ICollection<PlatformSubscription>? PlatformSubscriptions { get; set; }
}
