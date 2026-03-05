using App.Contracts.DAL.Delivery;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories.Delivery;

/// <summary>
/// EF Core repository implementation for Delivery aggregate.
/// </summary>
public class DeliveryRepository : BaseRepository<App.Domain.Delivery.Delivery, AppDbContext>, IDeliveryRepository
{
    public DeliveryRepository(AppDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<ICollection<App.Domain.Delivery.Delivery>> GetAllByCompanyIdAsync(Guid companyId)
    {
        return await RepositoryDbSet
            .Where(d => d.CompanyId == companyId)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<ICollection<App.Domain.Delivery.Delivery>> GetAllByCustomerIdAsync(Guid customerId)
    {
        return await RepositoryDbSet
            .Include(d => d.DeliveryStatus)
            .Include(d => d.Box)
            .Include(d => d.DeliveryWindow)
            .Where(d => RepositoryDbContext.MealSubscriptions
                .Any(ms => ms.Id == d.MealSubscriptionId && ms.CustomerId == customerId))
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<ICollection<App.Domain.Delivery.Delivery>> GetAllByCustomerIdAsync(Guid customerId, Guid companyId)
    {
        return await RepositoryDbSet
            .Include(d => d.DeliveryStatus)
            .Include(d => d.Box)
            .Include(d => d.DeliveryWindow)
            .Where(d => d.CustomerId == customerId && d.CompanyId == companyId)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<ICollection<App.Domain.Delivery.Delivery>> GetAllByCompanyAndScheduledDateAsync(Guid companyId, DateTime scheduledDateUtc)
    {
        var date = scheduledDateUtc.Date;
        return await RepositoryDbSet
            .Where(d => d.CompanyId == companyId && d.ScheduledTime.Date == date)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<bool> DeliveryBelongsToCustomerAsync(Guid deliveryId, Guid customerId)
    {
        return await RepositoryDbSet
            .AnyAsync(d => d.Id == deliveryId && d.CustomerId == customerId);
    }
}
