using App.Contracts.DAL.Subscription;
using App.Domain.Subscription;
using Base.DAL.EF;

namespace App.DAL.EF.Repositories.Subscription;

/// <summary>
/// EF Core repository implementation for PlatformSubscriptionStatus lookup aggregate.
/// </summary>
public class PlatformSubscriptionStatusRepository : BaseRepository<PlatformSubscriptionStatus, AppDbContext>, IPlatformSubscriptionStatusRepository
{
    public PlatformSubscriptionStatusRepository(AppDbContext context) : base(context)
    {
    }
}

