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
}
