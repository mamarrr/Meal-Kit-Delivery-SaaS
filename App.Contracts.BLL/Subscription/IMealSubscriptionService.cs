using App.Domain.Subscription;

namespace App.Contracts.BLL.Subscription;

public interface IMealSubscriptionService : ITenantEntityService<MealSubscription>
{
    Task<ICollection<MealSubscription>> GetAllByCustomerIdAsync(Guid customerId, Guid companyId);
}

