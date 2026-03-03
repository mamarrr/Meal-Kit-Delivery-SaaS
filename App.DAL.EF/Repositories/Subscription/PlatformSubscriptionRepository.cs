using App.Contracts.DAL.Subscription;
using App.Domain.Subscription;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories.Subscription;

/// <summary>
/// EF Core repository implementation for PlatformSubscription aggregate.
/// </summary>
public class PlatformSubscriptionRepository : BaseRepository<PlatformSubscription, AppDbContext>, IPlatformSubscriptionRepository
{
    public PlatformSubscriptionRepository(AppDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<ICollection<PlatformSubscription>> GetAllByCompanyIdAsync(Guid companyId)
    {
        return await RepositoryDbSet
            .Where(ps => ps.CompanyId == companyId)
            .ToListAsync();
    }
}
