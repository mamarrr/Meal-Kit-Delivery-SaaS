using App.Contracts.DAL.Subscription;
using App.Domain.Subscription;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories.Subscription;

/// <summary>
/// EF Core repository implementation for PlatformSubscriptionTier aggregate.
/// </summary>
public class PlatformSubscriptionTierRepository : BaseRepository<PlatformSubscriptionTier, AppDbContext>, IPlatformSubscriptionTierRepository
{
    public PlatformSubscriptionTierRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<ICollection<PlatformSubscriptionTier>> GetActiveAsync()
    {
        return await RepositoryDbSet
            .Where(x => x.IsActive && x.DeletedAt == null)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }
}
