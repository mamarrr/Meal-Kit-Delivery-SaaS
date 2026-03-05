using App.Contracts.DAL.Subscription;
using App.Domain.Subscription;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories.Subscription;

/// <summary>
/// EF Core repository implementation for MealSubscription aggregate.
/// </summary>
public class MealSubscriptionRepository : BaseRepository<MealSubscription, AppDbContext>, IMealSubscriptionRepository
{
    public MealSubscriptionRepository(AppDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<ICollection<MealSubscription>> GetAllByCompanyIdAsync(Guid companyId)
    {
        return await RepositoryDbSet
            .Where(ms => ms.CompanyId == companyId)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<ICollection<MealSubscription>> GetAllByCustomerIdAsync(Guid customerId)
    {
        return await RepositoryDbSet
            .Where(ms => ms.CustomerId == customerId)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<ICollection<MealSubscription>> GetAllByCustomerIdAsync(Guid customerId, Guid companyId)
    {
        return await RepositoryDbSet
            .Where(ms => ms.CustomerId == customerId && ms.CompanyId == companyId)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<ICollection<MealSubscription>> GetAllByCustomerIdsAsync(IReadOnlyCollection<Guid> customerIds)
    {
        if (customerIds.Count == 0)
        {
            return [];
        }

        return await RepositoryDbSet
            .Include(ms => ms.Box)
            .Include(ms => ms.Customer)
            .Where(ms => customerIds.Contains(ms.CustomerId) && ms.DeletedAt == null)
            .OrderByDescending(ms => ms.IsActive)
            .ThenByDescending(ms => ms.StartDate)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<ICollection<Guid>> GetDistinctCompanyIdsByCustomerIdAsync(Guid customerId)
    {
        return await RepositoryDbSet
            .Where(ms => ms.CustomerId == customerId && ms.IsActive && ms.DeletedAt == null)
            .Select(ms => ms.CompanyId)
            .Distinct()
            .ToListAsync();
    }
}
