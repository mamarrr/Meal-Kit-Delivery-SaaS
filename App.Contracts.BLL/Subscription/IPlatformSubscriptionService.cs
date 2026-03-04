using App.Domain.Subscription;

namespace App.Contracts.BLL.Subscription;

public interface IPlatformSubscriptionService : ITenantEntityService<PlatformSubscription>
{
    /// <summary>
    /// Gets currently active platform subscription for company with tier/status included.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <returns>Latest active subscription in current validity window or null.</returns>
    Task<PlatformSubscription?> GetCurrentActiveByCompanyIdAsync(Guid companyId);

    /// <summary>
    /// Gets all non-deleted platform subscriptions for billing/invoice listing.
    /// </summary>
    /// <returns>Collection with related company/tier/status included.</returns>
    Task<ICollection<PlatformSubscription>> GetAllForBillingAsync();
}

