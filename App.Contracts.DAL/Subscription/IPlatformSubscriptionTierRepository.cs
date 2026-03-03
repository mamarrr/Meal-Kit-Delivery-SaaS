using App.Domain.Subscription;
using Contracts.DAL;

namespace App.Contracts.DAL.Subscription;

/// <summary>
/// Repository interface for PlatformSubscriptionTier aggregate.
/// </summary>
public interface IPlatformSubscriptionTierRepository : IRepository<PlatformSubscriptionTier>
{
    // PlatformSubscriptionTier is not tenant-scoped, so no CompanyId filtering needed
}
