using App.Contracts.BLL.Subscription;
using App.Contracts.DAL.Subscription;
using App.Domain.Subscription;

namespace App.BLL.Subscription;

public class PlatformSubscriptionStatusService : BaseService<PlatformSubscriptionStatus, IPlatformSubscriptionStatusRepository>, IPlatformSubscriptionStatusService
{
    public PlatformSubscriptionStatusService(IPlatformSubscriptionStatusRepository repository) : base(repository)
    {
    }
}

