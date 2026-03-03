using App.Contracts.DAL.Subscription;
using App.Domain.Subscription;
using Base.DAL.EF;

namespace App.DAL.EF.Repositories.Subscription;

/// <summary>
/// EF Core repository implementation for PlatformSubscriptionTier aggregate.
/// </summary>
public class PlatformSubscriptionTierRepository : BaseRepository<PlatformSubscriptionTier, AppDbContext>, IPlatformSubscriptionTierRepository
{
    public PlatformSubscriptionTierRepository(AppDbContext context) : base(context)
    {
    }
}
