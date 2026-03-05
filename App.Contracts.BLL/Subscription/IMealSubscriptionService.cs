using App.Domain.Subscription;

namespace App.Contracts.BLL.Subscription;

public interface IMealSubscriptionService : ITenantEntityService<MealSubscription>
{
    Task<ICollection<MealSubscription>> GetAllByCustomerIdAsync(Guid customerId, Guid companyId);
    Task<ICollection<MealSubscription>> GetAllByCustomerIdsAsync(IReadOnlyCollection<Guid> customerIds);
    
    /// <summary>
    /// Gets distinct company IDs from a customer's active subscriptions.
    /// </summary>
    /// <param name="customerId">The customer ID.</param>
    /// <returns>A collection of company IDs the customer has active subscriptions with.</returns>
    Task<ICollection<Guid>> GetDistinctCompanyIdsByCustomerIdAsync(Guid customerId);
}

