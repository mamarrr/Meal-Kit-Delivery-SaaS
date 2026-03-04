using App.Domain.Subscription;
using Contracts.DAL;

namespace App.Contracts.DAL.Subscription;

/// <summary>
/// Repository interface for PlatformSubscription aggregate.
/// </summary>
public interface IPlatformSubscriptionRepository : IRepository<PlatformSubscription>
{
    /// <summary>
    /// Gets all platform subscriptions for a specific company.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <returns>A collection of platform subscriptions.</returns>
    Task<ICollection<PlatformSubscription>> GetAllByCompanyIdAsync(Guid companyId);

    /// <summary>
    /// Gets currently active platform subscription for company with tier/status included.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <returns>Latest active subscription in current validity window or null.</returns>
    Task<PlatformSubscription?> GetCurrentActiveByCompanyIdAsync(Guid companyId);

    /// <summary>
    /// Gets all non-deleted platform subscriptions with related company/tier/status included.
    /// </summary>
    /// <returns>Collection of subscriptions for billing/invoice listing.</returns>
    Task<ICollection<PlatformSubscription>> GetAllForBillingAsync();
}
