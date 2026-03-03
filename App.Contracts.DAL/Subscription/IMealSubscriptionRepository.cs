using App.Domain.Subscription;
using Contracts.DAL;

namespace App.Contracts.DAL.Subscription;

/// <summary>
/// Repository interface for MealSubscription aggregate.
/// </summary>
public interface IMealSubscriptionRepository : IRepository<MealSubscription>
{
    /// <summary>
    /// Gets all meal subscriptions for a specific customer.
    /// </summary>
    /// <param name="customerId">The customer ID.</param>
    /// <returns>A collection of meal subscriptions.</returns>
    Task<ICollection<MealSubscription>> GetAllByCustomerIdAsync(Guid customerId);
}
