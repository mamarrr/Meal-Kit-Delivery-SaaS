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
}
