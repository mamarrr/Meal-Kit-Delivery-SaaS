using App.Domain.Subscription;
using Contracts.DAL;

namespace App.Contracts.DAL.Subscription;

/// <summary>
/// Repository interface for MealSubscription aggregate.
/// </summary>
public interface IMealSubscriptionRepository : IRepository<MealSubscription>
{
    /// <summary>
    /// Gets all meal subscriptions for a specific company.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <returns>A collection of meal subscriptions belonging to the company.</returns>
    Task<ICollection<MealSubscription>> GetAllByCompanyIdAsync(Guid companyId);

    /// <summary>
    /// Gets all meal subscriptions for a specific customer.
    /// </summary>
    /// <param name="customerId">The customer ID.</param>
    /// <returns>A collection of meal subscriptions.</returns>
    Task<ICollection<MealSubscription>> GetAllByCustomerIdAsync(Guid customerId);

    /// <summary>
    /// Gets all meal subscriptions for a specific customer within company scope.
    /// </summary>
    /// <param name="customerId">The customer ID.</param>
    /// <param name="companyId">The company ID.</param>
    /// <returns>A collection of meal subscriptions for the customer in the company.</returns>
    Task<ICollection<MealSubscription>> GetAllByCustomerIdAsync(Guid customerId, Guid companyId);

    /// <summary>
    /// Gets all meal subscriptions for a set of customers across company scope.
    /// </summary>
    /// <param name="customerIds">Customer IDs.</param>
    /// <returns>A collection of meal subscriptions.</returns>
    Task<ICollection<MealSubscription>> GetAllByCustomerIdsAsync(IReadOnlyCollection<Guid> customerIds);

    /// <summary>
    /// Gets distinct company IDs from a customer's active subscriptions.
    /// </summary>
    /// <param name="customerId">The customer ID.</param>
    /// <returns>A collection of company IDs the customer has active subscriptions with.</returns>
    Task<ICollection<Guid>> GetDistinctCompanyIdsByCustomerIdAsync(Guid customerId);
}
