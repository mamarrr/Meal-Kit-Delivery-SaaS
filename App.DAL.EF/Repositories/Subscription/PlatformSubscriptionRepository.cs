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

    /// <inheritdoc />
    public async Task<PlatformSubscription?> GetCurrentActiveByCompanyIdAsync(Guid companyId)
    {
        return await RepositoryDbSet
            .Include(ps => ps.PlatformSubscriptionTier)
            .Include(ps => ps.PlatformSubscriptionStatus)
            .Where(ps => ps.CompanyId == companyId
                         && ps.DeletedAt == null
                         && ps.ValidFrom <= DateTime.UtcNow
                         && (ps.ValidTo == null || ps.ValidTo >= DateTime.UtcNow)
                         && ps.PlatformSubscriptionStatus != null
                         && ps.PlatformSubscriptionStatus.Code == "active")
            .OrderByDescending(ps => ps.ValidFrom)
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc />
    public async Task<ICollection<PlatformSubscription>> GetAllForBillingAsync()
    {
        return await RepositoryDbSet
            .Include(ps => ps.Company)
            .Include(ps => ps.PlatformSubscriptionTier)
            .Include(ps => ps.PlatformSubscriptionStatus)
            .Where(ps => ps.DeletedAt == null)
            .OrderByDescending(ps => ps.ValidFrom)
            .ToListAsync();
    }
}
