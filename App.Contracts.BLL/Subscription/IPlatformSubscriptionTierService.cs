using App.Domain.Subscription;

namespace App.Contracts.BLL.Subscription;

public interface IPlatformSubscriptionTierService : IBaseEntityService<PlatformSubscriptionTier>
{
    Task<ICollection<PlatformSubscriptionTier>> GetActiveAsync();
}

