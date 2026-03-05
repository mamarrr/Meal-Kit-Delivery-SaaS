using App.Contracts.BLL.Subscription;
using App.Contracts.DAL.Subscription;
using App.Domain.Subscription;

namespace App.BLL.Subscription;

public class MealSubscriptionService : BaseTenantService<MealSubscription, IMealSubscriptionRepository>, IMealSubscriptionService
{
    public MealSubscriptionService(IMealSubscriptionRepository repository) : base(repository)
    {
    }

    protected override async Task<ICollection<MealSubscription>> GetAllByCompanyIdCoreAsync(Guid companyId)
    {
        return await Repository.GetAllByCompanyIdAsync(companyId);
    }

    public async Task<ICollection<MealSubscription>> GetAllByCustomerIdAsync(Guid customerId, Guid companyId)
    {
        return await Repository.GetAllByCustomerIdAsync(customerId, companyId);
    }

    public async Task<ICollection<MealSubscription>> GetAllByCustomerIdsAsync(IReadOnlyCollection<Guid> customerIds)
    {
        return await Repository.GetAllByCustomerIdsAsync(customerIds);
    }

    public async Task<ICollection<Guid>> GetDistinctCompanyIdsByCustomerIdAsync(Guid customerId)
    {
        return await Repository.GetDistinctCompanyIdsByCustomerIdAsync(customerId);
    }
}

