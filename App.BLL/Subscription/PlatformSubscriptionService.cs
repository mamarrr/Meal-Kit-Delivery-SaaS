using App.Contracts.BLL.Subscription;
using App.Contracts.DAL.Subscription;
using App.Domain.Subscription;

namespace App.BLL.Subscription;

public class PlatformSubscriptionService : BaseTenantService<PlatformSubscription, IPlatformSubscriptionRepository>, IPlatformSubscriptionService
{
    public PlatformSubscriptionService(IPlatformSubscriptionRepository repository) : base(repository)
    {
    }

    protected override async Task<ICollection<PlatformSubscription>> GetAllByCompanyIdCoreAsync(Guid companyId)
    {
        return await Repository.GetAllByCompanyIdAsync(companyId);
    }
}

