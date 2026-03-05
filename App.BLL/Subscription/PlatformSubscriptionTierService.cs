using App.Contracts.BLL.Subscription;
using App.Contracts.DAL.Subscription;
using App.Domain.Subscription;

namespace App.BLL.Subscription;

public class PlatformSubscriptionTierService : BaseService<PlatformSubscriptionTier, IPlatformSubscriptionTierRepository>, IPlatformSubscriptionTierService
{
    public PlatformSubscriptionTierService(IPlatformSubscriptionTierRepository repository) : base(repository)
    {
    }

    public async Task<ICollection<PlatformSubscriptionTier>> GetActiveAsync()
    {
        return await Repository.GetActiveAsync();
    }
}

